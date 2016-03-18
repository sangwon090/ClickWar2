using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string Token
        { get; set; } = "\r\n";

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

        public T ReadData<T>()
        {
            StringBuilder stream = m_msg.Body.Data;
            StringBuilder item = new StringBuilder();

            if (stream.Length < 2)
                throw new System.IO.EndOfStreamException("메세지에 더이상 읽을 데이터가 없습니다.");

            while (stream.Length >= this.Token.Length)
            {
                bool bFindToken = true;

                for (int i = 0; i < this.Token.Length; ++i)
                {
                    if (stream[i] != this.Token[i])
                    {
                        bFindToken = false;
                        break;
                    }
                }

                if (bFindToken)
                {
                    stream.Remove(0, this.Token.Length);

                    break;
                }
                else
                {
                    item.Append(stream[0]);

                    stream.Remove(0, 1);
                }
            }


            return (T)Convert.ChangeType(item.ToString(), typeof(T));
        }

        public void WriteData<T>(T data)
        {
            StringBuilder stream = m_msg.Body.Data;

            stream.Append(data);
            stream.Append(this.Token);
        }

        //#####################################################################################

        public bool EndOfStream
        { get { return (m_msg.Body.Data.Length <= 0); } }
    }
}
