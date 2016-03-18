using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickWar2.Network.Protocol;
using ClickWar2.Network.IO;

namespace ClickWar2.Network
{
    public class NetServerProcedure
    {
        public NetServerProcedure()
        {

        }

        //#####################################################################################

        public delegate NetMessage MessageProcedureDelegate(ServerVisitor client, NetMessageStream reader);

        protected List<MessageProcedureDelegate> m_procList = new List<MessageProcedureDelegate>();

        //#####################################################################################

        public void Set(MessageProcedureDelegate proc, int number)
        {
            if (number < 0)
                throw new ArgumentException("number는 0보다 작을 수 없습니다.");

            // 공간확보
            while (number >= m_procList.Count)
                m_procList.Add(null);

            m_procList[number] = proc;
        }

        public void Reset(int number)
        {
            if (number < 0)
                throw new ArgumentException("number는 0보다 작을 수 없습니다.");

            if (number < m_procList.Count)
            {
                m_procList[number] = null;
            }
        }

        //#####################################################################################

        public void Run(int number, ServerVisitor client, NetMessageStream reader)
        {
            if (number < 0)
                throw new ArgumentException("number는 0보다 작을 수 없습니다.");

            if (number < m_procList.Count
                && m_procList[number] != null)
            {
                try
                {
                    var responce = m_procList[number](client, reader);

                    if (responce != null)
                    {
                        client.Sender.SendMessage(responce);
                    }
                }
                catch (FormatException)
                {
                    // TODO: 패킷변조 경고
                }
                catch (System.IO.EndOfStreamException)
                {
                    // TODO: 패킷변조 경고
                }
#if !DEBUG
                catch (Exception)
                {
                    
                }
#endif
            }
        }
    }
}
