using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ClickWar2.Network
{
    public class ServerVisitor
    {
        public ServerVisitor(Socket sock)
        {
            m_sender = new IO.NetSender();
            m_receiver = new IO.NetReceiver(m_sender);


            m_socket = sock;

            m_receiver.Start(sock);
            m_sender.Start(sock);
        }

        ~ServerVisitor()
        {
            this.Clear();
        }

        //#####################################################################################

        protected Socket m_socket = null;

        public int ID
        { get { return m_socket.Handle.ToInt32(); } }

        //#####################################################################################

        protected IO.NetReceiver m_receiver = null;
        protected IO.NetSender m_sender = null;

        public IO.NetReceiver Receiver
        { get { return m_receiver; } }
        public IO.NetSender Sender
        { get { return m_sender; } }

        //#####################################################################################

        public void Clear()
        {
            m_receiver.Stop();
            m_sender.Stop();

            if (m_socket != null)
            {
                m_socket.Close();
                m_socket = null;
            }
        }

        //#####################################################################################

        public Socket Socket
        { get { return m_socket; } set { m_socket = value; } }

        public bool IsValid
        { get { return (m_socket != null && m_sender.IsWorking && m_receiver.IsWorking); } }
    }
}
