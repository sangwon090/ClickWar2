using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ClickWar2.Network.Protocol;

namespace ClickWar2.Network
{
    public class NetClient
    {
        public NetClient()
        {
            m_sender = new IO.NetSender();
            m_receiver = new IO.NetReceiver(m_sender);


            // 이벤트 연결
            m_receiver.WhenDisconnected += DisconnectedEventTrigger;
        }

        ~NetClient()
        {
            m_receiver.WhenDisconnected -= DisconnectedEventTrigger;

            this.Disconnect();
        }

        //#####################################################################################
        // 클라이언트

        protected TcpClient m_client = null;

        //#####################################################################################
        // 연결확인 쓰레드

        protected Thread m_checkConnectionThread = null;
        protected bool m_runCheckConnection = false;

        //#####################################################################################
        // 클라이언트 정보

        public bool IsValid
        { get { return (m_client != null && m_sender.IsWorking && m_receiver.IsWorking && m_runCheckConnection); } }

        //#####################################################################################
        // 메세지 수신/송신자

        protected IO.NetReceiver m_receiver = null;
        protected IO.NetSender m_sender = null;

        public int ReceiveBufferSize
        { get { return m_receiver.MessageCount; } }

        public int SendBufferSize
        { get { return m_sender.MessageCount; } }

        //#####################################################################################
        // 이벤트
        
        public event Action WhenDisconnected = (() => { });
        protected void DisconnectedEventTrigger()
        {
            this.Disconnect();

            WhenDisconnected();
        }

        //#####################################################################################

        public void Connect(string address, string port)
        {
            // 이전연결 해제
            Disconnect();


            // 연결
            m_client = new TcpClient();

            try
            {
                m_client.Connect(address, int.Parse(port));
            }
            catch (SocketException)
            {
                m_client = null;
                return;
            }


            m_runCheckConnection = true;
            m_checkConnectionThread = new Thread(this.CheckConnectionJob);
            m_checkConnectionThread.Start();


            // 메세지 수신/송신 준비
            m_receiver.Start(m_client.Client);
            m_sender.Start(m_client.Client);
        }

        public void Disconnect()
        {
            m_runCheckConnection = false;
            if (m_checkConnectionThread != null)
            {
                m_checkConnectionThread.Join();
                m_checkConnectionThread = null;
            }

            // 메세지 수신/송신 중단
            m_receiver.Stop();
            m_sender.Stop();

            // 접속 해제
            if (m_client != null)
            {
                m_client.Close();
                m_client = null;
            }
        }

        //#####################################################################################
        // 쓰레드 작업

        protected void CheckConnectionJob()
        {
            while (m_runCheckConnection)
            {
                if (Utility.IsConnected(m_client.Client) == false)
                {
                    this.WhenDisconnected();

                    break;
                }


                Thread.Sleep(5000);
            }


            m_runCheckConnection = false;
        }

        //#####################################################################################
        // 메세지 처리

        public NetMessage ReceiveMessage()
        {
            return m_receiver.ReceiveMessage();
        }

        public void ReceiveMessageInto(NetClientProcedure procList)
        {
            var msg = m_receiver.ReceiveMessage();
            if (msg != null)
            {
                procList.Run(msg.Header.MessageNumber, new IO.NetMessageStream(msg));
            }
        }

        public void SendMessage(NetMessage message)
        {
            m_sender.SendMessage(message);
        }
    }
}
