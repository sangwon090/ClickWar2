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


            this.timer_check.Start();
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            this.timer_check.Stop();


            bool bExit = this.CheckUpdateAndNotice();

            if (bExit == false)
            {
                this.SequenceToNext();
            }
        }

        //#####################################################################################

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

            if(bNeedUpdate)
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

        private void SequenceToNext()
        {
            var nextForm = new Form_Open();
            nextForm.Show();

            this.Hide();
        }
    }
}
