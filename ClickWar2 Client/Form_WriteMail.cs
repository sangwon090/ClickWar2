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
    public partial class Form_WriteMail : Form
    {
        public Form_WriteMail(IMailReceiver receiver)
        {
            InitializeComponent();


            m_receiver = receiver;
        }

        //#####################################################################################

        protected IMailReceiver m_receiver = null;

        //#####################################################################################

        private void button_send_Click(object sender, EventArgs e)
        {
            if (this.comboBox_targetUser.Text.Length > 0
                && this.textBox_message.TextLength > 0)
            {
                if (m_receiver != null)
                {
                    m_receiver.ReceiveInputMail(this.comboBox_targetUser.Text, this.textBox_message.Text);
                }


                this.Close();
            }
            else
            {
                MessageBox.Show("내용을 입력해주세요.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //#####################################################################################

        public void SetUserList(string[] userNames)
        {
            this.comboBox_targetUser.Items.Clear();

            foreach (string user in userNames)
            {
                this.comboBox_targetUser.Items.Add(user);
            }
        }
    }
}
