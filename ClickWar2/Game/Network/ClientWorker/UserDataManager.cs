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
    public class UserDataManager
    {
        public UserDataManager()
        {

        }

        //#####################################################################################

        public NetClient Client
        { get; set; }

        public SignManager SignDirector
        { get; set; }

        protected Dictionary<string, Color> m_userColorMap = new Dictionary<string, Color>();

        protected List<GamePlayer> m_userList = new List<GamePlayer>();
        public List<GamePlayer> UserList
        { get { return m_userList; } }

        public GamePlayer Me
        { get; set; } = new GamePlayer();

        public Dictionary<string, int> MyCompanySiteCount
        { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, List<string>> MyCompanyTechList
        { get; set; } = new Dictionary<string, List<string>>();

        public Dictionary<string, List<string>> MyCompanyProductList
        { get; set; } = new Dictionary<string, List<string>>();

        //#####################################################################################
        // 이벤트 관리자

        public UserEventList EventDirector
        { get; set; } = new UserEventList();

        //#####################################################################################
        // 메세지 수신 콜백

        protected Action<string, Color> m_userColorCallback = null;
        protected int m_userColorCallbackCount = 0;

        protected Action m_allUserInfoCallback = null;

        protected Action m_myAllCompanyNameCallback = null;

        //#####################################################################################
        // 메세지 처리자 등록

        public void InitEventChain(NetClientProcedure procList)
        {
            procList.Set(this.WhenRspUserColor, (int)MessageTypes.Rsp_UserColor);
            procList.Set(this.WhenNtfUserLogin, (int)MessageTypes.Ntf_UserLogin);
            procList.Set(this.WhenNtfUserLogout, (int)MessageTypes.Ntf_UserLogout);
            procList.Set(this.WhenRspAllUserInfo, (int)MessageTypes.Rsp_AllUserInfo);
            procList.Set(this.WhenRspMyAllCompanyName, (int)MessageTypes.Rsp_MyAllCompanyName);
            procList.Set(this.WhenRspMyAllCompanySiteCount, (int)MessageTypes.Rsp_MyAllCompanySiteCount);
            procList.Set(this.WhenRspMyAllCompanyTechList, (int)MessageTypes.Rsp_MyAllCompanyTechList);
            procList.Set(this.WhenRspMyAllCompanyProductList, (int)MessageTypes.Rsp_MyAllCompanyProductList);
        }

        //#####################################################################################
        // 수신된 메세지 처리

        private void WhenRspUserColor(NetMessageStream msg)
        {
            string userName = msg.ReadString();
            int colorRgb = msg.ReadInt32();


            if (m_userColorMap.ContainsKey(userName))
                m_userColorMap[userName] = Color.FromArgb(colorRgb);
            else
                m_userColorMap.Add(userName, Color.FromArgb(colorRgb));


            if (m_userColorCallback != null)
            {
                m_userColorCallback(userName, Color.FromArgb(colorRgb));


                --m_userColorCallbackCount;

                if (m_userColorCallbackCount <= 0)
                {
                    m_userColorCallbackCount = 0;
                    m_userColorCallback = null;
                }
            }
        }

        private void WhenNtfUserLogin(NetMessageStream msg)
        {
            string userName = msg.ReadString();

            EventDirector.StartEvent((int)UserEvents.Login, userName);
        }

        private void WhenNtfUserLogout(NetMessageStream msg)
        {
            string userName = msg.ReadString();

            EventDirector.StartEvent((int)UserEvents.Logout, userName);
        }

        private void WhenRspAllUserInfo(NetMessageStream msg)
        {
            bool bClear = (msg.ReadInt32() != 0);

            if (bClear)
            {
                m_userList.Clear();
            }
            else
            {
                int colorRgb = msg.ReadInt32();
                string name = msg.ReadString();
                int area = msg.ReadInt32();
                int resource = msg.ReadInt32();
                bool isEnd = (msg.ReadInt32() != 0);

                // 자신의 정보이면 따로 저장
                if (name == this.SignDirector.LoginName)
                {
                    this.Me.UserColor = Color.FromArgb(colorRgb);
                    this.Me.Name = name;
                    this.Me.AreaCount = area;
                    this.Me.Resource = resource;
                }

                // 목록에 추가
                m_userList.Add(new GamePlayer()
                {
                    UserColor = Color.FromArgb(colorRgb),
                    Name = name,
                    AreaCount = area,
                    Resource = resource,
                });


                if (isEnd)
                {
                    if (m_allUserInfoCallback != null)
                    {
                        m_allUserInfoCallback();
                        m_allUserInfoCallback = null;
                    }
                }
            }
        }

        private void WhenRspMyAllCompanyName(NetMessageStream msg)
        {
            this.Me.Companies.Clear();


            int count = msg.ReadInt32();

            for (int i = 0; i < count; ++i)
            {
                this.Me.Companies.Add(msg.ReadString());
            }


            if (m_myAllCompanyNameCallback != null)
            {
                m_myAllCompanyNameCallback();
                m_myAllCompanyNameCallback = null;
            }
        }

        private void WhenRspMyAllCompanySiteCount(NetMessageStream msg)
        {
            this.MyCompanySiteCount.Clear();


            int count = msg.ReadInt32();

            for (int i = 0; i < count; ++i)
            {
                string company = msg.ReadString();
                int siteCount = msg.ReadInt32();

                if (this.MyCompanySiteCount.ContainsKey(company))
                    this.MyCompanySiteCount[company] = siteCount;
                else
                    this.MyCompanySiteCount.Add(company, siteCount);
            }
        }

        private void WhenRspMyAllCompanyTechList(NetMessageStream msg)
        {
            this.MyCompanyTechList.Clear();


            int count = msg.ReadInt32();

            for (int i = 0; i < count; ++i)
            {
                string company = msg.ReadString();
                int techCount = msg.ReadInt32();

                List<string> techList = null;

                if (this.MyCompanyTechList.ContainsKey(company))
                    techList = this.MyCompanyTechList[company];
                else
                {
                    techList = new List<string>();
                    this.MyCompanyTechList.Add(company, techList);
                }

                for (int t = 0; t < techCount; ++t)
                {
                    techList.Add(msg.ReadString());
                }
            }
        }

        private void WhenRspMyAllCompanyProductList(NetMessageStream msg)
        {
            this.MyCompanyProductList.Clear();


            int count = msg.ReadInt32();

            for (int i = 0; i < count; ++i)
            {
                string company = msg.ReadString();
                int productCount = msg.ReadInt32();

                List<string> productList = null;

                if (this.MyCompanyProductList.ContainsKey(company))
                    productList = this.MyCompanyProductList[company];
                else
                {
                    productList = new List<string>();
                    this.MyCompanyProductList.Add(company, productList);
                }

                for (int t = 0; t < productCount; ++t)
                {
                    productList.Add(msg.ReadString());
                }
            }
        }

        //#####################################################################################
        // 사용자 입력 처리

        public void RequestUserColor(string userName, Action<string, Color> callbackAsync)
        {
            m_userColorCallback = callbackAsync;
            ++m_userColorCallbackCount;


            // 해당 유저의 색 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(userName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_UserColor));
        }

        public void RequestAllUserInfo(Action callbackAsync)
        {
            m_allUserInfoCallback = callbackAsync;


            // 모든 유저의 정보 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData("");

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_AllUserInfo));
        }

        public void RequestMyAllCompanyName(Action callbackAsync)
        {
            m_myAllCompanyNameCallback = callbackAsync;


            // 자신의 모든 회사 이름 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_MyAllCompanyName));
        }

        public void RequestMyAllCompanySiteCount()
        {
            // 자신의 모든 회사별 건물 개수 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_MyAllCompanySiteCount));
        }

        public void RequestMyAllCompanyTechList()
        {
            // 자신의 모든 회사별 기술 정보 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_MyAllCompanyTechList));
        }

        public void RequestMyAllCompanyProductList()
        {
            // 자신의 모든 회사별 제품 정보 요청
            NetMessageStream writer = new NetMessageStream();
            writer.WriteData(this.SignDirector.LoginName);

            this.Client.SendMessage(writer.CreateMessage((int)MessageTypes.Req_MyAllCompanyProductList));
        }

        //#####################################################################################

        public Color GetUserColor(string userName)
        {
            if (m_userColorMap.ContainsKey(userName))
                return m_userColorMap[userName];
            else
                return Color.White;
        }

        public void AddMyCompanySiteCount(string companyName, int deltaCount)
        {
            if (this.MyCompanySiteCount.ContainsKey(companyName))
                this.MyCompanySiteCount[companyName] += deltaCount;
            else
                this.MyCompanySiteCount.Add(companyName, deltaCount);
        }

        public int GetMyCompanySiteCount(string companyName)
        {
            if (this.MyCompanySiteCount.ContainsKey(companyName))
                return this.MyCompanySiteCount[companyName];
            else
                return 0;
        }

        public bool CheckMyCompany(string companyName)
        {
            return this.Me.Companies.Any((name) => name == companyName);
        }

        public List<string> GetMyCompanyTechList(string companyName)
        {
            if (this.MyCompanyTechList.ContainsKey(companyName))
                return this.MyCompanyTechList[companyName];
            else
            {
                var tempList = new List<string>();
                this.MyCompanyTechList.Add(companyName, tempList);

                return tempList;
            }
        }

        public void AddTechInMyCompany(string companyName, string techName)
        {
            if (this.MyCompanyTechList.ContainsKey(companyName))
                this.MyCompanyTechList[companyName].Add(techName);
            else
            {
                var tempList = new List<string>();
                this.MyCompanyTechList.Add(companyName, tempList);

                tempList.Add(techName);
            }
        }

        public void RemoveTechInMyCompany(string companyName, string techName)
        {
            if (this.MyCompanyTechList.ContainsKey(companyName))
                this.MyCompanyTechList[companyName].Remove(techName);
        }

        public List<string> GetMyCompanyProductList(string companyName)
        {
            if (this.MyCompanyProductList.ContainsKey(companyName))
                return this.MyCompanyProductList[companyName];
            else
            {
                var tempList = new List<string>();
                this.MyCompanyProductList.Add(companyName, tempList);

                return tempList;
            }
        }

        public void AddProductInMyCompany(string companyName, string productName)
        {
            if (this.MyCompanyProductList.ContainsKey(companyName))
                this.MyCompanyProductList[companyName].Add(productName);
            else
            {
                var tempList = new List<string>();
                this.MyCompanyProductList.Add(companyName, tempList);

                tempList.Add(productName);
            }
        }

        public void RemoveProductInMyCompany(string companyName, int productIndex)
        {
            if (this.MyCompanyProductList.ContainsKey(companyName))
            {
                var list = this.MyCompanyProductList[companyName];

                if (productIndex >= 0 && productIndex < list.Count)
                {
                    list.RemoveAt(productIndex);
                }
            }
        }
    }
}
