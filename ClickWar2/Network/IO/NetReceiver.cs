using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ClickWar2.Utility;
using ClickWar2.Network.Protocol;
using System.Threading;

namespace ClickWar2.Network.IO
{
    public class NetReceiver
    {
        public NetReceiver(NetSender sender)
        {
            this.Sender = sender;

            m_resetIndexForReceivedSeqFlags = -sender.MaxBackupSequence / 2;
        }

        ~NetReceiver()
        {
            Stop();
        }

        //#####################################################################################
        // 수신자 정보

        protected Socket m_socket;

        protected NetSender Sender
        { get; set; } = null;

        //#####################################################################################
        // 메세지

        protected SafeQueue<NetMessage> m_messageQueue = new SafeQueue<NetMessage>();

        public int MessageCount
        { get { return m_messageQueue.Count; } }

        public int MaxMessageCount
        { get; } = 4096;

        protected byte[] m_key = null;

        protected List<bool> m_receivedSeqFlags = new List<bool>();
        protected int m_resetIndexForReceivedSeqFlags = -1;

        protected List<int> m_receivedSeqList = new List<int>();
        protected DateTime m_latestReceiveTime = DateTime.MinValue;

        //#####################################################################################
        // 작업 쓰레드

        protected Thread m_receiverThread = null;
        protected bool m_receiverRun = false;

        public bool IsWorking
        { get { return m_receiverRun; } }

        //#####################################################################################
        // 이벤트

        public event Action WhenDisconnected = (() => { });

        //#####################################################################################
        // 쓰레드 작업 함수

        protected void SendReceivedSeq()
        {
            // 정상적으로 수신했음을 알림
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(m_receivedSeqList.Count);

            foreach (int seq in m_receivedSeqList)
            {
                writer.WriteData(seq);
            }

            m_receivedSeqList.Clear();

            this.Sender.SendMessage(writer.CreateMessage((int)NetProtocols.CheckPacket));
        }

        protected bool WaitDataAndDoJob(Socket socket)
        {
            try
            {
                while (socket.Available <= 0)
                {
                    if (m_receiverRun == false)
                        return false;


                    if (m_receivedSeqList.Count > 0)
                    {
                        // 정상수신 알림 목록의 첫번째 메세지를 받은 시간에서 일정시간 경과했다면
                        var timespan = DateTime.Now - m_latestReceiveTime;
                        if (timespan.TotalSeconds > 3.0)
                        {
                            // 정상수신 알림
                            this.SendReceivedSeq();
                        }
                    }


                    Thread.Sleep(TimeSpan.Zero);
                }


                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
#if !DEBUG
            catch
            {
                return false;
            }
#endif
        }

        protected void ReceiverThreadJob()
        {
            try
            {
                while (m_receiverRun)
                {
                    // 데이터가 들어올때까지 대기하다가 종료플래그가 서면 종료
                    if (WaitDataAndDoJob(m_socket) == false)
                        break;


                    int readResult = 0;

                    // 헤더 읽고 생성
                    byte[] headerBytes = new byte[NetMessageHeader.ByteSize];

                    int recvSize = 0;

                    while (recvSize < headerBytes.Length)
                    {
                        try
                        {
                            readResult = m_socket.Receive(headerBytes, recvSize,
                                headerBytes.Length - recvSize, SocketFlags.None);
                        }
                        catch
                        { }

                        // 읽기 실패
                        if (readResult <= 0)
                        {
                            break;
                        }
                        else
                        {
                            recvSize += readResult;
                        }
                    }

                    NetMessageHeader header = new NetMessageHeader(headerBytes);


                    // 헤더가 유효하지 않으면
                    if (header.IsValid == false)
                    {
                        // 남은 데이터를 모두 버림.
                        while (m_socket.Available > 0)
                        {
                            byte[] tempBuffer = new byte[m_socket.Available];
                            m_socket.Receive(tempBuffer, 0, tempBuffer.Length, SocketFlags.None);
                        }
                    }
                    else
                    {
                        // 읽어야할 바디 크기가 있으면
                        if (header.BodySize > 0)
                        {
                            // 데이터가 들어올때까지 대기하다가 종료플래그가 서면 종료
                            if (WaitDataAndDoJob(m_socket) == false)
                                break;


                            readResult = 0;

                            // 바디 읽고 생성
                            byte[] bodyBytes = new byte[header.BodySize];

                            int recvBodySize = 0;

                            while (recvBodySize < bodyBytes.Length)
                            {
                                try
                                {
                                    readResult = m_socket.Receive(bodyBytes, recvBodySize,
                                        bodyBytes.Length - recvBodySize, SocketFlags.None);
                                }
                                catch
                                { }

                                // 읽기 실패
                                if (readResult <= 0)
                                {
                                    break;
                                }
                                else
                                {
                                    recvBodySize += readResult;
                                }
                            }

                            // 암호화 키가 있으면 해독
                            NetMessageBody body;
                            if (m_key != null)
                                body = new NetMessageBody(bodyBytes, m_key);
                            else
                                body = new NetMessageBody(bodyBytes);


                            // 메세지 생성
                            NetMessage msg = new NetMessage(header, body);

                            // 메세지가 유효하면
                            if (msg.IsValid)
                            {
                                // 수신 여부 확인용 목록 확보
                                while (header.SequenceNumber >= m_receivedSeqFlags.Count)
                                    m_receivedSeqFlags.Add(false);

                                // 리스트에서 수신 여부를 리셋할 위치를 이동
                                ++m_resetIndexForReceivedSeqFlags;

                                // 범위를 벗어났으면 처음 위치로 이동
                                if (m_resetIndexForReceivedSeqFlags >= this.Sender.MaxBackupSequence)
                                {
                                    m_resetIndexForReceivedSeqFlags = 0;
                                }

                                // 리셋할 위치값이 유효하면 리셋
                                if (m_resetIndexForReceivedSeqFlags >= 0
                                    && m_resetIndexForReceivedSeqFlags < m_receivedSeqFlags.Count)
                                {
                                    m_receivedSeqFlags[m_resetIndexForReceivedSeqFlags] = false;
                                }


                                // 정상수신 응답 메세지이면
                                if (header.MessageNumber == (int)NetProtocols.CheckPacket)
                                {
                                    // 정상적으로 수신되었음을 알림.
                                    NetMessageStream reader = new NetMessageStream(msg);
                                    int seqCount = reader.ReadInt32();

                                    for (int i = 0; i < seqCount; ++i)
                                    {
                                        int seqNum = reader.ReadInt32();
                                        this.Sender.ReceiveNormally(seqNum);
                                    }
                                }
                                else
                                {
                                    // 정상수신 응답 메세지가 아니면

                                    // 정상수신 알림 목록의 첫번째에 들어가는 메세지의 받은 시간을 기록
                                    if (m_receivedSeqList.Count <= 0)
                                    {
                                        m_latestReceiveTime = DateTime.Now;
                                    }

                                    // 정상적으로 수신했음을 알릴 예정으로 목록에 추가
                                    m_receivedSeqList.Add(header.SequenceNumber);

                                    // 정상수신 알림 목록이 일정크기 이상이 되었으면
                                    if (m_receivedSeqList.Count > 16)
                                    {
                                        // 정상수신 알림
                                        this.SendReceivedSeq();
                                    }


                                    // 이전에 받은적이 없는 시퀀스의 메세지이면
                                    if (m_receivedSeqFlags[header.SequenceNumber] == false)
                                    {
                                        // 받은걸로 표시
                                        m_receivedSeqFlags[header.SequenceNumber] = true;


                                        // 키 갱신 메세지이면
                                        if (header.MessageNumber == (int)NetProtocols.ResetKey)
                                        {
                                            // 키 갱신
                                            NetMessageStream reader = new NetMessageStream(msg);
                                            var keyStr = reader.ReadString();

                                            m_key = Enumerable.Range(0, keyStr.Length)
                                                .Where(x => x % 2 == 0)
                                                .Select(x => Convert.ToByte(keyStr.Substring(x, 2), 16))
                                                .ToArray();
                                        }
                                        // 죽으라는 메세지이면
                                        else if (header.MessageNumber == (int)NetProtocols.Die)
                                        {
                                            // 비정상 종료
                                            break;
                                        }
                                        // 일반 메세지이면
                                        else if (header.MessageNumber >= 0)
                                        {
                                            // 버퍼에 여유가 있을때까지 대기한뒤
                                            while (m_messageQueue.Count >= this.MaxMessageCount)
                                            {
                                                Thread.Sleep(16);
                                            }

                                            // 수신목록에 추가
                                            m_messageQueue.Push(msg);
                                        }
                                    }
                                }
                            }
                        }
                    }


                    Thread.Sleep(4);
                }
            }
            catch (IOException)
            { }
            catch (SocketException)
            { }
            catch (ThreadAbortException)
            { }
            catch (ObjectDisposedException)
            {
                m_receiverRun = false;
            }


            // 비정상적인 종료이면
            if (m_receiverRun)
            {
                // 이벤트 발생
                WhenDisconnected();
            }

            m_receiverRun = false;
        }

        //#####################################################################################
        // 수신자

        public void Start(Socket socket)
        {
            // 이전 스트림 해제
            Stop();


            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 30000);

            m_socket = socket;


            // 작업 쓰레드 생성
            m_receiverRun = true;

            m_receiverThread = new Thread(this.ReceiverThreadJob);
            m_receiverThread.Start();
        }

        public void Stop()
        {
            // 작업 쓰레드 중지
            if (m_receiverThread != null)
            {
                m_receiverRun = false;

                m_receiverThread.Join(TimeSpan.FromSeconds(10.0));

                if (m_receiverThread.ThreadState == ThreadState.WaitSleepJoin
                    || m_receiverThread.ThreadState == ThreadState.Running)
                {
                    m_receiverThread.Abort();
                }

                m_receiverThread = null;
            }


            m_messageQueue.Clear();

            m_key = null;

            m_receivedSeqFlags.Clear();
            m_resetIndexForReceivedSeqFlags = -this.Sender.MaxBackupSequence / 2;


            // 리셋
            m_socket = null;
        }

        //#####################################################################################

        public NetMessage ReceiveMessage()
        {
            return m_messageQueue.Pop();
        }
    }
}
