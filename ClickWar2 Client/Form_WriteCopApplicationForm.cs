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
    public partial class Form_WriteCopApplicationForm : Form
    {
        public Form_WriteCopApplicationForm(ICopApplicationFormReceiver receiver)
        {
            InitializeComponent();


            m_receiver = receiver;
        }

        //#####################################################################################

        public string Notice
        { get { return this.label_notice.Text; } set { this.label_notice.Text = value; } }

        public int MaxNameLength
        {
            get { return this.textBox_name.MaxLength; }
            set { this.textBox_name.MaxLength = value; }
        }

        protected ICopApplicationFormReceiver m_receiver = null;

        //#####################################################################################

        private void button_confirm_Click(object sender, EventArgs e)
        {
            if (this.textBox_name.TextLength > 0)
            {
                if (m_receiver != null)
                {
                    m_receiver.ReceiveInputCopApplicationForm(this.textBox_name.Text);
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
    }
}
