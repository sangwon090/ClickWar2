using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickWar2.Network;
using ClickWar2.Network.IO;
using ClickWar2.Network.Protocol;

namespace ClickWar2.Game.Network.ServerWorker
{
    public class NoticeManager
    {
        public NoticeManager()
        {

        }

        //#####################################################################################

        public Action<NetMessage> NoticeDelegate
        { get; set; } = null;

        protected string m_notice = "";
        public string Notice
        {
            get { return m_notice; }
            set
            {
                m_notice = value;

                if (value.Length > 0)
                {
                    // 모든 클라에게 공지 알림
                    if (NoticeDelegate != null)
                    {
                        NetMessageStream writer = new NetMessageStream();
                        writer.WriteData(value);

                        NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_Notice));
                    }
                }
            }
        }

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetServerProcedure procList)
        {
            procList.Set(this.WhenReqNotice, (int)MessageTypes.Req_Notice);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private NetMessage WhenReqNotice(ServerVisitor client, NetMessageStream msg)
        {
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.Notice);

            return writer.CreateMessage((int)MessageTypes.Ntf_Notice);
        }

        //#####################################################################################

        public void CheckUser()
        {
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(Utility.Random.Next(9));

            this.NoticeDelegate(writer.CreateMessage((int)MessageTypes.Ntf_CheckUser));
        }
    }
}
