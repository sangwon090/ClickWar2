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
    public class NetSender
    {
        public NetSender()
        {

        }

        ~NetSender()
        {
            Stop();
        }

        //#####################################################################################
        // 전송자 정보

        protected Socket m_socket;

        //#####################################################################################
        // 메세지

        protected SafeQueue<NetMessage> m_messageQueue = new SafeQueue<NetMessage>();

        public int MessageCount
        { get { return m_messageQueue.Count; } }

        public int MaxMessageCount
        { get; } = 4096;

        protected byte[] m_key;

        //#####################################################################################
        // 메세지 백업

        protected int m_sequenceNumber = 0;

        public int MaxBackupSequence
        { get; } = 4096;

        protected SafeList<KeyValuePair<NetMessage, DateTime>> m_backupMsgList = new SafeList<KeyValuePair<NetMessage, DateTime>>();

        protected int m_backupCheckIndex = 0;

        public double MaxWaitBackupTime
        { get; set; } = 10.0;

        public int MaxRetryCount
        { get; set; } = 3;

        //#####################################################################################
        // 작업 쓰레드

        protected Thread m_senderThread = null;
        protected bool m_senderRun = false;

        protected Thread m_backupCheckThread = null;

        public bool IsWorking
        { get { return m_senderRun; } }

        //#####################################################################################
        // 쓰레드 작업 함수

        protected void SenderThreadJob()
        {
            try
            {
                while (m_senderRun)
                {
                    Thread.Sleep(2);


                    // 메세지가 있고 실행 플래그가 서있으면
                    while (!m_messageQueue.IsEmpty && m_senderRun)
                    {
                        // 하나 뽑아내서
                        var messageToSend = m_messageQueue.Pop();

                        // 있으면
                        if (messageToSend != null)
                        {
                            int currentSeq = m_sequenceNumber;

                            // 처음 보내는 메세지가 아니면 시퀀스를 이전 메세지의 시퀀스로 설정
                            if (messageToSend.RetryCount > 0)
                                currentSeq = messageToSend.Header.SequenceNumber;

                            // 시퀀스 번호 설정하고
                            messageToSend.Header.SequenceNumber = currentSeq;

                            // 암호화된 데이터 얻음
                            var bytesToSend = messageToSend.GetEncryptedBytes(m_key);

                            if (bytesToSend != null)
                            {
                                // 백업 여유공간 확보
                                m_backupMsgList.DoSync((list) =>
                                {
                                    while (currentSeq >= list.Count)
                                        list.Add(new KeyValuePair<NetMessage, DateTime>(null, DateTime.MinValue));
                                });


                                // 정상수신 응답 메세지가 아니면
                                if (messageToSend.Header.MessageNumber != (int)NetProtocols.CheckPacket)
                                {
                                    // 백업
                                    m_backupMsgList.DoSync((list) =>
                                    {
                                        list[currentSeq] = new KeyValuePair<NetMessage, DateTime>(messageToSend,
                                            DateTime.Now);
                                    });
                                }


                                // 메세지 전송
                                try
                                {
                                    m_socket.Send(bytesToSend, 0, bytesToSend.Length, SocketFlags.None);
                                }
                                catch
                                {
                                    m_senderRun = false;
                                    break;
                                }


                                // 처음 보내는 메세지이면
                                if (messageToSend.RetryCount <= 0)
                                {
                                    // 시퀀스 번호 상승
                                    ++m_sequenceNumber;

                                    if (m_sequenceNumber > this.MaxBackupSequence)
                                        m_sequenceNumber = 0;
                                }
                            }
                        }


                        Thread.Sleep(TimeSpan.Zero);
                    }
                }
            }
            catch (IOException)
            {
                m_senderRun = false;
            }
            catch (ThreadAbortException)
            {
                m_senderRun = false;
            }
            catch(ObjectDisposedException)
            {
                m_senderRun = false;
            }
#if !DEBUG
            catch
            {
                m_senderRun = false;
            }
#endif
        }

        protected void BackupCheckThreadJob()
        {
            try
            {
                var nullBackup = new KeyValuePair<NetMessage, DateTime>(null, DateTime.MinValue);


                while (m_senderRun)
                {
                    Thread.Sleep(2);


                    // 수신 응답을 일정시간 이상 받지못한 메세지가 있는지 확인

                    int endIndex = m_backupCheckIndex + this.MaxBackupSequence / 1024;
                    if (endIndex > m_backupMsgList.Count)
                        endIndex = m_backupMsgList.Count;

                    for (int i = m_backupCheckIndex; i < endIndex && m_senderRun; ++i)
                    {
                        var msg_time = m_backupMsgList[i];
                        var msg = msg_time.Key;


                        if (msg != null)
                        {
                            var span = DateTime.Now - msg_time.Value;

                            // 일정시간 경과했고
                            if (span.TotalSeconds >= this.MaxWaitBackupTime)
                            {
                                // 재시도횟수가 남아있으면
                                if (msg.RetryCount < this.MaxRetryCount)
                                {
                                    // 다시 보냄
                                    ++msg.RetryCount;
                                    m_messageQueue.Push(msg);
                                }

                                // 목록에서 제거
                                m_backupMsgList[i] = nullBackup;
                            }
                        }


                        ++m_backupCheckIndex;


                        Thread.Sleep(TimeSpan.Zero);
                    }

                    if (m_backupCheckIndex >= m_backupMsgList.Count)
                        m_backupCheckIndex = 0;
                }
            }
            catch (IOException)
            {
                m_senderRun = false;
            }
            catch (ThreadAbortException)
            {
                m_senderRun = false;
            }
#if !DEBUG
            catch
            {
                m_senderRun = false;
            }
#endif
        }

        //#####################################################################################
        // 전송자

        public void Start(Socket socket)
        {
            // 이전 스트림 해제
            Stop();


            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 30000);
            Utility.InitializeSocketKeepAlive(socket, (UInt32)TimeSpan.FromSeconds(10.0).TotalMilliseconds);

            m_socket = socket;


            // 암호화 키 생성 후 전송
            ResetKey();


            // 작업 쓰레드 생성 및 시작
            m_senderRun = true;

            m_senderThread = new Thread(this.SenderThreadJob);
            m_senderThread.Start();

            m_backupCheckThread = new Thread(this.BackupCheckThreadJob);
            m_backupCheckThread.Start();
        }

        protected void StopThread(Thread thread)
        {
            thread.Join(TimeSpan.FromSeconds(10.0));

            if (thread.ThreadState == ThreadState.WaitSleepJoin
                || thread.ThreadState == ThreadState.Running)
            {
                thread.Abort();
            }
        }

        public void Stop()
        {
            m_messageQueue.Clear();


            // 작업 쓰레드 중지
            m_senderRun = false;

            if (m_senderThread != null)
            {
                this.StopThread(m_senderThread);
                m_senderThread = null;
            }

            if (m_backupCheckThread != null)
            {
                this.StopThread(m_backupCheckThread);
                m_backupCheckThread = null;
            }


            // 리셋
            m_socket = null;
        }

        //#####################################################################################

        public void SendMessage(NetMessage msg)
        {
            // 버퍼에 여유가 있을때까지 대기한뒤
            while (m_messageQueue.Count >= this.MaxMessageCount)
            {
                Thread.Sleep(32);
            }

            // 송신버퍼에 추가
            m_messageQueue.Push(msg);
        }

        protected void ResetKey()
        {
            m_key = Security.GenerateKey();

            NetMessageStream maker = new NetMessageStream();
            maker.WriteData(BitConverter.ToString(m_key).Replace("-", ""));

            var keyMsg = maker.CreateMessage((int)NetProtocols.ResetKey);
            keyMsg.Header.SequenceNumber = m_sequenceNumber++;
            var keyMsgBytes = keyMsg.GetBytes();
            m_socket.Send(keyMsgBytes, 0, keyMsgBytes.Length, SocketFlags.None);
        }

        public void ReceiveNormally(int sequenceNumber)
        {
            m_backupMsgList.DoSync((list) =>
            {
                if (sequenceNumber < list.Count
                    && list[sequenceNumber].Key != null)
                {
                    // 정상적으로 받았다고 하므로 백업 목록에서 제거
                    list[sequenceNumber] = new KeyValuePair<NetMessage, DateTime>(null,
                        DateTime.MinValue);
                }
            });
        }
    }
}
