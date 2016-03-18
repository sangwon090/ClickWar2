using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using ClickWar2.Game.Network;

namespace ClickWar2.Game.Presenter
{
    public enum ClickModes
    {
        Wait,
        Normal,
        BuildCountry,
    }

    public enum LimitTypes
    {
        AddPowerClick,
        AttackClick,
    }


    public class ClientBoardPresenter : BoardPresenter
    {
        public ClientBoardPresenter()
        {
            m_clickLimiter.Set((int)LimitTypes.AddPowerClick, GameValues.MinAddPowerDelay);
            m_clickLimiter.Set((int)LimitTypes.AttackClick, GameValues.MinAttackDelay);
        }

        //#####################################################################################

        public GameClient Client
        { get; set; } = null;

        //#####################################################################################

        public Func<string> InputCompanyNameDelegate = null;
        public Func<string> InputSelectedCompanyNameDelegate = null;
        public Func<int> InputSelectedCompanyProductDelegate = null;

        //#####################################################################################

        protected ClickModes m_clickMode = ClickModes.Wait;
        protected Utility.TimeLagManager m_clickLimiter = new Utility.TimeLagManager();

        protected Point m_prevTile;
        protected bool m_onLeftDrag = false;

        //#####################################################################################

        protected View.IContextMenu m_contextMenu = null;
        public View.IContextMenu ContextMenu
        {
            get { return m_contextMenu; }
            set
            {
                if (m_contextMenu != null)
                {
                    m_contextMenu.Clear();
                }


                m_contextMenu = value;


                value.Clear();

                value.AddMenu("Context_Cancel", "닫기",
                    this.WhenContext_Cancel_Clicked);
                value.AddMenu("Context_BuildFactory", "공장 짓기",
                    this.WhenContext_BuildFactory_Clicked);
                value.AddMenu("Context_DestroyFactory", "공장 폐쇄",
                    this.WhenContext_DestroyFactory_Clicked);
                value.AddMenu("Context_BuildCompany", "회사 짓기",
                    this.WhenContext_BuildCompany_Clicked);
                value.AddMenu("Context_DestroyCompany", "회사 폐쇄",
                    this.WhenContext_DestroyCompany_Clicked);
                value.AddMenu("Context_BuildProduct", "제품 설치",
                    this.WhenContext_BuildProduct_Clicked);
                value.AddMenu("Context_DestroyProduct", "제품 파괴",
                    this.WhenContext_DestroyProduct_Clicked);
                value.AddMenu("Context_ConvertAllResource", "모든 자원을 힘으로 변환",
                    this.WhenContext_ConvertAllResource_Clicked);
            }
        }

        //#####################################################################################

        protected Point m_lastBoardLocation = new Point(0, 0);
        protected Size m_lastScreenSize = new Size(0, 0);
        protected int m_lastTileSize = 0;

        //#####################################################################################

        public override void Initialize()
        {
            // 이벤트 등록
            this.Client.GameBoardDirector.EventDirector.SetEvent((int)Event.BoardEvents.BuildCountry,
                this.WhenCountryBuiltEvent);
            this.Client.GameBoardDirector.EventDirector.SetEvent((int)Event.BoardEvents.PowerUpTerritory,
                this.WhenPowerUpTerritoryEvent);
            this.Client.GameBoardDirector.EventDirector.SetEvent((int)Event.BoardEvents.AttackTerritory,
                this.WhenAttackTerritoryEvent);
            this.Client.GameBoardDirector.EventDirector.SetEvent((int)Event.BoardEvents.SendPower,
                this.WhenSendPowerEvent);
            this.Client.GameBoardDirector.EventDirector.SetEvent((int)Event.BoardEvents.EditTileSign,
                this.WhenEditTileSignEvent);
            this.Client.GameBoardDirector.EventDirector.SetEvent((int)Event.BoardEvents.EndUser,
                this.WhenEndUserEvent);
            this.Client.GameBoardDirector.EventDirector.SetEvent((int)Event.BoardEvents.ConvertAllResource,
                this.WhenConvertAllResourceEvent);

            this.Client.UserDataDirector.EventDirector.SetEvent((int)Event.UserEvents.Login,
                this.WhenOtherUserLoginEvent);
            this.Client.UserDataDirector.EventDirector.SetEvent((int)Event.UserEvents.Logout,
                this.WhenOtherUserLogoutEvent);

            this.Client.TalkDirector.EventDirector.SetEvent((int)Event.CommunicationEvents.ReceiveMail,
                this.WhenReceiveMailEvent);

            this.Client.CompanyDirector.EventDirector.SetEvent((int)Event.CompanyEvents.BuildCompany,
                this.WhenBuildCompanyEvent);
            this.Client.CompanyDirector.EventDirector.SetEvent((int)Event.CompanyEvents.DestroyCompany,
                this.WhenDestroyCompanyEvent);
            this.Client.CompanyDirector.EventDirector.SetEvent((int)Event.CompanyEvents.BuyTech,
                this.WhenBuyTechEvent);
            this.Client.CompanyDirector.EventDirector.SetEvent((int)Event.CompanyEvents.BuyProduct,
                this.WhenBuyProductEvent);


            // 플레이어 국가 위치 찾기
            this.Client.GameBoardDirector.RequestCountryLocation(this.WhenReceiveCountryLocation);
        }

        public override void Update()
        {
            this.UI.LeftAddPowerCooltime = m_clickLimiter.LeftTime((int)LimitTypes.AddPowerClick);
            this.UI.LeftAttackCooltime = m_clickLimiter.LeftTime((int)LimitTypes.AttackClick);


            // 플레이어 화면 영역 갱신
            if (m_lastBoardLocation != this.BoardView.BoardLocation
                || m_lastScreenSize != this.BoardView.ScreenSize
                || m_lastTileSize != this.BoardView.TileSize)
            {
                m_lastBoardLocation = this.BoardView.BoardLocation;
                m_lastScreenSize = this.BoardView.ScreenSize;
                m_lastTileSize = this.BoardView.TileSize;


                const int bezel = 16;

                Point beginPos = this.ViewPointToTileIndex(new Point(0, 0));
                Size screenSize = this.BoardView.ScreenSize;
                screenSize.Width /= this.BoardView.TileSize;
                screenSize.Height /= this.BoardView.TileSize;

                this.Client.GameBoardDirector.UpdateMyScreen(beginPos.X - bezel, beginPos.Y - bezel,
                    screenSize.Width + bezel * 2, screenSize.Height + bezel * 2);
            }
        }

        protected override void WhenCursorChanged(Point cursor, Point tile)
        {
            // 컨텍스트 메뉴가 닫힌 상태여야 보드의 선택된 타일을 갱신함.
            if (this.ContextMenu.Visible == false)
            {
                this.BoardView.Cursor = cursor;
                this.BoardView.TileCursor = tile;
            }
        }

        protected override void WhenLeftDown(Point cursor, Point tile)
        {
            this.ContextMenu.Cursor = cursor;
            if (this.ContextMenu.Visible && this.ContextMenu.Focused)
            {
                // 컨텍스트 메뉴 클릭
                this.ContextMenu.OnClick(cursor);
            }
            else
            {
                switch (m_clickMode)
                {
                    case ClickModes.BuildCountry:
                        m_clickMode = ClickModes.Wait; // NOTE: 이걸 먼저해야 callback 호출에서 클릭모드를 변경하는게 적용됨.
                        this.Client.GameBoardDirector.BuildCountry(tile.X, tile.Y,
                            this.WhenMyCountryBuilt);
                        break;

                    case ClickModes.Normal:
                        this.ProceedNormalClick(tile);
                        break;
                }
            }


            // 컨텍스트 메뉴 닫기
            this.ContextMenu.Visible = false;
        }

        protected override void WhenLeftUp(Point cursor, Point tile)
        {
            if (m_onLeftDrag)
            {
                m_onLeftDrag = false;


                // 드래그 보조 UI 숨기기
                this.BoardView.HideLeftDragHelper();


                // 드래그 처리
                this.ProceedLeftDrag(m_prevTile, tile);
            }
        }

        protected override void WhenRightDown(Point cursor, Point tile)
        {
            // 컨텍스트 메뉴 닫기
            this.ContextMenu.Visible = false;
        }

        protected override void WhenRightUp(Point cursor, Point tile)
        {
            // 화면 이동이 없었으면
            if (m_cursorBeginMove == cursor)
            {
                // 컨텍스트 메뉴 열기
                this.ContextMenu.Location = cursor;
                this.ContextMenu.Visible = true;
            }
        }

        protected override void WhenCursorMove(Point cursor, Point tile)
        {
            // 컨텍스트 메뉴 갱신
            this.ContextMenu.Cursor = cursor;
        }

        protected override void WhenKeyDown(Keys key)
        {
            if (key == Keys.Home)
            {
                int homeX, homeY;
                if (this.Client.GameBoard.FindCountryLocation(this.Client.SignDirector.LoginName,
                    out homeX, out homeY))
                {
                    this.BoardView.MoveScreenTo(homeX, homeY);
                }
            }
            else if (key == Keys.Delete)
            {
                Point tilePos = this.ViewPointToTileIndex(this.OldCursor);

                // 해당 타일의 푯말을 제거할 것을 요청.
                this.Client.GameBoardDirector.EditSign(tilePos.X, tilePos.Y, "");
            }
        }

        protected override void WhenInputText(string text)
        {
            Point tilePos = this.ViewPointToTileIndex(this.OldCursor);

            // 해당 타일의 푯말 변경을 요청.
            this.Client.GameBoardDirector.EditSign(tilePos.X, tilePos.Y, text);
        }

        protected override void WhenRequestUserColor(string userName)
        {
            // 해당 유저의 색깔을 요청.
            this.Client.UserDataDirector.RequestUserColor(userName, this.WhenReceiveUserColor);
        }

        protected override void WhenLongLeftClick(Point cursor, Point tile)
        {
            // 드래그 보조 UI 보이기
            this.BoardView.ShowLeftDragHelper(tile);


            // 이전 타일 위치에 저장
            m_prevTile = tile;
            // 드래그 플래그 설정
            m_onLeftDrag = true;
        }

        protected override void WhenInputMail(string targetName, string message)
        {
            // 메일 전송 요청
            this.Client.TalkDirector.SendMailTo(targetName, message);
        }

        protected override void WhenInputCopApplicationForm(string name, Action<RegisterCompanyResults> callbackAsync)
        {
            // 회사 등록 요청
            this.Client.CompanyDirector.RegisterCompany(name, callbackAsync);
        }

        protected override void WhenDevelopTech(string companyName, string techName, List<Command> program,
            Action<DevelopTechResults> callbackAsync)
        {
            // 기술 등록 요청
            this.Client.CompanyDirector.DevelopTech(companyName, techName, program, callbackAsync);
        }

        protected override void WhenDiscardTech(string companyName, string techName, Action callbackAsync)
        {
            // 기술 폐기 요청
            this.Client.CompanyDirector.DiscardTech(companyName, techName, callbackAsync);
        }

        protected override void WhenProduceProduct(string companyName, string techName, Action callbackAsync)
        {
            // 제품 생산 요청
            this.Client.CompanyDirector.ProduceProduct(companyName, techName, callbackAsync);
        }

        protected override void WhenDiscardProduct(string companyName, int productIndex, Action callbackAsync)
        {
            // 제품 폐기 요청
            this.Client.CompanyDirector.DiscardProduct(companyName, productIndex, callbackAsync);
        }

        protected override void WhenRequestTechProgram(string companyName, string techName,
            Action<string, string, string> callbackAsync)
        {
            // 기술 프로그램 요청
            this.Client.CompanyDirector.RequestTechProgram(companyName, techName, callbackAsync);
        }

        protected override void WhenSellTech(string companyName, string techName, int price, string targetUser)
        {
            // 기술 판매 요청
            this.Client.CompanyDirector.SellTech(companyName, techName, price, targetUser);
        }

        protected override void WhenBuyTech(string sellerName, string techName, int price, string buyerName)
        {
            // 기술 구매 요청
            this.Client.CompanyDirector.BuyTech(sellerName, techName, price, buyerName);
        }

        protected override void WhenSellProduct(string companyName, int productIndex, int price, string targetUser)
        {
            // 제품 판매 요청
            this.Client.CompanyDirector.SellProduct(companyName, productIndex, price, targetUser);
        }

        protected override void WhenBuyProduct(string sellerName, string productName, int price, string buyerName)
        {
            // 제품 구매 요청
            this.Client.CompanyDirector.BuyProduct(sellerName, productName, price, buyerName);
        }

        //#####################################################################################

        protected void WhenContext_Cancel_Clicked()
        {
            // 컨텍스트 메뉴 닫기
            this.ContextMenu.Visible = false;
        }

        protected void WhenContext_BuildFactory_Clicked()
        {
            Point tilePos = this.BoardView.TileCursor;

            // 공장 생성 요청
            this.Client.GameBoardDirector.BuildFactory(tilePos.X, tilePos.Y);
        }

        protected void WhenContext_DestroyFactory_Clicked()
        {
            Point tilePos = this.BoardView.TileCursor;

            // 공장 폐쇄 요청
            this.Client.GameBoardDirector.DestroyFactory(tilePos.X, tilePos.Y);
        }

        protected void WhenContext_BuildCompany_Clicked()
        {
            if (this.InputCompanyNameDelegate != null)
            {
                Point tilePos = this.BoardView.TileCursor;

                string companyName = this.InputCompanyNameDelegate().Trim();

                if (companyName.Length > 0)
                {
                    // 회사 건설 요청
                    this.Client.CompanyDirector.BuildCompany(companyName, tilePos.X, tilePos.Y);
                }
            }
        }

        protected void WhenContext_DestroyCompany_Clicked()
        {
            Point tilePos = this.BoardView.TileCursor;

            // 회사 폐쇄 요청
            this.Client.CompanyDirector.DestroyCompany(tilePos.X, tilePos.Y);
        }

        protected void WhenContext_BuildProduct_Clicked()
        {
            if (this.InputSelectedCompanyNameDelegate != null
                && this.InputSelectedCompanyProductDelegate != null)
            {
                string companyName = this.InputSelectedCompanyNameDelegate().Trim();
                int productIndex = this.InputSelectedCompanyProductDelegate();

                // 회사이름과 제품번호가 유효하면
                if (companyName.Length > 0 && productIndex >= 0)
                {
                    Point tilePos = this.BoardView.TileCursor;

                    // 제품 설치 요청
                    this.Client.GameBoardDirector.BuildChip(tilePos.X, tilePos.Y,
                        companyName, productIndex);
                }
            }
        }

        protected void WhenContext_DestroyProduct_Clicked()
        {
            Point tilePos = this.BoardView.TileCursor;

            // 제품 제거 요청
            this.Client.GameBoardDirector.DestroyChip(tilePos.X, tilePos.Y);
        }

        protected void WhenContext_ConvertAllResource_Clicked()
        {
            Point tilePos = this.BoardView.TileCursor;

            // 자원 변환 요청
            this.Client.GameBoardDirector.ConvertAllResource(tilePos.X, tilePos.Y);
        }

        //#####################################################################################

        protected void ProceedNormalClick(Point tile)
        {
            // 클릭한 타일이 존재하고
            if (this.Client.GameBoard.Board.ContainsItemAt(tile.X, tile.Y))
            {
                var currentTile = this.Client.GameBoard.Board.GetItemAt(tile.X, tile.Y);

                // 자신의 영토이면 힘을 상승시키고
                // 적군의 영토이거나 빈 영토이면 공격/점령 행동.
                if (currentTile.Owner == this.Client.SignDirector.LoginName)
                {
                    if (m_clickLimiter.Tick((int)LimitTypes.AddPowerClick))
                    {
                        // 힘 상승
                        this.Client.GameBoardDirector.AddTilePower(tile.X, tile.Y, null);


                        m_clickLimiter.Update((int)LimitTypes.AddPowerClick);
                    }
                }
                else
                {
                    if (m_clickLimiter.Tick((int)LimitTypes.AttackClick))
                    {
                        // 공격/점령
                        this.Client.GameBoardDirector.AttackTerritory(tile.X, tile.Y, null);


                        m_clickLimiter.Update((int)LimitTypes.AttackClick);
                    }
                }
            }
        }

        protected void ProceedLeftDrag(Point prevTile, Point currentTile)
        {
            if (prevTile != currentTile)
            {
                // 힘 전달 요청
                this.Client.GameBoardDirector.SendTerritoryPower(prevTile.X, prevTile.Y,
                    currentTile.X, currentTile.Y);
            }
        }

        protected void AddCircleEffect(int tileX, int tileY, int beginSize, int endSize, int ExpandSpeed,
            Color color)
        {
            if (this.Client.GameBoard.Board.ContainsItemAt(tileX, tileY))
            {
                int tileSize = this.BoardView.TileSize;

                tileX = tileX * tileSize + this.BoardView.BoardLocation.X + tileSize / 2;
                tileY = tileY * tileSize + this.BoardView.BoardLocation.Y + tileSize / 2;
                this.EffectDirector.AddCircleEffect(tileX, tileY, beginSize, endSize, ExpandSpeed,
                    color);
            }
        }

        //#####################################################################################

        protected override GameBoard GetGameBoard()
        {
            return this.Client.GameBoard;
        }

        //#####################################################################################

        private void WhenCountryBuiltEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            // 이펙트 발생
            this.AddCircleEffect(tileX, tileY, 4, 2048, 48, Color.LightGreen);


            // 이벤트 메세지 추가
            StringBuilder eventMsg = new StringBuilder("\"");
            eventMsg.Append(activeUser);
            eventMsg.Append("\"님이 건국하셨습니다!");

            this.UI.AddEventMessage(eventMsg.ToString(), Color.LightGreen);
        }

        private void WhenPowerUpTerritoryEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            // 이펙트 발생
            this.AddCircleEffect(tileX, tileY, 1, 32, 4, Color.Yellow);
        }

        private void WhenAttackTerritoryEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            // 이펙트 발생
            this.AddCircleEffect(tileX, tileY, 4, 128, 8, Color.Red);


            // 이벤트 메세지 추가
            StringBuilder eventMsg = new StringBuilder("\"");
            eventMsg.Append(activeUser);
            eventMsg.Append("\"님이 ");

            if (args != null && args.Length > 0 && args[0] is string)
            {
                string victimName = args[0] as string;

                if (victimName.Length > 0)
                {
                    eventMsg.Append("\"");
                    eventMsg.Append(victimName);
                    eventMsg.Append("\"님을 공격하고 있습니다!");
                }
                else
                {
                    eventMsg.Append("영토를 확장하고 있습니다!");
                }
            }
            else
            {
                eventMsg.Append("무언가를 공격하고 있습니다!");
            }

            this.UI.AddEventMessage(eventMsg.ToString(), Color.Red);
        }

        private void WhenSendPowerEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            // 이펙트 발생
            this.AddCircleEffect(tileX, tileY, 1, 32, 4, Color.White);


            // 이벤트 메세지 추가
            if (args != null && args.Length >= 2)
            {
                string receiverName = args[0] as string;
                int? sendingPower = args[1] as int?;

                if (sendingPower != null && sendingPower >= GameValues.MinPowerToEvent)
                {
                    StringBuilder eventMsg = new StringBuilder("\"");
                    eventMsg.Append(activeUser);
                    eventMsg.Append("\"님이 ");

                    if (receiverName != activeUser)
                    {
                        if (receiverName.Length <= 0)
                        {
                            eventMsg.Append("빈 땅에게 ");
                        }
                        else
                        {
                            eventMsg.Append("\"");
                            eventMsg.Append(receiverName);
                            eventMsg.Append("\"님에게 ");
                        }
                    }
                    else
                    {
                        eventMsg.Append("자신의 영토로 ");
                    }

                    if (sendingPower >= GameValues.MinHugePowerToEvent)
                    {
                        eventMsg.Append("무려 ");
                    }

                    eventMsg.Append(sendingPower);
                    eventMsg.Append(" 만큼의 힘을 전달했습니다.");

                    this.UI.AddEventMessage(eventMsg.ToString(), Color.Orange);
                }
            }
        }

        private void WhenEditTileSignEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            if (args != null && args.Length >= 1)
            {
                string sign = args[0] as string;


                // 푯말이 있으면
                if (sign.Length > 0)
                {
                    // 이펙트 발생
                    this.AddCircleEffect(tileX, tileY, 4, 128, 8, Color.White);


                    // 이벤트 메세지 추가
                    StringBuilder eventMsg = new StringBuilder("<");
                    eventMsg.Append(activeUser);
                    eventMsg.Append("> : \"");
                    eventMsg.Append(sign);
                    eventMsg.Append("\"");

                    this.UI.AddEventMessage(eventMsg.ToString(), Color.Black);
                }
            }
        }

        private void WhenEndUserEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            // 이펙트 발생
            this.AddCircleEffect(tileX, tileY, 4, 4096, 32, Color.DarkRed);


            // 이벤트 메세지 추가
            StringBuilder eventMsg = new StringBuilder("\"");
            eventMsg.Append(activeUser);
            eventMsg.Append("\"님이 멸망했습니다.");

            this.UI.AddEventMessage(eventMsg.ToString(), Color.DarkRed);
        }

        private void WhenConvertAllResourceEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            // 이펙트 발생
            this.AddCircleEffect(tileX, tileY, 4, 512, 32, Color.Yellow);
        }

        //------------------------------------------------------------------------------------

        private void WhenOtherUserLoginEvent(string userName, object[] args)
        {
            // 이벤트 메세지 추가
            StringBuilder eventMsg = new StringBuilder("\"");
            eventMsg.Append(userName);
            eventMsg.Append("\"님이 접속하셨습니다.");

            this.UI.AddEventMessage(eventMsg.ToString(), Color.YellowGreen);
        }

        private void WhenOtherUserLogoutEvent(string userName, object[] args)
        {
            // 이벤트 메세지 추가
            StringBuilder eventMsg = new StringBuilder("\"");
            eventMsg.Append(userName);
            eventMsg.Append("\"님이 접속을 종료하셨습니다.");

            this.UI.AddEventMessage(eventMsg.ToString(), Color.GreenYellow);
        }

        //------------------------------------------------------------------------------------

        private void WhenReceiveMailEvent(string userName, object[] args)
        {
            if (args.Length >= 1 && args[0] is Mail)
            {
                Mail mail = args[0] as Mail;


                if (mail != null)
                {
                    // 읽지않은 메일이면
                    if (mail.Read == false)
                    {
                        // 이벤트 메세지 추가
                        StringBuilder eventMsg = new StringBuilder("\"");
                        eventMsg.Append(userName);
                        eventMsg.Append("\"님으로부터의 읽지 않은 메일이 있습니다.");

                        this.UI.AddEventMessage(eventMsg.ToString(), Color.LightSkyBlue);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------

        private void WhenBuildCompanyEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            // 이펙트 발생
            this.AddCircleEffect(tileX, tileY, 2, 64, 4, Color.YellowGreen);


            if (args.Length >= 1 && args[0] is string)
            {
                string companyName = args[0] as string;


                // 이벤트 메세지 추가
                StringBuilder eventMsg = new StringBuilder("\"");
                eventMsg.Append(activeUser);
                eventMsg.Append("\"님이 \"");
                eventMsg.Append(companyName);
                eventMsg.Append("\" 회사를 건설하셨습니다.");

                this.UI.AddEventMessage(eventMsg.ToString(), Color.YellowGreen);
            }
        }

        private void WhenDestroyCompanyEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            // 이펙트 발생
            this.AddCircleEffect(tileX, tileY, 2, 64, 4, Color.YellowGreen);


            if (args.Length >= 1 && args[0] is string)
            {
                string companyName = args[0] as string;


                // 이벤트 메세지 추가
                StringBuilder eventMsg = new StringBuilder("\"");
                eventMsg.Append(activeUser);
                eventMsg.Append("\"님이 \"");
                eventMsg.Append(companyName);
                eventMsg.Append("\" 회사 건물 하나를 폐쇄하셨습니다.");

                this.UI.AddEventMessage(eventMsg.ToString(), Color.YellowGreen);
            }
        }

        private void WhenBuyTechEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            if (args.Length >= 2
                && args[0] is string && args[1] is int)
            {
                string sellerName = activeUser;
                string techName = args[0] as string;
                int? profit = args[1] as int?;


                // 이벤트 메세지 추가
                StringBuilder eventMsg = new StringBuilder("\"");
                eventMsg.Append(sellerName);
                eventMsg.Append("\" 회사가 \"");
                eventMsg.Append(techName);
                eventMsg.Append("\" 판매로 ");
                eventMsg.Append(profit);
                eventMsg.Append("◎ 자원을 얻었습니다.");

                this.UI.AddEventMessage(eventMsg.ToString(), Color.Yellow);
            }
        }

        private void WhenBuyProductEvent(int tileX, int tileY, string activeUser, object[] args)
        {
            if (args.Length >= 2
                && args[0] is string && args[1] is int)
            {
                string sellerName = activeUser;
                string productName = args[0] as string;
                int? profit = args[1] as int?;


                // 이벤트 메세지 추가
                StringBuilder eventMsg = new StringBuilder("\"");
                eventMsg.Append(sellerName);
                eventMsg.Append("\" 회사가 \"");
                eventMsg.Append(productName);
                eventMsg.Append("\" 판매로 ");
                eventMsg.Append(profit);
                eventMsg.Append("◎ 자원을 얻었습니다.");

                this.UI.AddEventMessage(eventMsg.ToString(), Color.Yellow);
            }
        }

        //#####################################################################################

        private void WhenReceiveCountryLocation(bool exist, int tileX, int tileY)
        {
            if (exist)
            {
                m_clickMode = ClickModes.Normal;

                // 해당 지역으로 화면 옮기기
                this.BoardView.MoveScreenTo(tileX, tileY);

                // 플레이어 땅 정보 요청
                this.Client.GameBoardDirector.RequestMyTerritory();

                // 플레이어 시야 정보 요청
                this.Client.GameBoardDirector.RequestMyVision();
            }
            else
            {
                // 건국할 지역 요청
                this.Client.GameBoardDirector.RequestNewTerritory(this.WhenReceiveNewTerritory);
            }
        }

        private void WhenReceiveNewTerritory(int tileX, int tileY)
        {
            m_clickMode = ClickModes.BuildCountry;

            // 해당 지역으로 화면 옮기기
            this.BoardView.MoveScreenTo(tileX, tileY);

            // 해당 지역의 땅 정보 요청
            this.Client.GameBoardDirector.UpdateChunkContainsTileAt(tileX, tileY);
        }

        private void WhenMyCountryBuilt(bool bSuccess, int tileX, int tileY)
        {
            if (bSuccess)
            {
                m_clickMode = ClickModes.Normal;

                this.BoardView.MoveScreenTo(tileX, tileY);
            }
            else
            {
                m_clickMode = ClickModes.BuildCountry;
            }
        }

        private void WhenReceiveUserColor(string userName, Color userColor)
        {
            this.BoardView.SetUserColor(userName, userColor);
        }
    }
}
