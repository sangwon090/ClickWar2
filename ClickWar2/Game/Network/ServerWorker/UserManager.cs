using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using ClickWar2.Network;
using ClickWar2.Network.IO;
using ClickWar2.Network.Protocol;

namespace ClickWar2.Game.Network.ServerWorker
{
    public class UserManager
    {
        public UserManager()
        {

        }

        //#####################################################################################
        // 정보

        public NetServer Server
        { get; set; } = null;

        public Action<NetMessage> NoticeDelegate
        { get; set; } = null;

        protected string UserFileName
        { get; set; } = "Users.txt";

        public string ServerPath
        { get; set; }

        public int MaxNameLength
        { get; set; } = 32;

        public int MaxPasswordLength
        { get; set; } = 64;

        public bool CanLogin
        { get; set; } = false;

        //#####################################################################################
        // 접속자

        protected Dictionary<int, GamePlayer> m_loginUserList = new Dictionary<int, GamePlayer>();
        public int LoginUsersCount
        { get { return m_loginUserList.Count; } }
        public GamePlayer[] LoginUsers
        {
            get
            {
                List<GamePlayer> userList = new List<GamePlayer>();

                foreach (var userInfo in m_loginUserList)
                    userList.Add(userInfo.Value);

                return userList.ToArray();
            }
        }

        //#####################################################################################
        // 모든 계정

        protected Dictionary<string, GamePlayer> m_accountList = new Dictionary<string, GamePlayer>();
        public GamePlayer[] Accounts
        {
            get
            {
                List<GamePlayer> userList = new List<GamePlayer>();

                foreach (var userInfo in m_accountList)
                    userList.Add(userInfo.Value);

                return userList.ToArray();
            }
        }

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetServerProcedure procList)
        {
            procList.Set(this.WhenReqLogin, (int)MessageTypes.Req_Login);
            procList.Set(this.WhenReqLogout, (int)MessageTypes.Req_Logout);
            procList.Set(this.WhenReqRegister, (int)MessageTypes.Req_Register);
        }

        //#####################################################################################

        public void SaveAllAccount()
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(this.ServerPath + this.UserFileName, FileMode.Create)))
            {
                sw.WriteLine(Application.ProductVersion.ToString());


                foreach (var account in m_accountList)
                {
                    account.Value.SaveTo(sw);
                }
            }
        }

        public void LoadAllAccount()
        {
            m_accountList.Clear();


            try
            {
                using (StreamReader sr = new StreamReader(new FileStream(this.ServerPath + this.UserFileName, FileMode.Open)))
                {
                    Version fileVersion = new Version(sr.ReadLine().Trim());


                    while (!sr.EndOfStream)
                    {
                        var account = new GamePlayer();
                        account.LoadFrom(sr);

                        m_accountList.Add(account.Name, account);
                    }
                }
            }
            catch (FileNotFoundException)
            { }
            catch (EndOfStreamException)
            { }
        }

        public GamePlayer GetAccount(string name)
        {
            if (m_accountList.ContainsKey(name))
                return m_accountList[name];

            return null;
        }

        public void RemoveLoginUser(int id)
        {
            if (m_loginUserList.ContainsKey(id))
            {
                // 로그
                Utility.Logger.GetInstance().Log(string.Format("\"{0}\"님이 로그아웃 했습니다.",
                    this.GetLoginUser(id)?.Name));


                // 로그아웃 사실을 모두에게 알림
                NetMessageStream logoutNotice = new NetMessageStream();
                logoutNotice.WriteData(m_loginUserList[id].Name);

                this.NoticeDelegate(logoutNotice.CreateMessage((int)MessageTypes.Ntf_UserLogout));


                m_loginUserList.Remove(id);
            }
        }

        public GamePlayer GetLoginUser(int id)
        {
            if (m_loginUserList.ContainsKey(id))
                return m_loginUserList[id];
            else
                return null;
        }

        public GamePlayer GetLoginUser(string name)
        {
            foreach (var user in m_loginUserList)
            {
                if (user.Value.Name == name)
                    return user.Value;
            }


            return null;
        }

        public int GetLoginClientID(string name)
        {
            foreach (var user in m_loginUserList)
            {
                if (user.Value.Name == name)
                    return user.Key;
            }


            return -1;
        }

        public string GetLoginUserNameAt(int index)
        {
            var loginUsers = this.LoginUsers;

            if (index >= 0 && index < loginUsers.Length)
                return loginUsers[index].Name;

            return "";
        }

        public string GetAccountNameAt(int index)
        {
            var accounts = this.Accounts;

            if (index >= 0 && index < accounts.Length)
                return accounts[index].Name;

            return "";
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private NetMessage WhenReqLogin(ServerVisitor client, NetMessageStream msg)
        {
            string userName = msg.ReadData<string>();


            LoginResults loginResult = LoginResults.Success;


            // 로그인을 받을 수 있는 상태이면
            if (this.CanLogin)
            {
                string password = msg.ReadData<string>();
                string versionText = Utility.Version.Zero.ToString();
                if (!msg.EndOfStream)
                    versionText = msg.ReadData<string>();
                Utility.Version clientVersion = new Utility.Version(versionText);


                // 클라이언트 버전이 서버버전과 같은지 확인
                if (Application.ProductVersion == clientVersion)
                {
                    // 회원여부 확인 및 비밀번호 얻기
                    bool isMember = m_accountList.ContainsKey(userName);
                    string realPassword = "";

                    if (isMember)
                    {
                        realPassword = m_accountList[userName].Password;
                    }


                    // 회원이 아니면 실패
                    if (isMember == false)
                    {
                        loginResult = LoginResults.Fail_NotUser;
                    }
                    // 비밀번호가 틀리면 실패
                    else if (password != realPassword)
                    {
                        loginResult = LoginResults.Fail_WrongPassword;
                    }
                    // 이미 접속된 유저와 이름이 중복되면 실패
                    else if (m_loginUserList.Any((user) => user.Value.Name == userName))
                    {
                        loginResult = LoginResults.Fail_AlreadyLogin;
                    }


                    // 로그인 성공시
                    if (loginResult == LoginResults.Success)
                    {
                        // 접속자 목록에 추가
                        m_loginUserList.Add(client.ID, new GamePlayer()
                        {
                            Name = userName
                        });


                        // 로그인 사실을 모두에게 알림
                        NetMessageStream loginNotice = new NetMessageStream();
                        loginNotice.WriteData(userName);

                        this.NoticeDelegate(loginNotice.CreateMessage((int)MessageTypes.Ntf_UserLogin));
                    }
                }
                else
                {
                    // 클라이언트의 버전과 서버의 버전이 다르므로 접속불가
                    loginResult = LoginResults.Fail_DifferentVersion;
                }
            }
            else
            {
                // 로그인을 받을 수 없음
                loginResult = LoginResults.Fail_ServerNotReady;
            }


            // 로그
            Utility.Logger.GetInstance().Log(string.Format("\"{0}\"님이 로그인을 시도했습니다. 결과 : {1}",
                userName, loginResult.ToString()));


            // 로그인 결과 전송
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData((int)loginResult);

            return writer.CreateMessage((int)MessageTypes.Rsp_Login);
        }

        private NetMessage WhenReqLogout(ServerVisitor client, NetMessageStream msg)
        {
            this.RemoveLoginUser(client.ID);


            return null;
        }

        private NetMessage WhenReqRegister(ServerVisitor client, NetMessageStream msg)
        {
            RegisterResults registerResult = RegisterResults.Success;


            string userName = msg.ReadData<string>().Trim();
            string password = msg.ReadData<string>();
            Color userColor = Color.FromArgb(msg.ReadData<int>());


            // 회원여부 확인
            string userListFile = this.ServerPath + this.UserFileName;
            bool isMember = m_accountList.ContainsKey(userName);


            // 이미 회원이면 실패
            if (isMember)
            {
                registerResult = RegisterResults.Fail_AlreadyExist;
            }
            // 이름이 없거나 너무 길거나 금지문자가 포함되어 있으면 실패
            else if (userName.Length <= 0 || userName.Length > this.MaxNameLength
                || userName.IndexOf('\\') >= 0)
            {
                registerResult = RegisterResults.Fail_InvalidName;
            }
            // 비밀번호가 없거나 너무 길면 실패
            else if (password.Length <= 0 || password.Length > this.MaxPasswordLength)
            {
                registerResult = RegisterResults.Fail_InvalidPassword;
            }


            // 회원가입 가능시
            if (registerResult == RegisterResults.Success)
            {
                // 회원 목록에 추가

                var account = new GamePlayer()
                {
                    Name = userName,
                    Password = password,
                    AreaCount = 0,
                    UserColor = userColor
                };

                m_accountList.Add(userName, account);

                using (StreamWriter sw = new StreamWriter(new FileStream(userListFile, FileMode.Append)))
                {
                    account.SaveTo(sw);
                }
            }


            // 로그
            Utility.Logger.GetInstance().Log(string.Format("{0}-클라이언트가 \"{1}\"로 회원가입을 시도했습니다. 결과 : {2}",
                client.ID, userName, registerResult.ToString()));


            // 회원가입 결과 전송
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData((int)registerResult);

            return writer.CreateMessage((int)MessageTypes.Rsp_Register);
        }

        //#####################################################################################

        public void ForceLogout(string name)
        {
            int id = this.GetLoginClientID(name);
            this.Server.DisconnectClient(id);
        }

        public void RemoveAccount(string name)
        {
            m_accountList.Remove(name);

            this.SaveAllAccount();
        }
    }
}
