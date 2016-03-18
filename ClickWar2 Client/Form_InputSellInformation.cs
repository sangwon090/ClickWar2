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
    public partial class Form_InputSellInformation : Form
    {
        public Form_InputSellInformation()
        {
            InitializeComponent();
        }

        //#####################################################################################

        public bool Confirmed
        { get; set; } = false;

        public string ItemName
        { get { return this.label_itemName.Text; } set { this.label_itemName.Text = value; } }

        public int Price
        { get { return (int)this.numericUpDown_price.Value; } set { this.numericUpDown_price.Value = value; } }

        public string TargetUser
        {
            get
            {
                if (this.checkBox_targetAll.Checked)
                    return "";
                else
                    return this.comboBox_targetUser.Text;
            }
            set
            {
                this.checkBox_targetAll.Checked = (value.Length <= 0);

                this.comboBox_targetUser.Text = value;
                this.comboBox_targetUser.Enabled = !this.checkBox_targetAll.Checked;
            }
        }

        //#####################################################################################

        private void button_confirm_Click(object sender, EventArgs e)
        {
            if (this.checkBox_targetAll.Checked
                || (this.checkBox_targetAll.Checked == false
                && this.comboBox_targetUser.Text.Length > 0))
            {
                this.Confirmed = true;

                this.Close();
            }
            else
            {
                MessageBox.Show("내용을 모두 입력해주세요.", "Warning!",
                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Confirmed = false;

            this.Close();
        }

        private void checkBox_targetAll_CheckedChanged(object sender, EventArgs e)
        {
            this.comboBox_targetUser.Enabled = !this.checkBox_targetAll.Checked;

            if (this.comboBox_targetUser.Enabled == false)
                this.comboBox_targetUser.Text = "";
        }

        //#####################################################################################

        public void SetUserList(string[] users)
        {
            this.comboBox_targetUser.Items.Clear();

            for (int i = 0; i < users.Length; ++i)
            {
                this.comboBox_targetUser.Items.Add(users[i]);
            }
        }
    }
}
