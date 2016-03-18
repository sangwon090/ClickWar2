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
    public partial class Form_SelectString : Form
    {
        public Form_SelectString(string[] list = null)
        {
            InitializeComponent();


            if (list != null)
            {
                for (int i = 0; i < list.Length; ++i)
                {
                    this.comboBox_list.Items.Add(list[i]);
                }
            }
        }

        //#####################################################################################

        public string Notice
        { get { return this.label_notice.Text; } set { this.label_notice.Text = value; } }

        protected string m_selectedString = "";
        public string SelectedString
        { get { return m_selectedString; } }

        //#####################################################################################

        private void Form_SelectString_Load(object sender, EventArgs e)
        {
            this.comboBox_list.Select();
            this.comboBox_list.Focus();
        }

        private void button_confirm_Click(object sender, EventArgs e)
        {
            m_selectedString = this.comboBox_list.Text;


            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            m_selectedString = "";


            this.Close();
        }
    }
}
