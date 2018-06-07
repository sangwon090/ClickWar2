using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ClickWar2.Network;
using ClickWar2.Network.IO;
using ClickWar2.Game.Event;

namespace ClickWar2.Game.Network.ClientWorker
{
    public class CommunicationManager
    {
        public CommunicationManager()
        {

        }

        //#####################################################################################

        public NetClient Client
        { get; set; }

        public SignManager SignDirector
        { get; set; }

        protected List<Mail> m_mailbox = new List<Mail>();
        public List<Mail> Mailbox
        { get { return m_mailbox; } }

        public int MaxMailLength
        { get; set; } = 1024;

        //#####################################################################################
        // 이벤트 관리자

        public UserEventList EventDirector
        { get; set; } = new UserEventList();

        //#####################################################################################
        // 메세지 수신 콜백

        protected Action<Mail> m_receiveMailCallback = null;

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetClientProcedure procList)
        {
            procList.Set(this.WhenNtfReceiveMail, (int)MessageTypes.Ntf_ReceiveMail);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private void WhenNtfReceiveMail(NetMessageStream msg)
        {
            int readFlag = msg.ReadInt32();
            string fromUser = msg.ReadString();
            string toUser = msg.ReadString();
            string sendingDate = msg.ReadString();
            string message = msg.ReadString();


            // 수신함에 추가
            Mail mail = new Mail()
            {
                Read = (readFlag != 0),
                From = fromUser,
                To = toUser,
                SendingDate = sendingDate,
                Message = message,
            };

            m_mailbox.Insert(0, mail);


            if (m_receiveMailCallback != null)
            {
                m_receiveMailCallback(mail);
            }


            // 이벤트 발생
            this.EventDirector.StartEvent((int)CommunicationEvents.ReceiveMail,
                fromUser, new object[] { mail });
        }

        //#####################################################################################
        // 사용자 입력 처리

        public void SendMailTo(string targetUserName, string message)
        {
            // 길이 제한
            if (message.Length > this.MaxMailLength)
                message = message.Remove(this.MaxMailLength);


            // 해당 유저에게 메일 전송 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);
            writer.WriteData(targetUserName);
            writer.WriteData(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            writer.WriteData(message.Replace("\r\n", "\n"));

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_SendMail));
        }

        public void RequestMailbox(Action<Mail> callbackAsync)
        {
            m_receiveMailCallback = callbackAsync;


            // 수신함의 모든 메일 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_Mailbox));
        }

        public void ReadMail(int index)
        {
            // 메일 읽음처리 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);
            writer.WriteData(index);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_ReadMail));
        }
    }
}
