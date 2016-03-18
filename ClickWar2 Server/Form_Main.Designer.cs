namespace ClickWar2_Server
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBox_loginUser = new System.Windows.Forms.ListBox();
            this.label_userCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer_update = new System.Windows.Forms.Timer(this.components);
            this.timer_slowUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox_allUser = new System.Windows.Forms.ListBox();
            this.label_allUserCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.timer_fastUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox_showUsersScreen = new System.Windows.Forms.CheckBox();
            this.checkBox_drawMap = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button_zoomOut = new System.Windows.Forms.Button();
            this.button_zoomIn = new System.Windows.Forms.Button();
            this.button_saveAll = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label_totalArea = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.listBox_log = new System.Windows.Forms.ListBox();
            this.contextMenuStrip_manageLoginUser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_forceLogout = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_manageAccount = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_deleteAccount = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.contextMenuStrip_manageLoginUser.SuspendLayout();
            this.contextMenuStrip_manageAccount.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox_loginUser);
            this.groupBox1.Controls.Add(this.label_userCount);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 165);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Online Users";
            // 
            // listBox_loginUser
            // 
            this.listBox_loginUser.ContextMenuStrip = this.contextMenuStrip_manageLoginUser;
            this.listBox_loginUser.FormattingEnabled = true;
            this.listBox_loginUser.HorizontalScrollbar = true;
            this.listBox_loginUser.ItemHeight = 15;
            this.listBox_loginUser.Location = new System.Drawing.Point(6, 45);
            this.listBox_loginUser.Name = "listBox_loginUser";
            this.listBox_loginUser.ScrollAlwaysVisible = true;
            this.listBox_loginUser.Size = new System.Drawing.Size(215, 109);
            this.listBox_loginUser.TabIndex = 1;
            // 
            // label_userCount
            // 
            this.label_userCount.AutoSize = true;
            this.label_userCount.Location = new System.Drawing.Point(68, 27);
            this.label_userCount.Name = "label_userCount";
            this.label_userCount.Size = new System.Drawing.Size(15, 15);
            this.label_userCount.TabIndex = 2;
            this.label_userCount.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Count :";
            // 
            // timer_update
            // 
            this.timer_update.Interval = 16;
            this.timer_update.Tick += new System.EventHandler(this.timer_update_Tick);
            // 
            // timer_slowUpdate
            // 
            this.timer_slowUpdate.Interval = 3000;
            this.timer_slowUpdate.Tick += new System.EventHandler(this.timer_slowUpdate_Tick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox_allUser);
            this.groupBox2.Controls.Add(this.label_allUserCount);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(245, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(393, 165);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "All Users";
            // 
            // listBox_allUser
            // 
            this.listBox_allUser.ContextMenuStrip = this.contextMenuStrip_manageAccount;
            this.listBox_allUser.FormattingEnabled = true;
            this.listBox_allUser.HorizontalScrollbar = true;
            this.listBox_allUser.ItemHeight = 15;
            this.listBox_allUser.Location = new System.Drawing.Point(6, 45);
            this.listBox_allUser.Name = "listBox_allUser";
            this.listBox_allUser.ScrollAlwaysVisible = true;
            this.listBox_allUser.Size = new System.Drawing.Size(381, 109);
            this.listBox_allUser.TabIndex = 1;
            // 
            // label_allUserCount
            // 
            this.label_allUserCount.AutoSize = true;
            this.label_allUserCount.Location = new System.Drawing.Point(68, 27);
            this.label_allUserCount.Name = "label_allUserCount";
            this.label_allUserCount.Size = new System.Drawing.Size(15, 15);
            this.label_allUserCount.TabIndex = 2;
            this.label_allUserCount.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Count :";
            // 
            // timer_fastUpdate
            // 
            this.timer_fastUpdate.Interval = 4;
            this.timer_fastUpdate.Tick += new System.EventHandler(this.timer_fastUpdate_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox_showUsersScreen);
            this.groupBox3.Controls.Add(this.checkBox_drawMap);
            this.groupBox3.Location = new System.Drawing.Point(644, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(312, 165);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "View Option";
            // 
            // checkBox_showUsersScreen
            // 
            this.checkBox_showUsersScreen.AutoSize = true;
            this.checkBox_showUsersScreen.Enabled = false;
            this.checkBox_showUsersScreen.Location = new System.Drawing.Point(22, 48);
            this.checkBox_showUsersScreen.Name = "checkBox_showUsersScreen";
            this.checkBox_showUsersScreen.Size = new System.Drawing.Size(161, 19);
            this.checkBox_showUsersScreen.TabIndex = 6;
            this.checkBox_showUsersScreen.Text = "Show user\'s screen";
            this.checkBox_showUsersScreen.UseVisualStyleBackColor = true;
            // 
            // checkBox_drawMap
            // 
            this.checkBox_drawMap.AutoSize = true;
            this.checkBox_drawMap.Location = new System.Drawing.Point(6, 23);
            this.checkBox_drawMap.Name = "checkBox_drawMap";
            this.checkBox_drawMap.Size = new System.Drawing.Size(108, 19);
            this.checkBox_drawMap.TabIndex = 5;
            this.checkBox_drawMap.Text = "Show board";
            this.checkBox_drawMap.UseVisualStyleBackColor = true;
            this.checkBox_drawMap.CheckedChanged += new System.EventHandler(this.checkBox_drawMap_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button_zoomOut);
            this.groupBox4.Controls.Add(this.button_zoomIn);
            this.groupBox4.Controls.Add(this.button_saveAll);
            this.groupBox4.Location = new System.Drawing.Point(12, 183);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(227, 165);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Control";
            // 
            // button_zoomOut
            // 
            this.button_zoomOut.Location = new System.Drawing.Point(116, 74);
            this.button_zoomOut.Name = "button_zoomOut";
            this.button_zoomOut.Size = new System.Drawing.Size(105, 44);
            this.button_zoomOut.TabIndex = 7;
            this.button_zoomOut.Text = "Zoom Out";
            this.button_zoomOut.UseVisualStyleBackColor = true;
            this.button_zoomOut.Click += new System.EventHandler(this.button_zoomOut_Click);
            // 
            // button_zoomIn
            // 
            this.button_zoomIn.Location = new System.Drawing.Point(6, 74);
            this.button_zoomIn.Name = "button_zoomIn";
            this.button_zoomIn.Size = new System.Drawing.Size(105, 44);
            this.button_zoomIn.TabIndex = 6;
            this.button_zoomIn.Text = "Zoom In";
            this.button_zoomIn.UseVisualStyleBackColor = true;
            this.button_zoomIn.Click += new System.EventHandler(this.button_zoomIn_Click);
            // 
            // button_saveAll
            // 
            this.button_saveAll.Location = new System.Drawing.Point(6, 24);
            this.button_saveAll.Name = "button_saveAll";
            this.button_saveAll.Size = new System.Drawing.Size(215, 44);
            this.button_saveAll.TabIndex = 6;
            this.button_saveAll.Text = "Save all";
            this.button_saveAll.UseVisualStyleBackColor = true;
            this.button_saveAll.Click += new System.EventHandler(this.button_saveAll_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label_totalArea);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Location = new System.Drawing.Point(12, 354);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(227, 165);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Game";
            // 
            // label_totalArea
            // 
            this.label_totalArea.AutoSize = true;
            this.label_totalArea.Location = new System.Drawing.Point(94, 27);
            this.label_totalArea.Name = "label_totalArea";
            this.label_totalArea.Size = new System.Drawing.Size(15, 15);
            this.label_totalArea.TabIndex = 1;
            this.label_totalArea.Text = "?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Total area :";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.listBox_log);
            this.groupBox6.Location = new System.Drawing.Point(245, 183);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(711, 165);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Log";
            // 
            // listBox_log
            // 
            this.listBox_log.FormattingEnabled = true;
            this.listBox_log.HorizontalScrollbar = true;
            this.listBox_log.ItemHeight = 15;
            this.listBox_log.Location = new System.Drawing.Point(6, 27);
            this.listBox_log.Name = "listBox_log";
            this.listBox_log.ScrollAlwaysVisible = true;
            this.listBox_log.Size = new System.Drawing.Size(699, 124);
            this.listBox_log.TabIndex = 0;
            // 
            // contextMenuStrip_manageLoginUser
            // 
            this.contextMenuStrip_manageLoginUser.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_manageLoginUser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_forceLogout});
            this.contextMenuStrip_manageLoginUser.Name = "contextMenuStrip_manageLoginUser";
            this.contextMenuStrip_manageLoginUser.Size = new System.Drawing.Size(180, 30);
            this.contextMenuStrip_manageLoginUser.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_manageLoginUser_Opening);
            // 
            // ToolStripMenuItem_forceLogout
            // 
            this.ToolStripMenuItem_forceLogout.Name = "ToolStripMenuItem_forceLogout";
            this.ToolStripMenuItem_forceLogout.Size = new System.Drawing.Size(179, 26);
            this.ToolStripMenuItem_forceLogout.Text = "강제 로그아웃";
            this.ToolStripMenuItem_forceLogout.Click += new System.EventHandler(this.ToolStripMenuItem_forceLogout_Click);
            // 
            // contextMenuStrip_manageAccount
            // 
            this.contextMenuStrip_manageAccount.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip_manageAccount.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_deleteAccount});
            this.contextMenuStrip_manageAccount.Name = "contextMenuStrip_manageAccount";
            this.contextMenuStrip_manageAccount.Size = new System.Drawing.Size(150, 30);
            this.contextMenuStrip_manageAccount.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_manageAccount_Opening);
            // 
            // ToolStripMenuItem_deleteAccount
            // 
            this.ToolStripMenuItem_deleteAccount.Name = "ToolStripMenuItem_deleteAccount";
            this.ToolStripMenuItem_deleteAccount.Size = new System.Drawing.Size(149, 26);
            this.ToolStripMenuItem_deleteAccount.Text = "계정 삭제";
            this.ToolStripMenuItem_deleteAccount.Click += new System.EventHandler(this.ToolStripMenuItem_deleteAccount_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClickWar2 Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form_Main_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_Main_KeyDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_Main_MouseUp);
            this.Resize += new System.EventHandler(this.Form_Main_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.contextMenuStrip_manageLoginUser.ResumeLayout(false);
            this.contextMenuStrip_manageAccount.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label_userCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox_loginUser;
        private System.Windows.Forms.Timer timer_update;
        private System.Windows.Forms.Timer timer_slowUpdate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox_allUser;
        private System.Windows.Forms.Label label_allUserCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timer_fastUpdate;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox_drawMap;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_saveAll;
        private System.Windows.Forms.Button button_zoomOut;
        private System.Windows.Forms.Button button_zoomIn;
        private System.Windows.Forms.CheckBox checkBox_showUsersScreen;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label_totalArea;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ListBox listBox_log;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_manageLoginUser;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_forceLogout;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_manageAccount;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_deleteAccount;
    }
}

