using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ClickWar2.Utility;
using ClickWar2.Network.Protocol;

namespace ClickWar2.Network
{
    public class NetServer
    {
        public NetServer()
        {
            
        }

        ~NetServer()
        {
            this.Stop();
        }

        //#####################################################################################
        // 서버

        protected TcpListener m_listener = null;

        //#####################################################################################
        // 작업 쓰레드

        protected Thread m_acceptThread = null;
        protected bool m_acceptRun = false;

        protected Thread m_checkThread = null;
        protected bool m_checkRun = false;

        //#####################################################################################
        // 클라이언트 관리

        protected SafeList<ServerVisitor> m_clientList = new SafeList<ServerVisitor>();

        public ServerVisitor[] Clients
        { get { return m_clientList.GetArray(); } }

        //#####################################################################################
        // 서버 정보

        public Socket Socket
        { get { return m_listener.Server; } }

        public int ConnectionCount
        { get { return m_clientList.Count; } }

        public bool IsValid
        { get { return (m_listener != null && m_acceptRun && m_checkRun); } }

        //#####################################################################################
        // 이벤트

        public event Action<ServerVisitor> WhenConnected = ((clt) => { });
        public event Action<ServerVisitor> WhenDisconnected = ((clt) => { });

        //#####################################################################################
        // 서버

        public void Start(string port)
        {
            // 기존연결 닫기
            Stop();


            // TCP 서버 생성 및 시작
            try
            {
                m_listener = new TcpListener(IPAddress.Any, int.Parse(port));
                m_listener.Start();
            }
            catch (SocketException)
            {
                m_listener = null;
                return;
            }


            // 클라이언트 접속처리용 쓰레드 생성 및 시작
            m_acceptRun = true;

            m_acceptThread = new Thread(this.AcceptThreadJob);
            m_acceptThread.Start();


            // 클라이언트 연결확인용 쓰레드 생성 및 시작
            m_checkRun = true;

            m_checkThread = new Thread(this.CheckThreadJob);
            m_checkThread.Start();
        }

        public void Stop()
        {
            // 접속처리용 쓰레드 중지
            if (m_acceptThread != null)
            {
                m_acceptRun = false;

                m_acceptThread.Join();
                m_acceptThread = null;
            }


            // 연결확인용 쓰레드 중지
            if (m_checkThread != null)
            {
                m_checkRun = false;

                m_checkThread.Join();
                m_checkThread = null;
            }


            // 연결된 클라이언트 해제
            foreach (var client in m_clientList)
            {
                client.Clear();
            }


            // 서버 닫기
            if (m_listener != null)
            {
                m_listener.Stop();
                m_listener = null;
            }
        }

        //#####################################################################################
        // 작업 쓰레드용 함수

        protected void AcceptThreadJob()
        {
            while (m_acceptRun)
            {
                // 보류중인 접속이 없으면 대기
                while (!m_listener.Pending())
                {
                    if (m_acceptRun == false)
                        return;

                    Thread.Sleep(128);
                }

                // 접속확인
                Socket newClient = m_listener.AcceptSocket();

                // 클라이언트 객체 생성
                if (newClient != null)
                {
                    ServerVisitor client = new ServerVisitor(newClient);

                    m_clientList.Add(client);


                    // 이벤트 발생
                    WhenConnected(client);
                }
            }
        }

        protected void CheckThreadJob()
        {
            while (m_checkRun)
            {
                for (int i = 0; i < m_clientList.Count; ++i)
                {
                    ServerVisitor client = m_clientList[i];

                    // 클라이언트 접속이 유효하지 않으면
                    if (client.IsValid == false
                        || Utility.IsConnected(client.Socket) == false)
                    {
                        this.RemoveClientAt(i);
                        --i;
                    }
                }


                Thread.Sleep(3000);
            }
        }

        //#####################################################################################
        // 메세지 처리

        public KeyValuePair<ServerVisitor, NetMessage>[] ReceiveMessageFromAll()
        {
            if (m_clientList.Count > 0)
            {
                var result = new KeyValuePair<ServerVisitor, NetMessage>[m_clientList.Count];

                for (int i = 0; i < result.Length; ++i)
                {
                    ServerVisitor client = m_clientList[i];

                    result[i] = new KeyValuePair<ServerVisitor, NetMessage>(client,
                        client.Receiver.ReceiveMessage());
                }


                return result;
            }


            return null;
        }

        public void SendMessageToAll(NetMessage message)
        {
            for (int i = 0; i < m_clientList.Count; ++i)
            {
                ServerVisitor client = m_clientList[i];

                if (client != null)
                {
                    client.Sender.SendMessage(message.Clone());
                }
            }
        }

        public void ReceiveMessageInto(NetServerProcedure procList)
        {
            for (int i = 0; i < m_clientList.Count; ++i)
            {
                ServerVisitor client = m_clientList[i];

                var msg = client.Receiver.ReceiveMessage();
                if (msg != null)
                {
                    procList.Run(msg.Header.MessageNumber, client, new IO.NetMessageStream(msg));
                }
            }
        }

        //#####################################################################################
        // 접속자

        public ServerVisitor GetClientByID(int id)
        {
            for (int i = 0; i < m_clientList.Count; ++i)
            {
                ServerVisitor client = m_clientList[i];

                if (client.ID == id)
                {
                    return client;
                }
            }


            return null;
        }

        public void RemoveClientAt(int index)
        {
            var client = m_clientList[index];


            // 이벤트 발생
            WhenDisconnected(client);


            // 접속을 해제하고
            client.Clear();

            // 클라이언트 목록에서 제거한다.
            m_clientList.RemoveAt(index);
        }

        public void DisconnectClient(int id)
        {
            for (int i = 0; i < m_clientList.Count; ++i)
            {
                ServerVisitor client = m_clientList[i];

                if (client.ID == id)
                {
                    this.RemoveClientAt(i);
                    break;
                }
            }
        }
    }
}
