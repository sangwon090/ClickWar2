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
    public partial class Form_DevTech : Form
    {
        public Form_DevTech()
        {
            InitializeComponent();
        }

        //#####################################################################################

        protected string m_techName = "";
        public string TechName
        {get { return m_techName; } set { m_techName = value; this.textBox_techName.Text = value; } }

        protected List<ClickWar2.Game.Command> m_cmdList = new List<ClickWar2.Game.Command>();
        public List<ClickWar2.Game.Command> Program
        { get { return m_cmdList; } }

        public string Code
        { set { this.textBox_program.Text = value; } }

        public bool Confirmed
        { get; set; } = false;

        //#####################################################################################

        private void Form_DevTech_Load(object sender, EventArgs e)
        {
            this.linkLabel_help.Links.Add(0, 4, "http://blog.naver.com/tlsehdgus321/220647320604");
        }

        private void linkLabel_help_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void button_confirm_Click(object sender, EventArgs e)
        {
            this.textBox_techName.Text = this.textBox_techName.Text.Trim();

            if (this.textBox_techName.TextLength > 0)
            {
                m_techName = this.textBox_techName.Text;

                this.ParseCode();


                this.Confirmed = true;


                this.Close();
            }
            else
            {
                MessageBox.Show("이름을 입력하세요.", "Warning!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            m_techName = "";
            m_cmdList.Clear();


            this.Close();
        }

        private void textBox_program_TextChanged(object sender, EventArgs e)
        {
            this.ParseCode();

            this.label_devFee.Text = string.Format("{0}◎",
                m_cmdList.Count * ClickWar2.Game.GameValues.DevFeePerProgramLine);
        }

        //#####################################################################################
        
        protected void ParseCode()
        {
            ClickWar2.Game.ScriptParser parser = new ClickWar2.Game.ScriptParser();
            m_cmdList = parser.Parse(this.textBox_program.Text);
        }
    }
}
