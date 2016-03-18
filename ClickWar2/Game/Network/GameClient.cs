using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickWar2.Network;
using ClickWar2.Game.Network.ClientWorker;

namespace ClickWar2.Game.Network
{
    public class GameClient
    {
        public GameClient()
        {
            // 클라이언트 이벤트 등록
            m_client.WhenDisconnected += this.WhenDisconnected;


            // 메세지 처리자 초기화
            m_signManager.Client = m_client;
            m_signManager.InitEventChain(m_procList);

            m_noticeManager.Client = m_client;
            m_noticeManager.InitEventChain(m_procList);

            m_gameBoardManager.Client = m_client;
            m_gameBoardManager.GameBoard = m_board;
            m_gameBoardManager.SignDirector = m_signManager;
            m_gameBoardManager.InitEventChain(m_procList);

            m_userDataManager.Client = m_client;
            m_userDataManager.SignDirector = m_signManager;
            m_userDataManager.InitEventChain(m_procList);

            m_talkManager.Client = m_client;
            m_talkManager.SignDirector = m_signManager;
            m_talkManager.InitEventChain(m_procList);

            m_companyManager.Client = m_client;
            m_companyManager.SignDirector = m_signManager;
            m_companyManager.BoardDirector = m_gameBoardManager;
            m_companyManager.UserDataDirector = m_userDataManager;
            m_companyManager.InitEventChain(m_procList);
        }

        //#####################################################################################
        // 클라이언트

        protected NetClient m_client = new NetClient();
        protected NetClientProcedure m_procList = new NetClientProcedure();

        public bool IsConnected
        { get { return m_client.IsValid; } }

        public event Action WhenDisconnectedAgainstExpectation = null;

        public int ReceiveBufferSize
        { get { return m_client.ReceiveBufferSize; } }

        public int SendBufferSize
        { get { return m_client.SendBufferSize; } }

        //#####################################################################################
        // 메세지 처리자

        protected SignManager m_signManager = new SignManager();
        public SignManager SignDirector
        { get { return m_signManager; } }

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

        //#####################################################################################
        // 클라이언트

        public void Connect(string address, string port)
        {
            m_client.Connect(address, port);
        }

        public void Disconnect()
        {
            m_client.Disconnect();
        }

        //#####################################################################################
        // 클라이언트 이벤트 처리

        private void WhenDisconnected()
        {
            if (WhenDisconnectedAgainstExpectation != null)
            {
                WhenDisconnectedAgainstExpectation();
            }
        }

        //#####################################################################################
        // 수신된 메세지 처리

        public void Update()
        {
            // 수신된 메세지 처리
            m_client.ReceiveMessageInto(m_procList);
        }
    }
}
