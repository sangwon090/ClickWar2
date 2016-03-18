using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickWar2_Client
{
    public partial class Form_Start : Form
    {
        public Form_Start()
        {
            InitializeComponent();
        }

        //#####################################################################################

        private void Form_Start_Load(object sender, EventArgs e)
        {
            this.label_under.Text = Application.ProductVersion;


            this.timer_update.Start();
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1.0)
            {
                this.Opacity += 0.02;

                if (this.Opacity >= 1.0)
                {
                    this.Opacity = 1.0;

                    this.timer_update.Stop();


                    bool bExit = this.CheckUpdateAndNotice();

                    if (bExit)
                    {
                        this.Close();
                        Application.Exit();
                    }
                    else
                    {
                        this.SequenceToNext();
                    }
                }
            }
        }

        //#####################################################################################

        private bool CheckUpdateAndNotice()
        {
            string downloadLink;
            bool bShutdown;
            string notice;

            bool bNeedUpdate = ClickWar2.Application.CheckUpdateAndNotice("ClientPublish", Application.ProductVersion,
                out downloadLink, out bShutdown, out notice);

            if (notice.Length > 0)
            {
                MessageBox.Show(string.Format("{0}", notice), "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (bShutdown)
            {
                return bShutdown;
            }

            if (bNeedUpdate)
            {
                var dlgResult = MessageBox.Show("업데이트가 있습니다.\n다운로드 하시겠습니까?", "Info",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dlgResult == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(downloadLink);
                    return true;
                }
            }


            return bShutdown;
        }

        private void SequenceToNext()
        {
            var nextForm = new Form_Connect();
            nextForm.Show();

            this.Hide();
        }
    }
}
