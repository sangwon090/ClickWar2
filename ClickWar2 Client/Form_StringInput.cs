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
    public partial class Form_StringInput : Form
    {
        public Form_StringInput(IStringReceiver receiver, int maxLength)
        {
            InitializeComponent();


            m_receiver = receiver;

            this.textBox_inputText.MaxLength = maxLength;
        }

        //#####################################################################################

        protected IStringReceiver m_receiver;

        //#####################################################################################

        private void button_confirm_Click(object sender, EventArgs e)
        {
            string inputText = this.textBox_inputText.Text;

            m_receiver.ReceiveInputText(inputText);

            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //#####################################################################################

        public void SetText(string text)
        {
            this.textBox_inputText.Text = text;
            this.textBox_inputText.SelectAll();
        }
    }
}
