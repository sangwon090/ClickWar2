using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ClickWar2.Network.Protocol;

namespace ClickWar2.Network.IO
{
    public class NetMessageStream
    {
        public NetMessageStream()
        {
            m_msg = new NetMessage();
        }

        public NetMessageStream(NetMessage msg)
        {
            m_msg = msg;
        }

        public NetMessageStream(NetMessageBody msgBody)
        {
            m_msg = new NetMessage(new NetMessageHeader(), msgBody);
        }

        //#####################################################################################

        protected NetMessage m_msg = null;

        //#####################################################################################

        public void SetMessage(NetMessage msg)
        {
            m_msg = msg;
        }

        public void SetMessage(NetMessageBody msgBody)
        {
            m_msg = new NetMessage(new NetMessageHeader(), msgBody);
        }

        //#####################################################################################

        public NetMessage CreateMessage(int messageNumber)
        {
            this.MessageNumber = messageNumber;


            return new NetMessage(m_msg.Header, m_msg.Body);
        }

        public NetMessage CreateMessage()
        {
            return new NetMessage(m_msg.Header, m_msg.Body);
        }

        //#####################################################################################

        public int MessageNumber
        {
            get { return m_msg.Header.MessageNumber; }
            set { m_msg.Header.MessageNumber = value; }
        }

        public int ReadInt32()
        {
            var data = BitConverter.ToInt32(m_msg.Body.Data.Take(sizeof(Int32)).ToArray(), 0);
            m_msg.Body.Data.RemoveRange(0, sizeof(Int32));
            return data;
        }

        public string ReadString()
        {
            int length = ReadInt32();
            var data = Encoding.UTF8.GetString(m_msg.Body.Data.Take(length).ToArray());
            m_msg.Body.Data.RemoveRange(0, length);
            return data;
        }

        public void WriteData(Int32 data) => m_msg.Body.Data.AddRange(BitConverter.GetBytes(data));

        public void WriteData(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            WriteData(bytes.Length);
            m_msg.Body.Data.AddRange(bytes);
        }

        //#####################################################################################

        public bool EndOfStream
        { get { return (m_msg.Body.Data.Count <= 0); } }
    }
}
