using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClickWar2.Game.Network;
using ClickWar2.Game.View;
using ClickWar2.Game.Presenter;

namespace ClickWar2_Client
{
    public partial class Form_Main : Form, IStringReceiver, IMailReceiver, ICopApplicationFormReceiver
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        //#####################################################################################

        public Form_Main(GameClient client)
        {
            // 더블 버퍼링
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            this.MouseWheel += Form_Main_MouseWheel;

            InitializeComponent();
            

            this.Reset(client);
        }

        //#####################################################################################
        // Model

        protected GameClient m_client = null;

        //#####################################################################################
        // View

        protected BoardView m_view = new BoardView();
        protected GameInterface m_ui = new GameInterface();
        protected EffectView m_effectView = new EffectView();
        protected ClickWar2.Game.View.ContextMenu m_contextMenu = new ClickWar2.Game.View.ContextMenu();

        //#####################################################################################
        // Presenter

        protected ClientBoardPresenter m_presenter = new ClientBoardPresenter();

        //#####################################################################################

        protected bool m_lockLongClickTimer = false;

        //#####################################################################################

        protected void ExitGame()
        {
            this.timer_slowUpdate.Stop();
            this.timer_update.Stop();
            this.timer_whenLongClick.Stop();


            // 클라이언트 접속 해제
            if (m_client != null)
            {
                m_client.Disconnect();
                m_client = null;
            }


            // 프로그램 종료
            Application.Exit();
        }

        protected void FocusOnGame()
        {
            this.Focus();
            this.Select();
            this.label_userName.Focus();
            this.label_userName.Select();
        }

        protected void ReleaseInput()
        {
            this.timer_whenLongClick.Stop();

            m_view.OnMouseLeftUp();
            m_view.OnMouseRightUp();
        }

        protected void UpdateCompanyTechList(string company)
        {
            int companySiteCount = m_client.UserDataDirector.GetMyCompanySiteCount(company);
            var techList = m_client.UserDataDirector.GetMyCompanyTechList(company);


            // 개수 표시 갱신
            int techCount = (techList == null) ? 0 : techList.Count;
            int maxTechCount = companySiteCount * ClickWar2.Game.GameValues.CompanyTechSizePerSite;

            if (techCount > maxTechCount)
                this.label_techRate.ForeColor = Color.Red;
            else
                this.label_techRate.ForeColor = Color.Black;

            this.label_techRate.Text = string.Format("{0}/{1}",
                techCount, maxTechCount);


            // 목록 갱신
            this.listBox_techList.Items.Clear();

            if (techList != null)
            {
                for (int i = 0; i < techList.Count; ++i)
                {
                    this.listBox_techList.Items.Add(techList[i]);
                }
            }
        }

        protected void UpdateCompanyProductList(string company)
        {
            int companySiteCount = m_client.UserDataDirector.GetMyCompanySiteCount(company);
            var productList = m_client.UserDataDirector.GetMyCompanyProductList(company);


            // 개수 표시 갱신
            int productCount = (productList == null) ? 0 : productList.Count;
            int maxProductCount = companySiteCount * ClickWar2.Game.GameValues.CompanyProductSizePerSite;

            if (productCount > maxProductCount)
                this.label_productRate.ForeColor = Color.Red;
            else
                this.label_productRate.ForeColor = Color.Black;

            this.label_productRate.Text = string.Format("{0}/{1}",
                productCount, maxProductCount);
            

            // 목록 갱신
            this.listBox_productList.Items.Clear();

            if (productList != null)
            {
                for (int i = 0; i < productList.Count; ++i)
                {
                    this.listBox_productList.Items.Add(productList[i]);
                }
            }
        }

        protected void SyncCompanyInformation()
        {
            // 자신의 회사 목록 요청
            m_client.UserDataDirector.RequestMyAllCompanyName(null);

            // 자신의 회사별 건물 개수 요청
            m_client.UserDataDirector.RequestMyAllCompanySiteCount();

            // 자신의 회사별 기술 정보 요청
            m_client.UserDataDirector.RequestMyAllCompanyTechList();

            // 자신의 회사별 제품 정보 요청
            m_client.UserDataDirector.RequestMyAllCompanyProductList();
        }

        protected void UpdateSlidePanel(bool bShow)
        {
            // 표시여부 설정
            this.panel_topSlideScreen.Visible = bShow;


            // 가로 위치 계산
            int panelX = this.ClientSize.Width - this.panel_topSlideScreen.Width - 8;
            int buttonX = panelX + this.panel_topSlideScreen.Width / 2 - this.button_slideTop.Width / 2;


            // 상대적 세로 위치 계산
            int deltaY = 0;

            if (this.panel_topSlideScreen.Visible)
                deltaY = this.panel_topSlideScreen.Height;


            // 위치 갱신
            this.panel_topSlideScreen.Location = new Point(panelX,
                    this.panel_topScreen.Height - this.panel_topSlideScreen.Height + deltaY);
            this.button_slideTop.Location = new Point(buttonX,
                this.panel_topScreen.Height + deltaY);


            // 보이게 되었으면
            if (this.panel_topSlideScreen.Visible)
            {
                // 내용 갱신

                this.UpdateCompanyNameList();
            }
        }

        protected void UpdateCompanyNameList()
        {
            List<string> companyList = m_client.UserDataDirector.Me.Companies;

            this.comboBox_companyList.Items.Clear();
            this.comboBox_companyList.Items.AddRange(companyList.ToArray());
        }

        protected void UpdateCompanyProductListForBuild(string company)
        {
            int oldSelection = this.listBox_productListForBuild.SelectedIndex;

            this.listBox_productListForBuild.Items.Clear();

            var productList = m_client.UserDataDirector.GetMyCompanyProductList(company);
            if (productList != null)
            {
                this.listBox_productListForBuild.Items.AddRange(productList.ToArray());

                if (oldSelection >= this.listBox_productListForBuild.Items.Count)
                    oldSelection = this.listBox_productListForBuild.Items.Count - 1;

                if (oldSelection >= 0)
                {
                    this.listBox_productListForBuild.SelectedIndex = oldSelection;
                }
            }
        }

        protected void DevelopTech(string companyName,
            List<string> techList = null, string beginTechName = "", string beginCode = "")
        {
            if (techList == null)
            {
                techList = m_client.UserDataDirector.GetMyCompanyTechList(companyName);

                if (techList == null)
                    return;
            }


            bool confirmed = false;
            string name = "";
            List<ClickWar2.Game.Command> program = null;

            using (var form = new Form_DevTech())
            {
                form.TechName = beginTechName;
                form.Code = beginCode;

                form.ShowDialog();

                confirmed = form.Confirmed;
                name = form.TechName;
                program = form.Program;
            }


            if (confirmed
                && name.Length > 0
                && program != null)
            {
                int devFee = program.Count * ClickWar2.Game.GameValues.DevFeePerProgramLine;

                // 개발 비용이 있고
                if (m_client.UserDataDirector.Me.Resource >= devFee)
                {
                    // 기술명이 중복되지 않으면
                    if (techList.Any(techName => techName == name) == false)
                    {
                        // 기술 등록 요청
                        m_view.OnDevelopTech(companyName, name, program,
                            this.WhenDevelopTech);
                    }
                    else
                    {
                        MessageBox.Show("이미 존재하는 기술명 입니다.", "Warning!",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    // 개발비용이 부족하면
                    MessageBox.Show("개발에 필요한 자원이 부족합니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        protected void UpdateTechStore()
        {
            this.listView_techStore.BeginUpdate();
            this.listView_techStore.Items.Clear();


            var techStore = m_client.CompanyDirector.TechStore;

            foreach (var tech in techStore)
            {
                ListViewItem item = new ListViewItem(tech.Name);
                item.SubItems.Add(tech.Price.ToString());
                item.SubItems.Add(tech.Seller);

                this.listView_techStore.Items.Add(item);
            }


            this.listView_techStore.EndUpdate();


            // 다음을 위해 전체정보 요청 및 초기화
            m_client.CompanyDirector.TechStore.Clear();
            m_client.CompanyDirector.RequestAllSellingTech();
        }

        protected void UpdateProductStore()
        {
            this.listView_productStore.BeginUpdate();
            this.listView_productStore.Items.Clear();


            var productStore = m_client.CompanyDirector.ProductStore;

            foreach (var product in productStore)
            {
                ListViewItem item = new ListViewItem(product.Name);
                item.SubItems.Add(product.Price.ToString());
                item.SubItems.Add(product.Seller);

                this.listView_productStore.Items.Add(item);
            }


            this.listView_productStore.EndUpdate();


            // 다음을 위해 전체정보 요청 및 초기화
            m_client.CompanyDirector.ProductStore.Clear();
            m_client.CompanyDirector.RequestAllSellingProduct();
        }

        //#####################################################################################

        private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ExitGame();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1024, 768);


            this.panel_topScreen.Size = new Size(this.ClientSize.Width, this.panel_topScreen.Size.Height);

            this.panel_bottomScreen.Size = new Size(this.ClientSize.Width, this.panel_bottomScreen.Size.Height);
            this.panel_bottomScreen.Location = new Point(this.panel_bottomScreen.Location.X,
                this.ClientSize.Height - this.panel_bottomScreen.Height);

            this.UpdateSlidePanel(false);
        }

        public void Reset(GameClient client)
        {
            this.listBox_productList.Items.Clear();
            this.listBox_techList.Items.Clear();
            this.listView_mailbox.Items.Clear();
            this.listView_userList.Items.Clear();
            this.listBox_productListForBuild.Items.Clear();

            this.FocusOnGame();
            this.panel_menu.Visible = false;
            this.panel_user.Visible = false;
            this.panel_mailbox.Visible = false;
            this.panel_manageCompany.Visible = false;
            this.panel_techStore.Visible = false;
            this.UpdateSlidePanel(false);


            // 모델, 뷰, 프리젠터 생성
            m_view = new BoardView();
            m_ui = new GameInterface();
            m_effectView = new EffectView();


            m_view.ScreenSize = this.ClientSize;
            

            // 모델, 뷰, 프리젠터 참조 설정
            m_presenter.BoardView = m_view;
            m_presenter.UI = m_ui;
            m_presenter.EffectDirector = m_effectView;
            m_presenter.ContextMenu = m_contextMenu;


            // 프리젠터 이벤트 연결
            m_presenter.InputCompanyNameDelegate = this.WhenSelectCompanyName;
            m_presenter.InputSelectedCompanyNameDelegate = this.GetSelectedCompanyName;
            m_presenter.InputSelectedCompanyProductDelegate = this.GetSelectedCompanyProductForBuild;


            // 이전 클라이언트 접속 해제 후 새 클라이언트로 설정
            if (m_client != null)
            {
                m_client.Disconnect();
            }

            m_client = client;


            // 클라이언트 이벤트 등록
            client.WhenDisconnectedAgainstExpectation += WhenDisconnectedAgainstExpectation;

            m_client.CompanyDirector.WhenCompanyProductListChanged = this.WhenCompanyProductListChanged;
            m_client.CompanyDirector.WhenCompanyTechListChanged = this.WhenCompanyTechListChanged;

            m_client.NoticeDirector.WhenCheckUser = WhenCheckUser;


            // 프리젠터 설정 후 초기화
            m_presenter.Client = client;

            m_presenter.Initialize();


            // 유저 정보 요청
            m_client.UserDataDirector.RequestAllUserInfo(this.WhenReceiveAllUserInfo);

            // 메일함 정보 요청
            m_client.TalkDirector.RequestMailbox(this.WhenReceiveMail);

            // 회사 관련 정보 요청
            this.SyncCompanyInformation();

            // 기술 상점 정보 요청
            m_client.CompanyDirector.RequestAllSellingTech();

            // 제품 상점 정보 요청
            m_client.CompanyDirector.RequestAllSellingProduct();


            m_lockLongClickTimer = false;


            // 갱신 타이머 시작
            this.timer_update.Start();
            this.timer_slowUpdate.Start();
        }

        private void WhenDisconnectedAgainstExpectation()
        {
            MessageBox.Show("서버와의 연결이 끊어졌습니다.", "Error!",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void WhenCheckUser(int data)
        {
            m_lockLongClickTimer = true;
            this.timer_slowUpdate.Stop();

            // 매크로 테스트 시작
            using (var form = new Form_BlockMacro())
            {
                form.ShowDialog();

                // 테스트 통과 못함
                if (form.Pass == false)
                {
                    // 게임 종료
                    this.ExitGame();
                }
            }

            this.ReleaseInput();
            this.timer_slowUpdate.Start();
        }

        //#####################################################################################

        private void Form_Main_Paint(object sender, PaintEventArgs e)
        {
            // 게임판
            m_view.DrawBoard(m_client.GameBoard, this.ClientRectangle, false, e.Graphics);

            // 이펙트
            m_effectView.UpdateAndDrawEffect(e.Graphics);

            // 이벤트 발생 기록
            m_ui.UpdateAndDrawEventList(e.Graphics, 8, m_ui.EventListMaxViewHeight + 8);

            // 드래그 보조 UI
            m_view.DrawDragHelper(e.Graphics);

            // 쿨타임
            m_ui.DrawCooltime(e.Graphics);

            // 커서 세부 정보
            m_ui.DrawCursorTileDetail(e.Graphics);

            // 컨텍스트 메뉴
            m_contextMenu.Draw(e.Graphics);
        }

        //#####################################################################################
        
        private void timer_update_Tick(object sender, EventArgs e)
        {
            // 네트워크 통신량 표시 갱신
            this.label_networkState.Text = string.Format("{0}↓ {1}↑",
                m_client.ReceiveBufferSize, m_client.SendBufferSize);


            // 한 프레임에 더 많은 처리를 하도록 함.
            int receivedCount = Math.Min(m_client.ReceiveBufferSize, 128);
            for (int i = 0; i < receivedCount; ++i)
            {
                m_client.Update();
            }


            m_presenter.Update();


            // 화면 갱신
            this.Invalidate();
        }

        private void timer_slowUpdate_Tick(object sender, EventArgs e)
        {
            // NOTE: 지속적인 포커스가 필요한 작업시에 이 타이머는 중지되며 닫힐때 다시 작동한다.


            // 유저 정보 요청
            m_client.UserDataDirector.RequestAllUserInfo(this.WhenReceiveAllUserInfo);


            // 열려있는 패널이 없으면
            if (!(this.panel_mailbox.Visible
                || this.panel_manageCompany.Visible
                || this.panel_menu.Visible
                || this.panel_user.Visible
                || this.panel_topSlideScreen.Visible
                || this.panel_techStore.Visible))
            {
                // 메인 화면에 입력포커스 설정
                this.FocusOnGame();
            }
        }

        //#####################################################################################
        // 사용자 입력

        protected bool IsTopWindow()
        {
            IntPtr topWin = GetForegroundWindow();

            return (this.Handle == topWin);
        }

        private void Form_Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.IsTopWindow())
            {
                m_view.OnKeyDown(e.KeyCode);
            }

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (this.panel_mailbox.Visible)
                    {
                        this.panel_mailbox.Visible = false;

                        this.FocusOnGame();
                    }
                    else
                    {
                        this.panel_menu.Location = new Point((this.ClientSize.Width - this.panel_menu.Width) / 2,
                            (this.ClientSize.Height - this.panel_menu.Height) / 2);
                        this.panel_menu.Visible = !this.panel_menu.Visible;

                        if (this.panel_menu.Visible == false)
                        {
                            this.FocusOnGame();
                        }
                    }
                    break;

                case Keys.Return:
                    this.ShowSignInputDlg();
                    break;
            }
        }

        private void Form_Main_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    // 메인 메뉴가 열려있지 않으면
                    if (this.panel_menu.Visible == false)
                    {
                        this.panel_user.Location = new Point((this.ClientSize.Width - this.panel_user.Width) / 2,
                            (this.ClientSize.Height - this.panel_user.Height) / 2);
                        this.panel_user.Visible = !this.panel_user.Visible;
                    }

                    this.FocusOnGame();
                    break;
            }
        }

        private void listView_userList_KeyDown(object sender, KeyEventArgs e)
        {
            this.Form_Main_KeyDown(sender, e);
        }

        private void listView_userList_KeyUp(object sender, KeyEventArgs e)
        {
            this.Form_Main_KeyUp(sender, e);
        }

        private void Form_Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.IsTopWindow())
            {
                if (e.Button == MouseButtons.Left)
                {
                    m_view.OnMouseLeftDown(e.Location);


                    if (!this.timer_whenLongClick.Enabled)
                        this.timer_whenLongClick.Start();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    m_view.OnMouseRightDown(e.Location);
                }
            }
        }

        private void Form_Main_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_view.OnMouseLeftUp(e.Location);


                if (this.timer_whenLongClick.Enabled)
                    this.timer_whenLongClick.Stop();
            }
            else if (e.Button == MouseButtons.Right)
            {
                m_view.OnMouseRightUp(e.Location);
            }
        }

        private void Form_Main_MouseMove(object sender, MouseEventArgs e)
        {
            m_view.OnMouseMove(e.Location);
        }

        private void Form_Main_Resize(object sender, EventArgs e)
        {
            m_view.ScreenSize = this.ClientSize;

            this.panel_topScreen.Size = new Size(this.ClientSize.Width, this.panel_topScreen.Size.Height);

            this.panel_bottomScreen.Size = new Size(this.ClientSize.Width, this.panel_bottomScreen.Size.Height);
            this.panel_bottomScreen.Location = new Point(this.panel_bottomScreen.Location.X,
                this.ClientSize.Height - this.panel_bottomScreen.Height);

            this.UpdateSlidePanel(this.panel_topSlideScreen.Visible);
        }

        private void Form_Main_MouseWheel(object sender, MouseEventArgs e)
        {
            m_view.OnMouseWheelScroll(e.Delta);
        }

        public void ReceiveInputText(string inputText)
        {
            m_view.OnInputText(inputText);
        }

        private void timer_whenLongClick_Tick(object sender, EventArgs e)
        {
            this.timer_whenLongClick.Stop();


            if (m_lockLongClickTimer)
            {
                m_lockLongClickTimer = false;
            }
            else
            {
                m_view.OnLongLeftClick(m_view.Cursor);
            }
        }

        private void ShowSignInputDlg()
        {
            Point tilePos = m_view.TileCursor;

            if (m_client.GameBoard.Board.ContainsItemAt(tilePos.X, tilePos.Y))
            {
                string sign = m_client.GameBoard.GetSignAt(tilePos.X, tilePos.Y);

                using (Form_StringInput inputDlg = new Form_StringInput(this, ClickWar2.Game.GameValues.MaxSignLength))
                {
                    this.timer_slowUpdate.Stop();

                    inputDlg.SetText(sign);
                    inputDlg.ShowDialog();

                    this.timer_slowUpdate.Start();
                }


                this.FocusOnGame();


                // NOTE: 마우스가 눌린 상태에서 새 창을 열었다가 닫고나면 Up 이벤트가 호출되지 않으므로
                // 강제로 호출해준다.
                this.ReleaseInput();
            }
        }

        protected string WhenSelectCompanyName()
        {
            string result = "";


            m_lockLongClickTimer = true;


            this.timer_slowUpdate.Stop();

            List<string> companyList = m_client.UserDataDirector.Me.Companies;

            using (var form = new Form_SelectString(companyList.ToArray()))
            {
                form.Notice = string.Format("건설비용 : {0}◎", ClickWar2.Game.GameValues.CompanyBuildFee);

                form.ShowDialog();

                result = form.SelectedString;
            }

            this.timer_slowUpdate.Start();


            this.ReleaseInput();


            return result;
        }

        protected string GetSelectedCompanyName()
        {
            return this.comboBox_companyList.Text;
        }

        protected int GetSelectedCompanyProductForBuild()
        {
            return this.listBox_productListForBuild.SelectedIndex;
        }

        //#####################################################################################
        // 메뉴

        private void button_closeMenu_Click(object sender, EventArgs e)
        {
            this.panel_menu.Visible = false;


            this.FocusOnGame();
        }

        private void button_logout_Click(object sender, EventArgs e)
        {
            // 갱신 타이머 중지
            this.timer_update.Stop();
            this.timer_whenLongClick.Stop();
            this.timer_slowUpdate.Stop();


            // 클라이언트 접속 해제
            if (m_client != null)
            {
                m_client.Disconnect();
                m_client = null;
            }


            // 접속화면 띄우기
            this.Hide();

            foreach (Form form in Application.OpenForms)
            {
                if (form is Form_Connect)
                {
                    (form as Form_Connect).Reset();
                    form.Show();
                    return;
                }
            }

            var nextForm = new Form_Connect();
            nextForm.Reset();
            nextForm.Show();
        }

        private void button_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //#####################################################################################
        // 유저 정보 목록

        private void WhenReceiveAllUserInfo()
        {
            var userList = m_client.UserDataDirector.UserList;

            userList.Sort(delegate (ClickWar2.Game.GamePlayer p1, ClickWar2.Game.GamePlayer p2)
            {
                return (p2.AreaCount - p1.AreaCount);
            });


            this.listView_userList.BeginUpdate();

            this.listView_userList.Items.Clear();

            int rank = 1;
            foreach (var user in userList)
            {
                // 현재 플레이어의 정보이면
                if (user.Name == m_client.SignDirector.LoginName)
                {
                    // 상단 정보영역에도 표시

                    this.label_userName.Text = string.Format("\"{0}\"", user.Name);
                    this.label_userAreaCount.Text = string.Format("□{0}", user.AreaCount);
                    this.label_userResource.Text = string.Format("◎{0}", user.Resource);
                }


                ListViewItem item = new ListViewItem(rank.ToString());
                item.SubItems.Add(user.Name);
                item.SubItems.Add(user.AreaCount.ToString());
                item.SubItems.Add(user.Resource.ToString());

                this.listView_userList.Items.Add(item);

                ++rank;
            }

            this.listView_userList.EndUpdate();
        }

        private void listView_userList_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = this.listView_userList.Columns[e.ColumnIndex].Width;
        }

        //#####################################################################################
        // 하단 영역

        private void button_showMailbox_Click(object sender, EventArgs e)
        {
            // 메인 메뉴가 열려있지 않으면
            if (this.panel_menu.Visible == false)
            {
                this.panel_mailbox.Location = new Point((this.ClientSize.Width - this.panel_mailbox.Width) / 2,
                    (this.ClientSize.Height - this.panel_mailbox.Height) / 2);
                this.panel_mailbox.Visible = !this.panel_mailbox.Visible;
            }


            this.FocusOnGame();
            this.ReleaseInput();
        }

        private void button_writeMail_Click(object sender, EventArgs e)
        {
            this.timer_slowUpdate.Stop();

            using (Form_WriteMail form = new Form_WriteMail(this))
            {
                List<string> userNames = new List<string>();

                var userList = m_client.UserDataDirector.UserList;

                foreach (var user in userList)
                {
                    userNames.Add(user.Name);
                }

                form.SetUserList(userNames.ToArray());


                form.ShowDialog();
            }

            this.timer_slowUpdate.Start();


            this.ReleaseInput();
        }

        public void ReceiveInputMail(string targetName, string message)
        {
            m_view.OnInputMail(targetName, message);
        }

        private void button_registerCompany_Click(object sender, EventArgs e)
        {
            this.timer_slowUpdate.Stop();

            using (var form = new Form_WriteCopApplicationForm(this))
            {
                form.Notice = string.Format("등록비용 : {0}◎", ClickWar2.Game.GameValues.CompanyRegistrationFee);
                form.MaxNameLength = ClickWar2.Game.GameValues.MaxCompanyNameLength;

                form.ShowDialog();
            }

            this.timer_slowUpdate.Start();


            this.FocusOnGame();
            this.ReleaseInput();
        }

        public void ReceiveInputCopApplicationForm(string name)
        {
            m_view.OnInputCopApplicationForm(name, this.WhenReceiveCopRegistrationResult);
        }

        private void WhenReceiveCopRegistrationResult(RegisterCompanyResults result)
        {
            switch (result)
            {
                case RegisterCompanyResults.Success:
                    MessageBox.Show("등록 성공!", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 자신의 회사 목록이 변경되었을테니 다시 요청
                    m_client.UserDataDirector.RequestMyAllCompanyName(this.WhenRegisterCompany);
                    break;

                case RegisterCompanyResults.Fail_Unauthorized:
                    MessageBox.Show("권한이 없어 등록할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case RegisterCompanyResults.Fail_NotEnoughResource:
                    MessageBox.Show("자원이 부족해 등록할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case RegisterCompanyResults.Fail_AlreadyExist:
                    MessageBox.Show("이름이 중복되어 등록할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case RegisterCompanyResults.Fail_InvalidName:
                    MessageBox.Show("이름이 유효하지 않아 등록할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                default:
                    MessageBox.Show("등록할 수 없습니다.", "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void WhenRegisterCompany()
        {
            this.UpdateCompanyNameList();
        }

        private void button_manageCompany_Click(object sender, EventArgs e)
        {
            if (this.panel_manageCompany.Visible)
            {
                this.panel_manageCompany.Visible = false;
                this.panel_techStore.Visible = false;
            }
            else
            {
                this.timer_slowUpdate.Stop();


                string company = "";


                List<string> companyList = m_client.UserDataDirector.Me.Companies;

                using (var form = new Form_SelectString(companyList.ToArray()))
                {
                    form.Notice = "관리할 회사를 선택하세요.";

                    form.ShowDialog();

                    company = form.SelectedString;
                }


                // 회사가 선택되었고
                // 자신의 회사이면
                if (company.Length > 0
                    && m_client.UserDataDirector.CheckMyCompany(company))
                {
                    // 해당 회사 정보 가져오기
                    int companySiteCount = m_client.UserDataDirector.GetMyCompanySiteCount(company);


                    // 관리창 초기화
                    this.label_companyName.Text = company;
                    this.label_companySiteCount.Text = companySiteCount.ToString() + "개";
                    
                    // 기술 목록 갱신
                    this.UpdateCompanyTechList(company);

                    // 제품 목록 갱신
                    this.UpdateCompanyProductList(company);


                    // 관리창 열기
                    this.panel_manageCompany.Location = new Point((this.ClientSize.Width - this.panel_manageCompany.Width) / 2,
                        (this.ClientSize.Height - this.panel_manageCompany.Height) / 2);
                    this.panel_manageCompany.Visible = true;
                }


                this.timer_slowUpdate.Start();
            }


            this.FocusOnGame();
            this.ReleaseInput();
        }

        private void button_help_Click(object sender, EventArgs e)
        {
            // 도움말 허브 페이지 열기
            System.Diagnostics.Process.Start("http://blog.naver.com/tlsehdgus321/220649272323");
        }

        //#####################################################################################
        // 메일함

        protected void WhenReceiveMail(ClickWar2.Game.Mail mail)
        {
            this.listView_mailbox.BeginUpdate();

            
            ListViewItem item = new ListViewItem(mail.Read ? "◎" : "○");
            item.SubItems.Add(mail.From);

            string previewMsg = mail.Message;
            int endIndex = previewMsg.IndexOf('\n');
            if (endIndex < 0)
                item.SubItems.Add(previewMsg);
            else
                item.SubItems.Add(previewMsg.Remove(endIndex));

            item.SubItems.Add(mail.SendingDate);

            if (mail.Read)
            {
                item.BackColor = Color.Gray;
            }

            this.listView_mailbox.Items.Insert(0, item);


            this.listView_mailbox.EndUpdate();
        }

        protected void ShowSelectedMail()
        {
            var indexList = this.listView_mailbox.SelectedIndices;

            if (indexList.Count > 0)
            {
                int index = indexList[0];


                var mailList = m_client.TalkDirector.Mailbox;

                if (index >= 0 && index < mailList.Count)
                {
                    var mail = mailList[index];

                    if (mail != null)
                    {
                        if (mail.Read == false)
                        {
                            // 읽음 처리
                            mail.Read = true;

                            var selectionList = this.listView_mailbox.SelectedItems;
                            if (selectionList.Count > 0)
                            {
                                ListViewItem selection = selectionList[0];

                                selection.SubItems[0].Text = "◎";
                                selection.BackColor = Color.Gray;
                            }

                            // 읽음처리 요청
                            m_client.TalkDirector.ReadMail(index);
                        }


                        // 메일 전문 보여주기
                        MessageBox.Show(mail.Message, string.Format("From \"{0}\"", mail.From),
                            MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void button_openMail_Click(object sender, EventArgs e)
        {
            this.ShowSelectedMail();   
        }

        private void button_closeMailbox_Click(object sender, EventArgs e)
        {
            this.panel_mailbox.Visible = false;


            this.FocusOnGame();
        }

        private void listView_mailbox_DoubleClick(object sender, EventArgs e)
        {
            this.ShowSelectedMail();
        }

        //#####################################################################################
        // 회사 관리 패널

        protected bool TryInputSellingInformation(string beginItemName, out string itemName, out int price,
            out string targetUser)
        {
            bool confirmed;

            using (var form = new Form_InputSellInformation())
            {
                form.ItemName = beginItemName;


                List<string> userNames = new List<string>();

                var userList = m_client.UserDataDirector.UserList;

                foreach (var user in userList)
                {
                    userNames.Add(user.Name);
                }

                form.SetUserList(userNames.ToArray());


                form.ShowDialog();


                confirmed = form.Confirmed;

                if (confirmed)
                {
                    itemName = form.ItemName;
                    price = form.Price;
                    targetUser = form.TargetUser;
                }
                else
                {
                    itemName = string.Empty;
                    price = -1;
                    targetUser = string.Empty;
                }
            }


            return confirmed;
        }

        private void button_closeCompanyManagement_Click(object sender, EventArgs e)
        {
            // 회사 관리창 닫기
            this.panel_manageCompany.Visible = false;

            this.FocusOnGame();
        }

        private void button_buyTech_Click(object sender, EventArgs e)
        {
            string companyName = this.label_companyName.Text;

            var techList = m_client.UserDataDirector.GetMyCompanyTechList(companyName);

            int companySiteCount = m_client.UserDataDirector.GetMyCompanySiteCount(companyName);
            int maxTechSize = companySiteCount * ClickWar2.Game.GameValues.CompanyTechSizePerSite;


            // 여유공간이 있으면
            if (techList != null && techList.Count < maxTechSize)
            {
                // 기술 상점창 열기
                this.panel_techStore.Location = new Point((this.ClientSize.Width - this.panel_techStore.Width) / 2,
                            (this.ClientSize.Height - this.panel_techStore.Height) / 2);
                this.panel_techStore.Visible = true;

                // 초기화
                this.UpdateTechStore();
            }
            else
            {
                MessageBox.Show("여유공간이 없습니다.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_sellTech_Click(object sender, EventArgs e)
        {
            if (this.listBox_techList.SelectedIndex >= 0)
            {
                string itemName, targetUser;
                int price;

                if (this.TryInputSellingInformation(this.listBox_techList.SelectedItem as string,
                    out itemName, out price, out targetUser))
                {
                    m_view.OnSellTech(this.label_companyName.Text, itemName, price, targetUser);
                }
            }
            else
            {
                MessageBox.Show("판매할 기술을 목록에서 선택하세요.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_devTech_Click(object sender, EventArgs e)
        {
            this.timer_slowUpdate.Stop();


            string companyName = this.label_companyName.Text;

            var techList = m_client.UserDataDirector.GetMyCompanyTechList(companyName);

            int companySiteCount = m_client.UserDataDirector.GetMyCompanySiteCount(companyName);
            int maxTechSize = companySiteCount * ClickWar2.Game.GameValues.CompanyTechSizePerSite;


            // 여유공간이 있으면
            if (techList != null && techList.Count < maxTechSize)
            {
                // 개발 시작
                this.DevelopTech(companyName, techList);
            }
            else
            {
                MessageBox.Show("여유공간이 없습니다.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            this.timer_slowUpdate.Start();
        }

        protected void WhenDevelopTech(DevelopTechResults result)
        {
            switch (result)
            {
                case DevelopTechResults.Success:
                    this.UpdateCompanyTechList(this.label_companyName.Text);
                    MessageBox.Show("개발 성공!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case DevelopTechResults.Fail_Unauthorized:
                    MessageBox.Show("권한이 없어 개발할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case DevelopTechResults.Fail_NotEnoughResource:
                    MessageBox.Show("자원이 부족해 개발할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case DevelopTechResults.Fail_AlreadyExist:
                    MessageBox.Show("기술명이 중복되어 개발할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case DevelopTechResults.Fail_NotEnoughClearance:
                    MessageBox.Show("여유공간이 부족해 개발할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case DevelopTechResults.Fail_InvalidName:
                    MessageBox.Show("기술명이 올바르지 않아 개발할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case DevelopTechResults.Fail_CompanyNotExist:
                    MessageBox.Show("해당 회사가 존재하지 않아 개발할 수 없습니다.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                default:
                    MessageBox.Show("알 수 없는 이유로 개발에 실패하였습니다.", "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void button_buyProduct_Click(object sender, EventArgs e)
        {
            string companyName = this.label_companyName.Text;

            var productList = m_client.UserDataDirector.GetMyCompanyProductList(companyName);

            int companySiteCount = m_client.UserDataDirector.GetMyCompanySiteCount(companyName);
            int maxProductSize = companySiteCount * ClickWar2.Game.GameValues.CompanyProductSizePerSite;


            // 여유공간이 있으면
            if (productList != null && productList.Count < maxProductSize)
            {
                // 제품 상점창 열기
                this.panel_productStore.Location = new Point((this.ClientSize.Width - this.panel_productStore.Width) / 2,
                            (this.ClientSize.Height - this.panel_productStore.Height) / 2);
                this.panel_productStore.Visible = true;

                // 초기화
                this.UpdateProductStore();
            }
            else
            {
                MessageBox.Show("여유공간이 없습니다.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_sellProduct_Click(object sender, EventArgs e)
        {
            if (this.listBox_productList.SelectedIndex >= 0)
            {
                string itemName, targetUser;
                int price;
                int itemIndex = this.listBox_productList.SelectedIndex;

                if (this.TryInputSellingInformation(this.listBox_productList.SelectedItem as string,
                    out itemName, out price, out targetUser)
                    && itemIndex >= 0)
                {
                    m_view.OnSellProduct(this.label_companyName.Text, itemIndex,
                        price, targetUser);
                }
            }
            else
            {
                MessageBox.Show("판매할 제품을 목록에서 선택하세요.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_produceProduct_Click(object sender, EventArgs e)
        {
            string companyName = this.label_companyName.Text;
            var productList = m_client.UserDataDirector.GetMyCompanyProductList(companyName);

            int companySiteCount = m_client.UserDataDirector.GetMyCompanySiteCount(companyName);
            int maxProductSize = companySiteCount * ClickWar2.Game.GameValues.CompanyProductSizePerSite;


            if (productList != null && productList.Count < maxProductSize)
            {
                if (this.listBox_techList.SelectedIndex >= 0)
                {
                    // 제품 생산 요청
                    m_view.OnProduceProduct(companyName, this.listBox_techList.SelectedItem as string,
                        this.WhenCompanyProductListChanged);
                }
                else
                {
                    MessageBox.Show("생산할 기술을 목록에서 선택하세요.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("여유공간이 없습니다.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected void WhenCompanyProductListChanged()
        {
            string companyName = this.label_companyName.Text;

            if (this.panel_manageCompany.Visible)
            {
                this.UpdateCompanyProductList(companyName);
            }

            if (this.comboBox_companyList.Text == companyName)
            {
                this.UpdateCompanyProductListForBuild(companyName);
            }
        }

        protected void WhenCompanyTechListChanged()
        {
            string companyName = this.label_companyName.Text;

            if (this.panel_manageCompany.Visible)
            {
                this.UpdateCompanyTechList(companyName);
            }
        }

        private void ToolStripMenuItem_deleteTech_Click(object sender, EventArgs e)
        {
            if (this.listBox_techList.SelectedIndex >= 0)
            {
                // 기술 폐기 요청
                m_view.OnDiscardTech(this.label_companyName.Text, this.listBox_techList.SelectedItem as string,
                    this.WhenDiscardTech);
            }
        }

        protected void WhenDiscardTech()
        {
            if (this.panel_manageCompany.Visible)
            {
                this.UpdateCompanyTechList(this.label_companyName.Text);
            }
        }

        private void ToolStripMenuItem_deleteProduct_Click(object sender, EventArgs e)
        {
            if (this.listBox_productList.SelectedIndex >= 0)
            {
                // 제품 폐기 요청
                m_view.OnDiscardProduct(this.label_companyName.Text, this.listBox_productList.SelectedIndex,
                    this.WhenCompanyProductListChanged);
            }
        }

        private void ToolStripMenuItem_upgradeTech_Click(object sender, EventArgs e)
        {
            string companyName = this.label_companyName.Text;

            var techList = m_client.UserDataDirector.GetMyCompanyTechList(companyName);

            int companySiteCount = m_client.UserDataDirector.GetMyCompanySiteCount(companyName);
            int maxTechSize = companySiteCount * ClickWar2.Game.GameValues.CompanyTechSizePerSite;


            // 여유공간이 있고
            if (techList != null && techList.Count < maxTechSize)
            {
                // 선택된 기술이 있으면
                if (this.listBox_techList.SelectedIndex >= 0)
                {
                    //this.ToolStripMenuItem_upgradeTech.Enabled = false;

                    // 기술 프로그램 요청
                    m_view.OnRequestTechProgram(companyName, this.listBox_techList.SelectedItem as string,
                        this.WhenReceiveTechProgram);
                }
                else
                {
                    MessageBox.Show("수정할 기술을 목록에서 선택해주세요.", "Warning!",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("여유공간이 없습니다.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected void WhenReceiveTechProgram(string companyName, string techName, string program)
        {
            //this.ToolStripMenuItem_upgradeTech.Enabled = true;

            if (this.label_companyName.Text == companyName
                && techName.Length > 0)
            {
                this.DevelopTech(companyName, null, techName + "+", program);
            }
        }

        //#####################################################################################
        // 상단 슬라이드 패널

        private void button_slideTop_Click(object sender, EventArgs e)
        {
            this.UpdateSlidePanel(!this.panel_topSlideScreen.Visible);

            if (this.panel_topSlideScreen.Visible)
                this.button_slideTop.Text = "▲";
            else
                this.button_slideTop.Text = "▼";


            this.FocusOnGame();
        }

        private void comboBox_companyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateCompanyProductListForBuild(this.comboBox_companyList.Text);
        }

        //#####################################################################################
        // 기술 상점

        private void button_closeTechStore_Click(object sender, EventArgs e)
        {
            // 상점창 닫기
            this.panel_techStore.Visible = false;

            this.FocusOnGame();
        }

        private void button_refreshTechStore_Click(object sender, EventArgs e)
        {
            // 새로고침 버튼 비활성화
            this.button_refreshTechStore.Enabled = false;

            // 상점 동기화
            this.UpdateTechStore();

            // 일정 시간뒤에 버튼 활성화
            Task.Factory.StartNew(() => {
                System.Threading.Thread.Sleep(4000);
                this.button_refreshTechStore.Invoke(new Action(() => this.button_refreshTechStore.Enabled = true));
            });
        }

        private void button_buyTechInStore_Click(object sender, EventArgs e)
        {
            if (this.listView_techStore.SelectedIndices.Count > 0)
            {
                var item = this.listView_techStore.SelectedItems[0];

                string techName = item.SubItems[0].Text;
                string seller = item.SubItems[2].Text;

                int price;
                if (int.TryParse(item.SubItems[1].Text, out price))
                {
                    // 구매 요청
                    m_view.OnBuyTech(seller, techName, price, this.label_companyName.Text);
                }
            }
            else
            {
                MessageBox.Show("구매할 기술을 목록에서 선택하세요.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //#####################################################################################
        // 제품 상점

        private void button_closeProductStore_Click(object sender, EventArgs e)
        {
            // 상점창 닫기
            this.panel_productStore.Visible = false;

            this.FocusOnGame();
        }

        private void button_refreshProductStore_Click(object sender, EventArgs e)
        {
            // 새로고침 버튼 비활성화
            this.button_refreshProductStore.Enabled = false;

            // 상점 동기화
            this.UpdateProductStore();

            // 일정 시간뒤에 버튼 활성화
            Task.Factory.StartNew(() => {
                System.Threading.Thread.Sleep(4000);
                this.button_refreshProductStore.Invoke(new Action(() => this.button_refreshProductStore.Enabled = true));
            });
        }

        private void button_buyProductInStore_Click(object sender, EventArgs e)
        {
            if (this.listView_productStore.SelectedIndices.Count > 0)
            {
                var item = this.listView_productStore.SelectedItems[0];

                string productName = item.SubItems[0].Text;
                string seller = item.SubItems[2].Text;

                int price;
                if (int.TryParse(item.SubItems[1].Text, out price))
                {
                    // 구매 요청
                    m_view.OnBuyProduct(seller, productName, price, this.label_companyName.Text);
                }
            }
            else
            {
                MessageBox.Show("구매할 제품을 목록에서 선택하세요.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
