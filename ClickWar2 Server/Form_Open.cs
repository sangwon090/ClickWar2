using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickWar2_Server
{
    public partial class Form_Open : Form
    {
        public Form_Open()
        {
            InitializeComponent();


#if DEBUG
            this.textBox_port.Text = "3344";
#else
            this.textBox_port.Text = ClickWar2.Utility.Random.Next(1100, 20000).ToString();
            this.textBox_port.SelectAll();
#endif
        }

        //#####################################################################################

        private void Form_Connect_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (this.textBox_port.Text.Length >= 4)
            {
                // 서버 경로가 유효하지 않으면 만듬.
                string serverPath = AppDomain.CurrentDomain.BaseDirectory + "Server/";

                if (System.IO.Directory.Exists(serverPath) == false)
                    System.IO.Directory.CreateDirectory(serverPath);


                // 서버 생성 및 시작
                var server = new ClickWar2.Game.Network.GameServer(serverPath);

                int port;
                if (int.TryParse(this.textBox_port.Text, out port))
                {
                    server.Start(this.textBox_port.Text);


                    if (server.IsOpened)
                    {
                        // 다음 창으로 이동
                        this.Hide();

                        foreach (Form form in Application.OpenForms)
                        {
                            if (form is Form_Main)
                            {
                                ((Form_Main)form).Reset(server);
                                form.Show();
                                return;
                            }
                        }

                        var nextForm = new Form_Main(server);
                        nextForm.Show();
                    }
                    else
                    {
                        server.Stop();

                        MessageBox.Show("현재 속성으로 서버를 열 수 없습니다.", "Error!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("포트가 유효하지 않습니다.", "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.textBox_port.Select();
                }
            }
            else
            {
                MessageBox.Show("포트는 최소 4자리여야 합니다.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
