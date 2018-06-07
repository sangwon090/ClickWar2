using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ClickWar2.Network;
using ClickWar2.Network.IO;

namespace ClickWar2.Game.Network.ClientWorker
{
    public class SignManager
    {
        public SignManager()
        {
            
        }

        //#####################################################################################

        public NetClient Client
        { get; set; }

        public bool IsOnLogin
        { get; set; } = false;

        public string LoginName
        { get; set; } = "";
        protected string m_tryName = "";

        //#####################################################################################
        // 메세지 수신 콜백

        protected Action<LoginResults> m_loginCallback = null;
        protected Action<RegisterResults> m_registerCallback = null;

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetClientProcedure procList)
        {
            procList.Set(this.WhenRspLogin, (int)MessageTypes.Rsp_Login);
            procList.Set(this.WhenRspLogout, (int)MessageTypes.Rsp_Logout);
            procList.Set(this.WhenRspRegister, (int)MessageTypes.Rsp_Register);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private void WhenRspLogin(NetMessageStream msg)
        {
            LoginResults loginResult = (LoginResults)msg.ReadInt32();

            if (loginResult == LoginResults.Success)
            {
                this.LoginName = m_tryName;
            }
            
            // 로그인 성공여부 저장
            this.IsOnLogin = (loginResult == LoginResults.Success);

            // 로그인 콜백 호출
            if (m_loginCallback != null)
            {
                m_loginCallback(loginResult);
                m_loginCallback = null;
            }
        }

        private void WhenRspLogout(NetMessageStream msg)
        {
            this.IsOnLogin = false;
            this.LoginName = "";
        }

        private void WhenRspRegister(NetMessageStream msg)
        {
            RegisterResults result = (RegisterResults)msg.ReadInt32();

            // 계정등록 콜백 호출
            if (m_registerCallback != null)
            {
                m_registerCallback(result);
                m_registerCallback = null;
            }
        }

        //#####################################################################################
        // 사용자 입력 처리

        public void Login(string name, string password, Action<LoginResults> callbackAsync)
        {
            m_loginCallback = callbackAsync;

            m_tryName = name;


            // 로그인 요청 메세지 전송
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(name);
            writer.WriteData(password);
            writer.WriteData(Application.ProductVersion.ToString());

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_Login));
        }

        public void Register(string name, string password, Color userColor,
            Action<RegisterResults> callbackAsync)
        {
            m_registerCallback = callbackAsync;


            // 계정생성 요청 전송
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(name);
            writer.WriteData(password);
            writer.WriteData(userColor.ToArgb());

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_Register));
        }
    }
}
