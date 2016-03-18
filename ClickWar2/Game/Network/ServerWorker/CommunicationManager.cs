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
    public class CommunicationManager
    {
        public CommunicationManager()
        {

        }

        //#####################################################################################

        public NetServer Server
        { get; set; }

        public UserManager UserDirector
        { get; set; }

        public int MaxMailboxSize
        { get; set; } = 32;

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetServerProcedure procList)
        {
            procList.Set(this.WhenReqSendMail, (int)MessageTypes.Req_SendMail);
            procList.Set(this.WhenReqMailbox, (int)MessageTypes.Req_Mailbox);
            procList.Set(this.WhenReqReadMail, (int)MessageTypes.Req_ReadMail);
        }

        //#####################################################################################

        protected void NtfReceiveMailTo(ServerVisitor targetClient, Mail mail)
        {
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData<int>(mail.Read ? 1 : 0);
            writer.WriteData(mail.From);
            writer.WriteData(mail.To);
            writer.WriteData(mail.SendingDate);
            writer.WriteData(mail.Message);

            targetClient.Sender.SendMessage(writer.CreateMessage((int)MessageTypes.Ntf_ReceiveMail));
        }

        protected void NtfReceiveMailTo(string targetName, Mail mail)
        {
            int id = this.UserDirector.GetLoginClientID(targetName);
            if (id >= 0)
            {
                var targetClient = this.Server.GetClientByID(id);
                if (targetClient != null)
                {
                    this.NtfReceiveMailTo(targetClient, mail);
                }
            }
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private NetMessage WhenReqSendMail(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            string targetName = msg.ReadData<string>();
            string date = msg.ReadData<string>();
            string message = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                var targetUser = this.UserDirector.GetAccount(targetName);

                if (targetUser != null)
                {
                    Mail mail = new Mail()
                    {
                        Read = false,
                        From = userName,
                        To = targetName,
                        SendingDate = date,
                        Message = message,
                    };

                    // 메일함에 추가
                    targetUser.Mailbox.Insert(0, mail);

                    // 메일함의 크기가 너무 크면 오래된 메일부터 지움.
                    if (targetUser.Mailbox.Count > this.MaxMailboxSize)
                    {
                        targetUser.Mailbox.RemoveRange(this.MaxMailboxSize, targetUser.Mailbox.Count - this.MaxMailboxSize);
                    }


                    // 메일 수신 알림
                    this.NtfReceiveMailTo(targetName, mail);
                }
            }


            return null;
        }

        private NetMessage WhenReqMailbox(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);

                if (user != null)
                {
                    var mailbox = user.Mailbox;

                    for (int i = mailbox.Count - 1; i >= 0; --i)
                    {
                        this.NtfReceiveMailTo(client, mailbox[i]);
                    }
                }
            }


            return null;
        }

        private NetMessage WhenReqReadMail(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();
            int index = msg.ReadData<int>();


            // 인증
            var user = this.UserDirector.GetLoginUser(client.ID);
            if (user != null && user.Name == userName)
            {
                user = this.UserDirector.GetAccount(user.Name);

                if (user != null)
                {
                    var mailbox = user.Mailbox;

                    if (index >= 0 && index < mailbox.Count)
                    {
                        mailbox[index].Read = true;
                    }
                }
            }


            return null;
        }
    }
}
