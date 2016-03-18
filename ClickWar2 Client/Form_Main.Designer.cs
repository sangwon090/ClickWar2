namespace ClickWar2_Client
{
    partial class Form_Main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.timer_update = new System.Windows.Forms.Timer(this.components);
            this.timer_whenLongClick = new System.Windows.Forms.Timer(this.components);
            this.panel_menu = new System.Windows.Forms.Panel();
            this.button_exit = new System.Windows.Forms.Button();
            this.button_logout = new System.Windows.Forms.Button();
            this.button_closeMenu = new System.Windows.Forms.Button();
            this.panel_user = new System.Windows.Forms.Panel();
            this.listView_userList = new System.Windows.Forms.ListView();
            this.columnHeader_rank = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_userName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_territoryCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_resource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timer_slowUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel_topScreen = new System.Windows.Forms.Panel();
            this.label_networkState = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_userResource = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_userAreaCount = new System.Windows.Forms.Label();
            this.label_userName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_bottomScreen = new System.Windows.Forms.Panel();
            this.button_help = new System.Windows.Forms.Button();
            this.button_manageCompany = new System.Windows.Forms.Button();
            this.button_registerCompany = new System.Windows.Forms.Button();
            this.button_writeMail = new System.Windows.Forms.Button();
            this.button_showMailbox = new System.Windows.Forms.Button();
            this.panel_mailbox = new System.Windows.Forms.Panel();
            this.button_closeMailbox = new System.Windows.Forms.Button();
            this.button_openMail = new System.Windows.Forms.Button();
            this.listView_mailbox = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel_manageCompany = new System.Windows.Forms.Panel();
            this.label_companySiteCount = new System.Windows.Forms.Label();
            this.label_productRate = new System.Windows.Forms.Label();
            this.label_techRate = new System.Windows.Forms.Label();
            this.button_closeCompanyManagement = new System.Windows.Forms.Button();
            this.button_produceProduct = new System.Windows.Forms.Button();
            this.button_sellProduct = new System.Windows.Forms.Button();
            this.button_buyProduct = new System.Windows.Forms.Button();
            this.listBox_productList = new System.Windows.Forms.ListBox();
            this.contextMenuStrip_productList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_deleteProduct = new System.Windows.Forms.ToolStripMenuItem();
            this.label6 = new System.Windows.Forms.Label();
            this.button_devTech = new System.Windows.Forms.Button();
            this.button_sellTech = new System.Windows.Forms.Button();
            this.button_buyTech = new System.Windows.Forms.Button();
            this.listBox_techList = new System.Windows.Forms.ListBox();
            this.contextMenuStrip_techList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_deleteTech = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_upgradeTech = new System.Windows.Forms.ToolStripMenuItem();
            this.label5 = new System.Windows.Forms.Label();
            this.label_companyName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel_topSlideScreen = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.listBox_productListForBuild = new System.Windows.Forms.ListBox();
            this.comboBox_companyList = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button_slideTop = new System.Windows.Forms.Button();
            this.panel_techStore = new System.Windows.Forms.Panel();
            this.button_buyTechInStore = new System.Windows.Forms.Button();
            this.listView_techStore = new System.Windows.Forms.ListView();
            this.columnHeader_techName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_techPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_techSeller = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label10 = new System.Windows.Forms.Label();
            this.button_refreshTechStore = new System.Windows.Forms.Button();
            this.button_closeTechStore = new System.Windows.Forms.Button();
            this.panel_productStore = new System.Windows.Forms.Panel();
            this.button_buyProductInStore = new System.Windows.Forms.Button();
            this.listView_productStore = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label11 = new System.Windows.Forms.Label();
            this.button_refreshProductStore = new System.Windows.Forms.Button();
            this.button_closeProductStore = new System.Windows.Forms.Button();
            this.panel_menu.SuspendLayout();
            this.panel_user.SuspendLayout();
            this.panel_topScreen.SuspendLayout();
            this.panel_bottomScreen.SuspendLayout();
            this.panel_mailbox.SuspendLayout();
            this.panel_manageCompany.SuspendLayout();
            this.contextMenuStrip_productList.SuspendLayout();
            this.contextMenuStrip_techList.SuspendLayout();
            this.panel_topSlideScreen.SuspendLayout();
            this.panel_techStore.SuspendLayout();
            this.panel_productStore.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer_update
            // 
            this.timer_update.Interval = 16;
            this.timer_update.Tick += new System.EventHandler(this.timer_update_Tick);
            // 
            // timer_whenLongClick
            // 
            this.timer_whenLongClick.Interval = 500;
            this.timer_whenLongClick.Tick += new System.EventHandler(this.timer_whenLongClick_Tick);
            // 
            // panel_menu
            // 
            this.panel_menu.BackColor = System.Drawing.SystemColors.Control;
            this.panel_menu.Controls.Add(this.button_exit);
            this.panel_menu.Controls.Add(this.button_logout);
            this.panel_menu.Controls.Add(this.button_closeMenu);
            this.panel_menu.Location = new System.Drawing.Point(12, 79);
            this.panel_menu.Name = "panel_menu";
            this.panel_menu.Size = new System.Drawing.Size(256, 189);
            this.panel_menu.TabIndex = 0;
            this.panel_menu.Visible = false;
            // 
            // button_exit
            // 
            this.button_exit.Location = new System.Drawing.Point(16, 124);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(224, 48);
            this.button_exit.TabIndex = 2;
            this.button_exit.Text = "종료";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new System.EventHandler(this.button_exit_Click);
            // 
            // button_logout
            // 
            this.button_logout.Location = new System.Drawing.Point(16, 70);
            this.button_logout.Name = "button_logout";
            this.button_logout.Size = new System.Drawing.Size(224, 48);
            this.button_logout.TabIndex = 1;
            this.button_logout.Text = "로그아웃";
            this.button_logout.UseVisualStyleBackColor = true;
            this.button_logout.Click += new System.EventHandler(this.button_logout_Click);
            // 
            // button_closeMenu
            // 
            this.button_closeMenu.Location = new System.Drawing.Point(16, 16);
            this.button_closeMenu.Name = "button_closeMenu";
            this.button_closeMenu.Size = new System.Drawing.Size(224, 48);
            this.button_closeMenu.TabIndex = 0;
            this.button_closeMenu.Text = "메뉴 닫기";
            this.button_closeMenu.UseVisualStyleBackColor = true;
            this.button_closeMenu.Click += new System.EventHandler(this.button_closeMenu_Click);
            // 
            // panel_user
            // 
            this.panel_user.BackColor = System.Drawing.SystemColors.Control;
            this.panel_user.Controls.Add(this.listView_userList);
            this.panel_user.Location = new System.Drawing.Point(12, 274);
            this.panel_user.Name = "panel_user";
            this.panel_user.Size = new System.Drawing.Size(512, 384);
            this.panel_user.TabIndex = 0;
            this.panel_user.Visible = false;
            // 
            // listView_userList
            // 
            this.listView_userList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_rank,
            this.columnHeader_userName,
            this.columnHeader_territoryCount,
            this.columnHeader_resource});
            this.listView_userList.GridLines = true;
            this.listView_userList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_userList.HoverSelection = true;
            this.listView_userList.Location = new System.Drawing.Point(3, 3);
            this.listView_userList.MultiSelect = false;
            this.listView_userList.Name = "listView_userList";
            this.listView_userList.Size = new System.Drawing.Size(506, 378);
            this.listView_userList.TabIndex = 0;
            this.listView_userList.UseCompatibleStateImageBehavior = false;
            this.listView_userList.View = System.Windows.Forms.View.Details;
            this.listView_userList.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listView_userList_ColumnWidthChanging);
            this.listView_userList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_userList_KeyDown);
            this.listView_userList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_userList_KeyUp);
            // 
            // columnHeader_rank
            // 
            this.columnHeader_rank.Text = "#";
            this.columnHeader_rank.Width = 32;
            // 
            // columnHeader_userName
            // 
            this.columnHeader_userName.Text = "Name";
            this.columnHeader_userName.Width = 226;
            // 
            // columnHeader_territoryCount
            // 
            this.columnHeader_territoryCount.Text = "Area";
            this.columnHeader_territoryCount.Width = 80;
            // 
            // columnHeader_resource
            // 
            this.columnHeader_resource.Text = "Resource";
            this.columnHeader_resource.Width = 91;
            // 
            // timer_slowUpdate
            // 
            this.timer_slowUpdate.Interval = 3000;
            this.timer_slowUpdate.Tick += new System.EventHandler(this.timer_slowUpdate_Tick);
            // 
            // panel_topScreen
            // 
            this.panel_topScreen.BackColor = System.Drawing.SystemColors.Control;
            this.panel_topScreen.Controls.Add(this.label_networkState);
            this.panel_topScreen.Controls.Add(this.label3);
            this.panel_topScreen.Controls.Add(this.label_userResource);
            this.panel_topScreen.Controls.Add(this.label2);
            this.panel_topScreen.Controls.Add(this.label_userAreaCount);
            this.panel_topScreen.Controls.Add(this.label_userName);
            this.panel_topScreen.Controls.Add(this.label1);
            this.panel_topScreen.Location = new System.Drawing.Point(0, 0);
            this.panel_topScreen.Name = "panel_topScreen";
            this.panel_topScreen.Size = new System.Drawing.Size(1123, 35);
            this.panel_topScreen.TabIndex = 1;
            // 
            // label_networkState
            // 
            this.label_networkState.AutoSize = true;
            this.label_networkState.Location = new System.Drawing.Point(1011, 9);
            this.label_networkState.Name = "label_networkState";
            this.label_networkState.Size = new System.Drawing.Size(48, 15);
            this.label_networkState.TabIndex = 6;
            this.label_networkState.Text = "0↓ 0↑";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(661, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Resource :";
            // 
            // label_userResource
            // 
            this.label_userResource.AutoSize = true;
            this.label_userResource.Location = new System.Drawing.Point(747, 9);
            this.label_userResource.Name = "label_userResource";
            this.label_userResource.Size = new System.Drawing.Size(74, 15);
            this.label_userResource.TabIndex = 4;
            this.label_userResource.Text = "Loading...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(349, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Area :";
            // 
            // label_userAreaCount
            // 
            this.label_userAreaCount.AutoSize = true;
            this.label_userAreaCount.Location = new System.Drawing.Point(401, 9);
            this.label_userAreaCount.Name = "label_userAreaCount";
            this.label_userAreaCount.Size = new System.Drawing.Size(74, 15);
            this.label_userAreaCount.TabIndex = 2;
            this.label_userAreaCount.Text = "Loading...";
            // 
            // label_userName
            // 
            this.label_userName.AutoSize = true;
            this.label_userName.Location = new System.Drawing.Point(71, 9);
            this.label_userName.Name = "label_userName";
            this.label_userName.Size = new System.Drawing.Size(74, 15);
            this.label_userName.TabIndex = 1;
            this.label_userName.Text = "Loading...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name :";
            // 
            // panel_bottomScreen
            // 
            this.panel_bottomScreen.BackColor = System.Drawing.SystemColors.Control;
            this.panel_bottomScreen.Controls.Add(this.button_help);
            this.panel_bottomScreen.Controls.Add(this.button_manageCompany);
            this.panel_bottomScreen.Controls.Add(this.button_registerCompany);
            this.panel_bottomScreen.Controls.Add(this.button_writeMail);
            this.panel_bottomScreen.Controls.Add(this.button_showMailbox);
            this.panel_bottomScreen.Location = new System.Drawing.Point(0, 1005);
            this.panel_bottomScreen.Name = "panel_bottomScreen";
            this.panel_bottomScreen.Size = new System.Drawing.Size(1024, 42);
            this.panel_bottomScreen.TabIndex = 6;
            // 
            // button_help
            // 
            this.button_help.Location = new System.Drawing.Point(607, 3);
            this.button_help.Name = "button_help";
            this.button_help.Size = new System.Drawing.Size(134, 29);
            this.button_help.TabIndex = 4;
            this.button_help.Text = "Help";
            this.button_help.UseVisualStyleBackColor = true;
            this.button_help.Click += new System.EventHandler(this.button_help_Click);
            // 
            // button_manageCompany
            // 
            this.button_manageCompany.Location = new System.Drawing.Point(439, 3);
            this.button_manageCompany.Name = "button_manageCompany";
            this.button_manageCompany.Size = new System.Drawing.Size(144, 29);
            this.button_manageCompany.TabIndex = 3;
            this.button_manageCompany.Text = "Manage Company";
            this.button_manageCompany.UseVisualStyleBackColor = true;
            this.button_manageCompany.Click += new System.EventHandler(this.button_manageCompany_Click);
            // 
            // button_registerCompany
            // 
            this.button_registerCompany.Location = new System.Drawing.Point(289, 3);
            this.button_registerCompany.Name = "button_registerCompany";
            this.button_registerCompany.Size = new System.Drawing.Size(144, 29);
            this.button_registerCompany.TabIndex = 2;
            this.button_registerCompany.Text = "Register Company";
            this.button_registerCompany.UseVisualStyleBackColor = true;
            this.button_registerCompany.Click += new System.EventHandler(this.button_registerCompany_Click);
            // 
            // button_writeMail
            // 
            this.button_writeMail.Location = new System.Drawing.Point(137, 3);
            this.button_writeMail.Name = "button_writeMail";
            this.button_writeMail.Size = new System.Drawing.Size(128, 29);
            this.button_writeMail.TabIndex = 1;
            this.button_writeMail.Text = "Write Mail";
            this.button_writeMail.UseVisualStyleBackColor = true;
            this.button_writeMail.Click += new System.EventHandler(this.button_writeMail_Click);
            // 
            // button_showMailbox
            // 
            this.button_showMailbox.Location = new System.Drawing.Point(3, 3);
            this.button_showMailbox.Name = "button_showMailbox";
            this.button_showMailbox.Size = new System.Drawing.Size(128, 29);
            this.button_showMailbox.TabIndex = 0;
            this.button_showMailbox.Text = "Mailbox";
            this.button_showMailbox.UseVisualStyleBackColor = true;
            this.button_showMailbox.Click += new System.EventHandler(this.button_showMailbox_Click);
            // 
            // panel_mailbox
            // 
            this.panel_mailbox.BackColor = System.Drawing.SystemColors.Control;
            this.panel_mailbox.Controls.Add(this.button_closeMailbox);
            this.panel_mailbox.Controls.Add(this.button_openMail);
            this.panel_mailbox.Controls.Add(this.listView_mailbox);
            this.panel_mailbox.Location = new System.Drawing.Point(530, 79);
            this.panel_mailbox.Name = "panel_mailbox";
            this.panel_mailbox.Size = new System.Drawing.Size(766, 384);
            this.panel_mailbox.TabIndex = 3;
            this.panel_mailbox.Visible = false;
            // 
            // button_closeMailbox
            // 
            this.button_closeMailbox.Location = new System.Drawing.Point(447, 350);
            this.button_closeMailbox.Name = "button_closeMailbox";
            this.button_closeMailbox.Size = new System.Drawing.Size(316, 31);
            this.button_closeMailbox.TabIndex = 8;
            this.button_closeMailbox.Text = "Close";
            this.button_closeMailbox.UseVisualStyleBackColor = true;
            this.button_closeMailbox.Click += new System.EventHandler(this.button_closeMailbox_Click);
            // 
            // button_openMail
            // 
            this.button_openMail.Location = new System.Drawing.Point(3, 350);
            this.button_openMail.Name = "button_openMail";
            this.button_openMail.Size = new System.Drawing.Size(316, 31);
            this.button_openMail.TabIndex = 7;
            this.button_openMail.Text = "Read";
            this.button_openMail.UseVisualStyleBackColor = true;
            this.button_openMail.Click += new System.EventHandler(this.button_openMail_Click);
            // 
            // listView_mailbox
            // 
            this.listView_mailbox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader5,
            this.columnHeader6});
            this.listView_mailbox.FullRowSelect = true;
            this.listView_mailbox.GridLines = true;
            this.listView_mailbox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_mailbox.HideSelection = false;
            this.listView_mailbox.Location = new System.Drawing.Point(3, 3);
            this.listView_mailbox.MultiSelect = false;
            this.listView_mailbox.Name = "listView_mailbox";
            this.listView_mailbox.Size = new System.Drawing.Size(760, 341);
            this.listView_mailbox.TabIndex = 1;
            this.listView_mailbox.UseCompatibleStateImageBehavior = false;
            this.listView_mailbox.View = System.Windows.Forms.View.Details;
            this.listView_mailbox.DoubleClick += new System.EventHandler(this.listView_mailbox_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Read";
            this.columnHeader1.Width = 47;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Sender";
            this.columnHeader2.Width = 152;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Preview";
            this.columnHeader5.Width = 287;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Date";
            this.columnHeader6.Width = 206;
            // 
            // panel_manageCompany
            // 
            this.panel_manageCompany.BackColor = System.Drawing.SystemColors.Control;
            this.panel_manageCompany.Controls.Add(this.label_companySiteCount);
            this.panel_manageCompany.Controls.Add(this.label_productRate);
            this.panel_manageCompany.Controls.Add(this.label_techRate);
            this.panel_manageCompany.Controls.Add(this.button_closeCompanyManagement);
            this.panel_manageCompany.Controls.Add(this.button_produceProduct);
            this.panel_manageCompany.Controls.Add(this.button_sellProduct);
            this.panel_manageCompany.Controls.Add(this.button_buyProduct);
            this.panel_manageCompany.Controls.Add(this.listBox_productList);
            this.panel_manageCompany.Controls.Add(this.label6);
            this.panel_manageCompany.Controls.Add(this.button_devTech);
            this.panel_manageCompany.Controls.Add(this.button_sellTech);
            this.panel_manageCompany.Controls.Add(this.button_buyTech);
            this.panel_manageCompany.Controls.Add(this.listBox_techList);
            this.panel_manageCompany.Controls.Add(this.label5);
            this.panel_manageCompany.Controls.Add(this.label_companyName);
            this.panel_manageCompany.Controls.Add(this.label4);
            this.panel_manageCompany.Location = new System.Drawing.Point(530, 469);
            this.panel_manageCompany.Name = "panel_manageCompany";
            this.panel_manageCompany.Size = new System.Drawing.Size(588, 486);
            this.panel_manageCompany.TabIndex = 7;
            this.panel_manageCompany.Visible = false;
            // 
            // label_companySiteCount
            // 
            this.label_companySiteCount.AutoSize = true;
            this.label_companySiteCount.Location = new System.Drawing.Point(444, 12);
            this.label_companySiteCount.Name = "label_companySiteCount";
            this.label_companySiteCount.Size = new System.Drawing.Size(30, 15);
            this.label_companySiteCount.TabIndex = 15;
            this.label_companySiteCount.Text = "0개";
            // 
            // label_productRate
            // 
            this.label_productRate.AutoSize = true;
            this.label_productRate.Location = new System.Drawing.Point(55, 284);
            this.label_productRate.Name = "label_productRate";
            this.label_productRate.Size = new System.Drawing.Size(29, 15);
            this.label_productRate.TabIndex = 14;
            this.label_productRate.Text = "0/0";
            // 
            // label_techRate
            // 
            this.label_techRate.AutoSize = true;
            this.label_techRate.Location = new System.Drawing.Point(55, 62);
            this.label_techRate.Name = "label_techRate";
            this.label_techRate.Size = new System.Drawing.Size(29, 15);
            this.label_techRate.TabIndex = 13;
            this.label_techRate.Text = "0/0";
            // 
            // button_closeCompanyManagement
            // 
            this.button_closeCompanyManagement.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_closeCompanyManagement.Location = new System.Drawing.Point(557, 3);
            this.button_closeCompanyManagement.Name = "button_closeCompanyManagement";
            this.button_closeCompanyManagement.Size = new System.Drawing.Size(28, 24);
            this.button_closeCompanyManagement.TabIndex = 12;
            this.button_closeCompanyManagement.Text = "X";
            this.button_closeCompanyManagement.UseVisualStyleBackColor = true;
            this.button_closeCompanyManagement.Click += new System.EventHandler(this.button_closeCompanyManagement_Click);
            // 
            // button_produceProduct
            // 
            this.button_produceProduct.Location = new System.Drawing.Point(421, 419);
            this.button_produceProduct.Name = "button_produceProduct";
            this.button_produceProduct.Size = new System.Drawing.Size(152, 52);
            this.button_produceProduct.TabIndex = 11;
            this.button_produceProduct.Text = "제품 생산";
            this.button_produceProduct.UseVisualStyleBackColor = true;
            this.button_produceProduct.Click += new System.EventHandler(this.button_produceProduct_Click);
            // 
            // button_sellProduct
            // 
            this.button_sellProduct.Location = new System.Drawing.Point(421, 356);
            this.button_sellProduct.Name = "button_sellProduct";
            this.button_sellProduct.Size = new System.Drawing.Size(152, 52);
            this.button_sellProduct.TabIndex = 10;
            this.button_sellProduct.Text = "제품 판매";
            this.button_sellProduct.UseVisualStyleBackColor = true;
            this.button_sellProduct.Click += new System.EventHandler(this.button_sellProduct_Click);
            // 
            // button_buyProduct
            // 
            this.button_buyProduct.Location = new System.Drawing.Point(421, 302);
            this.button_buyProduct.Name = "button_buyProduct";
            this.button_buyProduct.Size = new System.Drawing.Size(152, 52);
            this.button_buyProduct.TabIndex = 9;
            this.button_buyProduct.Text = "제품 구매";
            this.button_buyProduct.UseVisualStyleBackColor = true;
            this.button_buyProduct.Click += new System.EventHandler(this.button_buyProduct_Click);
            // 
            // listBox_productList
            // 
            this.listBox_productList.ContextMenuStrip = this.contextMenuStrip_productList;
            this.listBox_productList.FormattingEnabled = true;
            this.listBox_productList.HorizontalScrollbar = true;
            this.listBox_productList.ItemHeight = 15;
            this.listBox_productList.Location = new System.Drawing.Point(15, 302);
            this.listBox_productList.Name = "listBox_productList";
            this.listBox_productList.ScrollAlwaysVisible = true;
            this.listBox_productList.Size = new System.Drawing.Size(400, 169);
            this.listBox_productList.TabIndex = 8;
            // 
            // contextMenuStrip_productList
            // 
            this.contextMenuStrip_productList.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_productList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_deleteProduct});
            this.contextMenuStrip_productList.Name = "contextMenuStrip_productList";
            this.contextMenuStrip_productList.Size = new System.Drawing.Size(109, 28);
            // 
            // ToolStripMenuItem_deleteProduct
            // 
            this.ToolStripMenuItem_deleteProduct.Name = "ToolStripMenuItem_deleteProduct";
            this.ToolStripMenuItem_deleteProduct.Size = new System.Drawing.Size(108, 24);
            this.ToolStripMenuItem_deleteProduct.Text = "폐기";
            this.ToolStripMenuItem_deleteProduct.Click += new System.EventHandler(this.ToolStripMenuItem_deleteProduct_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 284);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 15);
            this.label6.TabIndex = 7;
            this.label6.Text = "창고";
            // 
            // button_devTech
            // 
            this.button_devTech.Location = new System.Drawing.Point(421, 197);
            this.button_devTech.Name = "button_devTech";
            this.button_devTech.Size = new System.Drawing.Size(152, 52);
            this.button_devTech.TabIndex = 6;
            this.button_devTech.Text = "기술 개발";
            this.button_devTech.UseVisualStyleBackColor = true;
            this.button_devTech.Click += new System.EventHandler(this.button_devTech_Click);
            // 
            // button_sellTech
            // 
            this.button_sellTech.Location = new System.Drawing.Point(421, 134);
            this.button_sellTech.Name = "button_sellTech";
            this.button_sellTech.Size = new System.Drawing.Size(152, 52);
            this.button_sellTech.TabIndex = 5;
            this.button_sellTech.Text = "기술 판매";
            this.button_sellTech.UseVisualStyleBackColor = true;
            this.button_sellTech.Click += new System.EventHandler(this.button_sellTech_Click);
            // 
            // button_buyTech
            // 
            this.button_buyTech.Location = new System.Drawing.Point(421, 80);
            this.button_buyTech.Name = "button_buyTech";
            this.button_buyTech.Size = new System.Drawing.Size(152, 52);
            this.button_buyTech.TabIndex = 4;
            this.button_buyTech.Text = "기술 구매";
            this.button_buyTech.UseVisualStyleBackColor = true;
            this.button_buyTech.Click += new System.EventHandler(this.button_buyTech_Click);
            // 
            // listBox_techList
            // 
            this.listBox_techList.ContextMenuStrip = this.contextMenuStrip_techList;
            this.listBox_techList.FormattingEnabled = true;
            this.listBox_techList.HorizontalScrollbar = true;
            this.listBox_techList.ItemHeight = 15;
            this.listBox_techList.Location = new System.Drawing.Point(15, 80);
            this.listBox_techList.Name = "listBox_techList";
            this.listBox_techList.ScrollAlwaysVisible = true;
            this.listBox_techList.Size = new System.Drawing.Size(400, 169);
            this.listBox_techList.TabIndex = 3;
            // 
            // contextMenuStrip_techList
            // 
            this.contextMenuStrip_techList.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_techList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_deleteTech,
            this.ToolStripMenuItem_upgradeTech});
            this.contextMenuStrip_techList.Name = "contextMenuStrip_techList";
            this.contextMenuStrip_techList.Size = new System.Drawing.Size(109, 52);
            // 
            // ToolStripMenuItem_deleteTech
            // 
            this.ToolStripMenuItem_deleteTech.Name = "ToolStripMenuItem_deleteTech";
            this.ToolStripMenuItem_deleteTech.Size = new System.Drawing.Size(108, 24);
            this.ToolStripMenuItem_deleteTech.Text = "폐기";
            this.ToolStripMenuItem_deleteTech.Click += new System.EventHandler(this.ToolStripMenuItem_deleteTech_Click);
            // 
            // ToolStripMenuItem_upgradeTech
            // 
            this.ToolStripMenuItem_upgradeTech.Name = "ToolStripMenuItem_upgradeTech";
            this.ToolStripMenuItem_upgradeTech.Size = new System.Drawing.Size(108, 24);
            this.ToolStripMenuItem_upgradeTech.Text = "개선";
            this.ToolStripMenuItem_upgradeTech.Click += new System.EventHandler(this.ToolStripMenuItem_upgradeTech_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "기술";
            // 
            // label_companyName
            // 
            this.label_companyName.AutoSize = true;
            this.label_companyName.Location = new System.Drawing.Point(80, 12);
            this.label_companyName.Name = "label_companyName";
            this.label_companyName.Size = new System.Drawing.Size(74, 15);
            this.label_companyName.TabIndex = 1;
            this.label_companyName.Text = "Loading...";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "회사명 :";
            // 
            // panel_topSlideScreen
            // 
            this.panel_topSlideScreen.BackColor = System.Drawing.SystemColors.Control;
            this.panel_topSlideScreen.Controls.Add(this.label9);
            this.panel_topSlideScreen.Controls.Add(this.label8);
            this.panel_topSlideScreen.Controls.Add(this.listBox_productListForBuild);
            this.panel_topSlideScreen.Controls.Add(this.comboBox_companyList);
            this.panel_topSlideScreen.Controls.Add(this.label7);
            this.panel_topSlideScreen.Location = new System.Drawing.Point(12, 666);
            this.panel_topSlideScreen.Name = "panel_topSlideScreen";
            this.panel_topSlideScreen.Size = new System.Drawing.Size(256, 300);
            this.panel_topSlideScreen.TabIndex = 9;
            this.panel_topSlideScreen.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(97, 42);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(144, 15);
            this.label9.TabIndex = 4;
            this.label9.Text = "*설치할 제품을 선택";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 15);
            this.label8.TabIndex = 3;
            this.label8.Text = "창고";
            // 
            // listBox_productListForBuild
            // 
            this.listBox_productListForBuild.FormattingEnabled = true;
            this.listBox_productListForBuild.HorizontalScrollbar = true;
            this.listBox_productListForBuild.ItemHeight = 15;
            this.listBox_productListForBuild.Location = new System.Drawing.Point(15, 60);
            this.listBox_productListForBuild.Name = "listBox_productListForBuild";
            this.listBox_productListForBuild.ScrollAlwaysVisible = true;
            this.listBox_productListForBuild.Size = new System.Drawing.Size(226, 229);
            this.listBox_productListForBuild.TabIndex = 2;
            // 
            // comboBox_companyList
            // 
            this.comboBox_companyList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox_companyList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox_companyList.FormattingEnabled = true;
            this.comboBox_companyList.Location = new System.Drawing.Point(65, 9);
            this.comboBox_companyList.Name = "comboBox_companyList";
            this.comboBox_companyList.Size = new System.Drawing.Size(176, 23);
            this.comboBox_companyList.TabIndex = 1;
            this.comboBox_companyList.SelectedIndexChanged += new System.EventHandler(this.comboBox_companyList_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "회사 :";
            // 
            // button_slideTop
            // 
            this.button_slideTop.Location = new System.Drawing.Point(97, 972);
            this.button_slideTop.Name = "button_slideTop";
            this.button_slideTop.Size = new System.Drawing.Size(64, 24);
            this.button_slideTop.TabIndex = 10;
            this.button_slideTop.Text = "▼";
            this.button_slideTop.UseVisualStyleBackColor = true;
            this.button_slideTop.Click += new System.EventHandler(this.button_slideTop_Click);
            // 
            // panel_techStore
            // 
            this.panel_techStore.BackColor = System.Drawing.SystemColors.Control;
            this.panel_techStore.Controls.Add(this.button_buyTechInStore);
            this.panel_techStore.Controls.Add(this.listView_techStore);
            this.panel_techStore.Controls.Add(this.label10);
            this.panel_techStore.Controls.Add(this.button_refreshTechStore);
            this.panel_techStore.Controls.Add(this.button_closeTechStore);
            this.panel_techStore.Location = new System.Drawing.Point(1302, 79);
            this.panel_techStore.Name = "panel_techStore";
            this.panel_techStore.Size = new System.Drawing.Size(535, 384);
            this.panel_techStore.TabIndex = 11;
            this.panel_techStore.Visible = false;
            // 
            // button_buyTechInStore
            // 
            this.button_buyTechInStore.Location = new System.Drawing.Point(3, 350);
            this.button_buyTechInStore.Name = "button_buyTechInStore";
            this.button_buyTechInStore.Size = new System.Drawing.Size(529, 31);
            this.button_buyTechInStore.TabIndex = 9;
            this.button_buyTechInStore.Text = "Buy";
            this.button_buyTechInStore.UseVisualStyleBackColor = true;
            this.button_buyTechInStore.Click += new System.EventHandler(this.button_buyTechInStore_Click);
            // 
            // listView_techStore
            // 
            this.listView_techStore.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_techName,
            this.columnHeader_techPrice,
            this.columnHeader_techSeller});
            this.listView_techStore.FullRowSelect = true;
            this.listView_techStore.GridLines = true;
            this.listView_techStore.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_techStore.HideSelection = false;
            this.listView_techStore.Location = new System.Drawing.Point(3, 41);
            this.listView_techStore.MultiSelect = false;
            this.listView_techStore.Name = "listView_techStore";
            this.listView_techStore.Size = new System.Drawing.Size(529, 303);
            this.listView_techStore.TabIndex = 14;
            this.listView_techStore.UseCompatibleStateImageBehavior = false;
            this.listView_techStore.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader_techName
            // 
            this.columnHeader_techName.Text = "Name";
            this.columnHeader_techName.Width = 264;
            // 
            // columnHeader_techPrice
            // 
            this.columnHeader_techPrice.Text = "Price";
            this.columnHeader_techPrice.Width = 91;
            // 
            // columnHeader_techSeller
            // 
            this.columnHeader_techSeller.Text = "Seller";
            this.columnHeader_techSeller.Width = 131;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(102, 15);
            this.label10.TabIndex = 13;
            this.label10.Text = "판매중인 기술";
            // 
            // button_refreshTechStore
            // 
            this.button_refreshTechStore.Location = new System.Drawing.Point(120, 6);
            this.button_refreshTechStore.Name = "button_refreshTechStore";
            this.button_refreshTechStore.Size = new System.Drawing.Size(96, 31);
            this.button_refreshTechStore.TabIndex = 9;
            this.button_refreshTechStore.Text = "새로고침";
            this.button_refreshTechStore.UseVisualStyleBackColor = true;
            this.button_refreshTechStore.Click += new System.EventHandler(this.button_refreshTechStore_Click);
            // 
            // button_closeTechStore
            // 
            this.button_closeTechStore.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_closeTechStore.Location = new System.Drawing.Point(504, 3);
            this.button_closeTechStore.Name = "button_closeTechStore";
            this.button_closeTechStore.Size = new System.Drawing.Size(28, 24);
            this.button_closeTechStore.TabIndex = 12;
            this.button_closeTechStore.Text = "X";
            this.button_closeTechStore.UseVisualStyleBackColor = true;
            this.button_closeTechStore.Click += new System.EventHandler(this.button_closeTechStore_Click);
            // 
            // panel_productStore
            // 
            this.panel_productStore.BackColor = System.Drawing.SystemColors.Control;
            this.panel_productStore.Controls.Add(this.button_buyProductInStore);
            this.panel_productStore.Controls.Add(this.listView_productStore);
            this.panel_productStore.Controls.Add(this.label11);
            this.panel_productStore.Controls.Add(this.button_refreshProductStore);
            this.panel_productStore.Controls.Add(this.button_closeProductStore);
            this.panel_productStore.Location = new System.Drawing.Point(1124, 469);
            this.panel_productStore.Name = "panel_productStore";
            this.panel_productStore.Size = new System.Drawing.Size(535, 384);
            this.panel_productStore.TabIndex = 15;
            this.panel_productStore.Visible = false;
            // 
            // button_buyProductInStore
            // 
            this.button_buyProductInStore.Location = new System.Drawing.Point(3, 350);
            this.button_buyProductInStore.Name = "button_buyProductInStore";
            this.button_buyProductInStore.Size = new System.Drawing.Size(529, 31);
            this.button_buyProductInStore.TabIndex = 9;
            this.button_buyProductInStore.Text = "Buy";
            this.button_buyProductInStore.UseVisualStyleBackColor = true;
            this.button_buyProductInStore.Click += new System.EventHandler(this.button_buyProductInStore_Click);
            // 
            // listView_productStore
            // 
            this.listView_productStore.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader7});
            this.listView_productStore.FullRowSelect = true;
            this.listView_productStore.GridLines = true;
            this.listView_productStore.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView_productStore.HideSelection = false;
            this.listView_productStore.Location = new System.Drawing.Point(3, 41);
            this.listView_productStore.MultiSelect = false;
            this.listView_productStore.Name = "listView_productStore";
            this.listView_productStore.Size = new System.Drawing.Size(529, 303);
            this.listView_productStore.TabIndex = 14;
            this.listView_productStore.UseCompatibleStateImageBehavior = false;
            this.listView_productStore.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 264;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Price";
            this.columnHeader4.Width = 91;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Seller";
            this.columnHeader7.Width = 131;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(102, 15);
            this.label11.TabIndex = 13;
            this.label11.Text = "판매중인 제품";
            // 
            // button_refreshProductStore
            // 
            this.button_refreshProductStore.Location = new System.Drawing.Point(120, 4);
            this.button_refreshProductStore.Name = "button_refreshProductStore";
            this.button_refreshProductStore.Size = new System.Drawing.Size(96, 31);
            this.button_refreshProductStore.TabIndex = 9;
            this.button_refreshProductStore.Text = "새로고침";
            this.button_refreshProductStore.UseVisualStyleBackColor = true;
            this.button_refreshProductStore.Click += new System.EventHandler(this.button_refreshProductStore_Click);
            // 
            // button_closeProductStore
            // 
            this.button_closeProductStore.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button_closeProductStore.Location = new System.Drawing.Point(504, 3);
            this.button_closeProductStore.Name = "button_closeProductStore";
            this.button_closeProductStore.Size = new System.Drawing.Size(28, 24);
            this.button_closeProductStore.TabIndex = 12;
            this.button_closeProductStore.Text = "X";
            this.button_closeProductStore.UseVisualStyleBackColor = true;
            this.button_closeProductStore.Click += new System.EventHandler(this.button_closeProductStore_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1914, 1037);
            this.Controls.Add(this.panel_menu);
            this.Controls.Add(this.panel_productStore);
            this.Controls.Add(this.panel_techStore);
            this.Controls.Add(this.button_slideTop);
            this.Controls.Add(this.panel_topSlideScreen);
            this.Controls.Add(this.panel_user);
            this.Controls.Add(this.panel_manageCompany);
            this.Controls.Add(this.panel_mailbox);
            this.Controls.Add(this.panel_bottomScreen);
            this.Controls.Add(this.panel_topScreen);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClickWar2 Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form_Main_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_Main_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_Main_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseUp);
            this.Resize += new System.EventHandler(this.Form_Main_Resize);
            this.panel_menu.ResumeLayout(false);
            this.panel_user.ResumeLayout(false);
            this.panel_topScreen.ResumeLayout(false);
            this.panel_topScreen.PerformLayout();
            this.panel_bottomScreen.ResumeLayout(false);
            this.panel_mailbox.ResumeLayout(false);
            this.panel_manageCompany.ResumeLayout(false);
            this.panel_manageCompany.PerformLayout();
            this.contextMenuStrip_productList.ResumeLayout(false);
            this.contextMenuStrip_techList.ResumeLayout(false);
            this.panel_topSlideScreen.ResumeLayout(false);
            this.panel_topSlideScreen.PerformLayout();
            this.panel_techStore.ResumeLayout(false);
            this.panel_techStore.PerformLayout();
            this.panel_productStore.ResumeLayout(false);
            this.panel_productStore.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer_update;
        private System.Windows.Forms.Timer timer_whenLongClick;
        private System.Windows.Forms.Panel panel_menu;
        private System.Windows.Forms.Button button_logout;
        private System.Windows.Forms.Button button_closeMenu;
        private System.Windows.Forms.Button button_exit;
        private System.Windows.Forms.Panel panel_user;
        private System.Windows.Forms.ListView listView_userList;
        private System.Windows.Forms.ColumnHeader columnHeader_rank;
        private System.Windows.Forms.ColumnHeader columnHeader_userName;
        private System.Windows.Forms.ColumnHeader columnHeader_territoryCount;
        private System.Windows.Forms.ColumnHeader columnHeader_resource;
        private System.Windows.Forms.Timer timer_slowUpdate;
        private System.Windows.Forms.Panel panel_topScreen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_userResource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_userAreaCount;
        private System.Windows.Forms.Label label_userName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel_bottomScreen;
        private System.Windows.Forms.Button button_showMailbox;
        private System.Windows.Forms.Panel panel_mailbox;
        private System.Windows.Forms.ListView listView_mailbox;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button button_closeMailbox;
        private System.Windows.Forms.Button button_openMail;
        private System.Windows.Forms.Button button_writeMail;
        private System.Windows.Forms.Button button_registerCompany;
        private System.Windows.Forms.Button button_manageCompany;
        private System.Windows.Forms.Panel panel_manageCompany;
        private System.Windows.Forms.Button button_devTech;
        private System.Windows.Forms.Button button_sellTech;
        private System.Windows.Forms.Button button_buyTech;
        private System.Windows.Forms.ListBox listBox_techList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_companyName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBox_productList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button_produceProduct;
        private System.Windows.Forms.Button button_sellProduct;
        private System.Windows.Forms.Button button_buyProduct;
        private System.Windows.Forms.Button button_closeCompanyManagement;
        private System.Windows.Forms.Label label_productRate;
        private System.Windows.Forms.Label label_techRate;
        private System.Windows.Forms.Label label_companySiteCount;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_techList;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_deleteTech;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_productList;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_deleteProduct;
        private System.Windows.Forms.Panel panel_topSlideScreen;
        private System.Windows.Forms.Button button_slideTop;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListBox listBox_productListForBuild;
        private System.Windows.Forms.ComboBox comboBox_companyList;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_upgradeTech;
        private System.Windows.Forms.Panel panel_techStore;
        private System.Windows.Forms.ListView listView_techStore;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button_refreshTechStore;
        private System.Windows.Forms.Button button_closeTechStore;
        private System.Windows.Forms.ColumnHeader columnHeader_techName;
        private System.Windows.Forms.ColumnHeader columnHeader_techPrice;
        private System.Windows.Forms.ColumnHeader columnHeader_techSeller;
        private System.Windows.Forms.Button button_buyTechInStore;
        private System.Windows.Forms.Button button_help;
        private System.Windows.Forms.Label label_networkState;
        private System.Windows.Forms.Panel panel_productStore;
        private System.Windows.Forms.Button button_buyProductInStore;
        private System.Windows.Forms.ListView listView_productStore;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button_refreshProductStore;
        private System.Windows.Forms.Button button_closeProductStore;
    }
}

