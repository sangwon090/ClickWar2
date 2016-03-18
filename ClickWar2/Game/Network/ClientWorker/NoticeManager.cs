using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickWar2.Network;
using ClickWar2.Network.IO;

namespace ClickWar2.Game.Network.ClientWorker
{
    public class NoticeManager
    {
        public NoticeManager()
        {

        }

        //#####################################################################################

        public NetClient Client
        { get; set; }

        public string LatestNotice
        { get; set; } = "";

        //#####################################################################################
        // 메세지 수신 콜백

        protected Action<string> m_noticeCallback = null;

        public Action<int> WhenCheckUser;

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetClientProcedure procList)
        {
            procList.Set(this.WhenNtfNotice, (int)MessageTypes.Ntf_Notice);
            procList.Set(this.WhenNtfCheckUser, (int)MessageTypes.Ntf_CheckUser);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private void WhenNtfNotice(NetMessageStream msg)
        {
            // 공지 갱신
            this.LatestNotice = msg.ReadData<string>();

            // 공지 갱신 알림
            if (m_noticeCallback != null)
            {
                m_noticeCallback(this.LatestNotice);
                m_noticeCallback = null;
            }
        }

        private void WhenNtfCheckUser(NetMessageStream msg)
        {
            int data = msg.ReadData<int>();


            this.WhenCheckUser(data);
        }

        //#####################################################################################
        // 사용자 입력 처리

        public void UpdateNotice(Action<string> callbackAsync = null)
        {
            m_noticeCallback = callbackAsync;


            // 공지 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData("");

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_Notice));
        }
    }
}
