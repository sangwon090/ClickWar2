using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using ClickWar2.Game.Network;
using ClickWar2.Game.View;
using ClickWar2.Game.Presenter;

namespace ClickWar2_Server
{
    public partial class Form_Main : Form
    {
        public Form_Main(GameServer server)
        {
            // 더블 버퍼링
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            this.MouseWheel += Form_Main_MouseWheel;

            InitializeComponent();


            m_view.ScreenSize = this.ClientSize;


            m_presenter.BoardView = m_view;
            m_presenter.EffectDirector = m_effectView;

            this.Reset(server);


            // 로그 이벤트 등록
            ClickWar2.Utility.Logger.GetInstance().WhenLogAdded += Form_Main_WhenLogAdded;
        }

        //#####################################################################################
        // Model

        protected GameServer m_server = null;
        private Thread m_serverThread = null;

        //#####################################################################################
        // View

        protected BoardView m_view = new BoardView();
        protected EffectView m_effectView = new EffectView();

        //#####################################################################################
        // Presenter

        protected ServerBoardPresenter m_presenter = new ServerBoardPresenter();

        //#####################################################################################

        private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_server != null)
            {
                m_server.Stop();
            }

            Application.Exit();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            /*
            bool bExit = this.CheckUpdateAndNotice();

            if (bExit)
            {
                this.Close();
                Application.Exit();
            }
            */
        }

        public void Reset(GameServer server)
        {
            if (m_server != null)
            {
                m_server.Stop();
                m_server = null;
            }

            if (m_serverThread != null)
            {
                m_serverThread.Join();
            }

            m_server = server;


            m_presenter.Server = server;

            m_presenter.Initialize();


            // 로그 목록 초기화
            this.listBox_log.Items.Clear();


            // 갱신 타이머 시작
            this.timer_update.Start();
            this.timer_slowUpdate.Start();


            m_serverThread = new Thread(this.JobServer);
            m_serverThread.Start();
        }

        //#####################################################################################

        private void Form_Main_Paint(object sender, PaintEventArgs e)
        {
            if (this.checkBox_drawMap.Checked)
            {
                m_view.DrawBoard(m_server.GameBoard, this.ClientRectangle, true, e.Graphics);

                m_effectView.UpdateAndDrawEffect(e.Graphics);


                if (this.checkBox_showUsersScreen.Checked)
                {
                    var loginUsers = m_server.UserDirector.LoginUsers;

                    foreach (var user in loginUsers)
                    {
                        var rect = m_server.GameBoardDirector.GetUserScreen(user.Name);

                        rect.X *= m_view.TileSize;
                        rect.Y *= m_view.TileSize;
                        rect.Width *= m_view.TileSize;
                        rect.Height *= m_view.TileSize;

                        rect.X += m_view.BoardLocation.X;
                        rect.Y += m_view.BoardLocation.Y;

                        e.Graphics.DrawRectangle(Pens.Red, rect);
                    }
                }
            }
        }

        //#####################################################################################

        private void timer_slowUpdate_Tick(object sender, EventArgs e)
        {
            UpdateUserInformation();
            UpdateAllUserInformation();

            int totalArea = m_server.GameBoard.Board.ChunkCount * m_server.GameBoard.Board.ChunkSize
                * m_server.GameBoard.Board.ChunkSize;
            this.label_totalArea.Text = totalArea.ToString();
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            m_presenter.Update();


            // 화면 갱신
            if (this.checkBox_drawMap.Checked)
                this.Invalidate();
        }

        private void JobServer()
        {
            while (m_server != null)
            {
                m_server.Update();

                Task.Delay(TimeSpan.Zero).Wait();
            }
        }

        protected void UpdateUserInformation()
        {
            var clients = m_server.Server.Clients;

            this.label_userCount.Text = clients.Length.ToString();


            this.listBox_loginUser.BeginUpdate();

            this.listBox_loginUser.Items.Clear();

            foreach (var client in clients)
            {
                var user = m_server.UserDirector.GetLoginUser(client.ID);

                if (user != null)
                {
                    StringBuilder userInfo = new StringBuilder("\"");
                    userInfo.Append(user.Name);
                    userInfo.Append("\" (↓:");
                    userInfo.Append(client.Receiver.MessageCount);
                    userInfo.Append(", ↑:");
                    userInfo.Append(client.Sender.MessageCount);
                    userInfo.Append(")");

                    this.listBox_loginUser.Items.Add(userInfo.ToString());
                }
            }

            this.listBox_loginUser.EndUpdate();
        }

        protected void UpdateAllUserInformation()
        {
            var userList = m_server.UserDirector.Accounts;

            this.label_allUserCount.Text = userList.Length.ToString();


            this.listBox_allUser.BeginUpdate();

            this.listBox_allUser.Items.Clear();

            foreach (var user in userList)
            {
                StringBuilder infoText = new StringBuilder();
                infoText.Append('\"');
                infoText.Append(user.Name);
                infoText.Append('\"');

                infoText.Append(" ▣:");
                infoText.Append(user.AreaCount);

                infoText.Append(" ◎:");
                infoText.Append(user.Resource);


                this.listBox_allUser.Items.Add(infoText.ToString());
            }

            this.listBox_allUser.EndUpdate();
        }

        //#####################################################################################
        // 사용자 입력

        private void Form_Main_KeyDown(object sender, KeyEventArgs e)
        {
            m_view.OnKeyDown(e.KeyCode);
        }

        private void Form_Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_view.OnMouseLeftDown(e.Location);
            }
            else if (e.Button == MouseButtons.Right)
            {
                m_view.OnMouseRightDown(e.Location);
            }
        }

        private void Form_Main_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_view.OnMouseLeftUp(e.Location);
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

        private void Form_Main_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Form_Main_Resize(object sender, EventArgs e)
        {
            m_view.ScreenSize = this.ClientSize;
        }

        private void Form_Main_MouseWheel(object sender, MouseEventArgs e)
        {
            m_view.OnMouseWheelScroll(e.Delta);
        }

        private void button_saveAll_Click(object sender, EventArgs e)
        {
            m_server.StartSave();
        }

        private void button_zoomIn_Click(object sender, EventArgs e)
        {
            m_view.OnMouseWheelScroll(100);
        }

        private void button_zoomOut_Click(object sender, EventArgs e)
        {
            m_view.OnMouseWheelScroll(-100);
        }

        private void checkBox_drawMap_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBox_showUsersScreen.Enabled = this.checkBox_drawMap.Checked;

            if (this.checkBox_showUsersScreen.Enabled == false)
            {
                this.checkBox_showUsersScreen.Checked = false;
            }
        }

        private void contextMenuStrip_manageLoginUser_Opening(object sender, CancelEventArgs e)
        {
            this.ToolStripMenuItem_forceLogout.Enabled = (this.listBox_loginUser.SelectedIndex >= 0);
        }

        private void ToolStripMenuItem_forceLogout_Click(object sender, EventArgs e)
        {
            if (this.listBox_loginUser.SelectedIndex >= 0)
            {
                string userName = m_server.UserDirector.GetLoginUserNameAt(this.listBox_loginUser.SelectedIndex);
                m_server.UserDirector.ForceLogout(userName);
            }
        }

        private void contextMenuStrip_manageAccount_Opening(object sender, CancelEventArgs e)
        {
            this.ToolStripMenuItem_deleteAccount.Enabled = (this.listBox_allUser.SelectedIndex >= 0);
        }

        private void ToolStripMenuItem_deleteAccount_Click(object sender, EventArgs e)
        {
            if (this.listBox_allUser.SelectedIndex >= 0)
            {
                string userName = m_server.UserDirector.GetAccountNameAt(this.listBox_allUser.SelectedIndex);
                m_server.UserDirector.RemoveAccount(userName);
            }
        }

        //#####################################################################################

        private void Form_Main_WhenLogAdded(string message)
        {
            this.listBox_log.Invoke(new Action(() =>
            {
                this.listBox_log.BeginUpdate();


                if (this.listBox_log.Items.Count >= ClickWar2.Utility.Logger.GetInstance().MaxLogCount)
                {
                    this.listBox_log.Items.RemoveAt(0);
                }


                this.listBox_log.Items.Add("(" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + ") " + message);
                this.listBox_log.SelectedIndex = this.listBox_log.Items.Count - 1;


                this.listBox_log.EndUpdate();
            }));
        }

        private bool CheckUpdateAndNotice()
        {
            string downloadLink;
            bool bShutdown;
            string notice;

            bool bNeedUpdate = ClickWar2.Application.CheckUpdateAndNotice("ServerPublish", Application.ProductVersion,
                out downloadLink, out bShutdown, out notice);

            if (notice.Length > 0)
            {
                MessageBox.Show(string.Format("{0}", notice), "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (bShutdown)
            {
                Application.Exit();
                return bShutdown;
            }

            if (bNeedUpdate)
            {
                var dlgResult = MessageBox.Show("업데이트가 있습니다.\n다운로드 하시겠습니까?", "Info",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(downloadLink);
                    Application.Exit();
                }
            }


            return bShutdown;
        }
    }
}
