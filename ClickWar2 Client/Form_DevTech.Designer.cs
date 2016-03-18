namespace ClickWar2_Client
{
    partial class Form_DevTech
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_DevTech));
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_techName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_program = new System.Windows.Forms.TextBox();
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_confirm = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label_devFee = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel_help = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "이름 :";
            // 
            // textBox_techName
            // 
            this.textBox_techName.Location = new System.Drawing.Point(65, 12);
            this.textBox_techName.MaxLength = 128;
            this.textBox_techName.Name = "textBox_techName";
            this.textBox_techName.Size = new System.Drawing.Size(343, 25);
            this.textBox_techName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "프로그램";
            // 
            // textBox_program
            // 
            this.textBox_program.Location = new System.Drawing.Point(12, 68);
            this.textBox_program.Multiline = true;
            this.textBox_program.Name = "textBox_program";
            this.textBox_program.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_program.Size = new System.Drawing.Size(396, 360);
            this.textBox_program.TabIndex = 3;
            this.textBox_program.TextChanged += new System.EventHandler(this.textBox_program_TextChanged);
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(218, 459);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(190, 42);
            this.button_cancel.TabIndex = 6;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_confirm
            // 
            this.button_confirm.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_confirm.Location = new System.Drawing.Point(12, 459);
            this.button_confirm.Name = "button_confirm";
            this.button_confirm.Size = new System.Drawing.Size(190, 42);
            this.button_confirm.TabIndex = 5;
            this.button_confirm.Text = "Confirm";
            this.button_confirm.UseVisualStyleBackColor = true;
            this.button_confirm.Click += new System.EventHandler(this.button_confirm_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(137, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "개발 비용 :";
            // 
            // label_devFee
            // 
            this.label_devFee.AutoSize = true;
            this.label_devFee.Location = new System.Drawing.Point(225, 50);
            this.label_devFee.Name = "label_devFee";
            this.label_devFee.Size = new System.Drawing.Size(29, 15);
            this.label_devFee.TabIndex = 8;
            this.label_devFee.Text = "0◎";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 441);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(317, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "주의 : 아래 버튼을 누르기 전에 백업해두세요.";
            // 
            // linkLabel_help
            // 
            this.linkLabel_help.AutoSize = true;
            this.linkLabel_help.Location = new System.Drawing.Point(372, 50);
            this.linkLabel_help.Name = "linkLabel_help";
            this.linkLabel_help.Size = new System.Drawing.Size(36, 15);
            this.linkLabel_help.TabIndex = 10;
            this.linkLabel_help.TabStop = true;
            this.linkLabel_help.Text = "Help";
            this.linkLabel_help.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_help_LinkClicked);
            // 
            // Form_DevTech
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(420, 513);
            this.Controls.Add(this.linkLabel_help);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label_devFee);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_confirm);
            this.Controls.Add(this.textBox_program);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_techName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form_DevTech";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Develop Chip";
            this.Load += new System.EventHandler(this.Form_DevTech_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_techName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_program;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_confirm;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_devFee;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel_help;
    }
}