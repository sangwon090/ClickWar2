using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickWar2.Game.Network.ServerWorker;
using ClickWar2.Network;

namespace ClickWar2.Game.Network
{
    public class GameServer
    {
        public GameServer(string serverPath)
        {
            // 서버 경로 등록
            this.ServerPath = serverPath;


            // 서버 이벤트 등록
            m_server.WhenDisconnected += WhenDisconnected;


            // 메세지 처리자 초기화
            m_userManager.Server = m_server;
            m_userManager.NoticeDelegate = m_server.SendMessageToAll;
            m_userManager.InitEventChain(m_procList);
            m_userManager.LoadAllAccount();

            m_noticeManager.NoticeDelegate = m_server.SendMessageToAll;
            m_noticeManager.InitEventChain(m_procList);

            m_gameBoardManager.NoticeDelegate = m_server.SendMessageToAll;
            m_gameBoardManager.NoticeWhereDelegate = this.NoticeWhere;
            m_gameBoardManager.FindClientByIDDelegate = m_server.GetClientByID;
            m_gameBoardManager.GameBoard = m_board;
            m_gameBoardManager.UserDirector = m_userManager;
            m_gameBoardManager.CompanyDirector = m_companyManager;
            m_gameBoardManager.InitEventChain(m_procList);

            m_userDataManager.UserDirector = m_userManager;
            m_userDataManager.CompanyDirector = m_companyManager;
            m_userDataManager.InitEventChain(m_procList);

            m_talkManager.Server = m_server;
            m_talkManager.UserDirector = m_userManager;
            m_talkManager.InitEventChain(m_procList);

            m_companyManager.NoticeDelegate = m_server.SendMessageToAll;
            m_companyManager.NoticeWhereDelegate = this.NoticeWhere;
            m_companyManager.Server = m_server;
            m_companyManager.UserDirector = m_userManager;
            m_companyManager.GameBoard = m_board;
            m_companyManager.InitEventChain(m_procList);
            m_companyManager.LoadAll();


            // 자동저장 타이머 설정
            m_autoSaveTimer.Set(0, (int)TimeSpan.FromMinutes(5.0).TotalMilliseconds);


            // 유저 확인 타이머 설정
            m_checkUserTimer.Set(0, (int)TimeSpan.FromMinutes(5.0).TotalMilliseconds);
        }

        //#####################################################################################
        // 서버

        protected NetServer m_server = new NetServer();
        protected NetServerProcedure m_procList = new NetServerProcedure();

        public NetServer Server
        { get { return m_server; } }

        public bool IsOpened
        { get { return m_server.IsValid; } }

        private string m_serverPath;
        public string ServerPath
        {
            get { return m_serverPath; }
            set
            {
                if (value.Length > 0 && value[value.Length - 1] != '/'
                    && value[value.Length - 1] != '\\')
                {
                    throw new ArgumentException("ServerPath의 마지막 문자는 \'/\'나 \'\\\'로 끝나야 합니다.");
                }

                m_serverPath = value;
                m_userManager.ServerPath = value;
                m_companyManager.ServerPath = value;
                m_board.BoardPath = value + "Map/";
            }
        }

        //#####################################################################################
        // 메세지 처리자

        protected UserManager m_userManager = new UserManager();
        public UserManager UserDirector
        { get { return m_userManager; } }

        protected NoticeManager m_noticeManager = new NoticeManager();
        public NoticeManager NoticeDirector
        { get { return m_noticeManager; } }

        protected GameBoardManager m_gameBoardManager = new GameBoardManager();
        public GameBoardManager GameBoardDirector
        { get { return m_gameBoardManager; } }

        protected UserDataManager m_userDataManager = new UserDataManager();
        public UserDataManager UserDataDirector
        { get { return m_userDataManager; } }

        protected CommunicationManager m_talkManager = new CommunicationManager();
        public CommunicationManager TalkDirector
        { get { return m_talkManager; } }

        protected CompanyManager m_companyManager = new CompanyManager();
        public CompanyManager CompanyDirector
        { get { return m_companyManager; } }

        //#####################################################################################
        // 게임

        protected GameBoard m_board = new GameBoard();
        public GameBoard GameBoard
        { get { return m_board; } }

        protected Utility.TimeLagManager m_autoSaveTimer = new Utility.TimeLagManager();
        protected bool m_timeToLoadMap = true;

        protected Utility.TimeLagManager m_checkUserTimer = new Utility.TimeLagManager();

        //#####################################################################################
        // 서버

        public void Start(string port)
        {
            // 서버 시작
            m_server.Start(port);

            // 로그인을 받지않도록 설정.
            this.UserDirector.CanLogin = false;


            // 맵을 새로 불러오도록 함
            m_timeToLoadMap = true;
        }

        public void Stop()
        {
            // 서버 중지
            m_server.Stop();


            // 유저 정보 저장
            m_userManager.SaveAllAccount();


            // 회사 정보 저장
            m_companyManager.SaveAll();


            // 맵 저장 후 초기화
            m_board.SaveAll();
            m_board.Clear();
        }

        public void StartSave()
        {
            // 남은 시간에 관계없이 Tick이 발생하도록 설정.
            m_autoSaveTimer.Run(0);
        }

        //#####################################################################################
        // 서버 이벤트 처리
        
        private void WhenDisconnected(ServerVisitor client)
        {
            // 접속자 목록에서 제거
            m_userManager.RemoveLoginUser(client.ID);
        }

        //#####################################################################################
        // 갱신

        public void Update()
        {
            // 게임판 갱신
            this.GameBoardDirector.Update();


            // 수신된 메세지 처리
            m_server.ReceiveMessageInto(m_procList);


            // 유저 확인을 할때가 되었으면
            if (m_checkUserTimer.Tick(0))
            {
                // 랜덤한 간격으로 재설정
                m_checkUserTimer.Set(0, (int)TimeSpan.FromMinutes(Utility.Random.Next(4, 8)).TotalMilliseconds);
                m_checkUserTimer.Update(0);

                // 확인 요청
                m_noticeManager.CheckUser();
            }


            // 맵 불러오기
            if (m_timeToLoadMap)
            {
                m_timeToLoadMap = this.GameBoard.LoadNext();

                // 맵 로딩이 완료되었으면
                if (m_timeToLoadMap == false)
                {
                    // 로그인을 받도록 설정.
                    this.UserDirector.CanLogin = true;
                }
            }
            else
            {
                // 불러오는 도중에는 저장안함.

                // 자동저장
                if (m_autoSaveTimer.Tick(0))
                {
                    // 더이상 저장할 청크가 없으면
                    if (this.GameBoard.SaveNext() == false)
                    {
                        // 유저정보 저장
                        m_userManager.SaveAllAccount();


                        // 회사 정보 저장
                        m_companyManager.SaveAll();


                        // 타이머 리셋
                        m_autoSaveTimer.Update(0);
                    }
                }
            }
        }

        //#####################################################################################
        // 통신

        public void NoticeWhere(string[] userNames, ClickWar2.Network.Protocol.NetMessage msg)
        {
            for (int i = 0; i < userNames.Length; ++i)
            {
                int id = m_userManager.GetLoginClientID(userNames[i]);
                var client = m_server.GetClientByID(id);

                if (client != null)
                {
                    client.Sender.SendMessage(msg.Clone());
                }
            }
        }
    }
}
