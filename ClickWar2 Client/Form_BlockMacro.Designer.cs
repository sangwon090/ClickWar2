namespace ClickWar2_Client
{
    partial class Form_BlockMacro
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BlockMacro));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timer_update = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label_leftTime = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_clickTest = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button_clickTest = new System.Windows.Forms.Button();
            this.button_resetClickTest = new System.Windows.Forms.Button();
            this.button_finish = new System.Windows.Forms.Button();
            this.panel_image1 = new System.Windows.Forms.Panel();
            this.panel_image2 = new System.Windows.Forms.Panel();
            this.panel_image3 = new System.Windows.Forms.Panel();
            this.checkBox_checkImg1 = new System.Windows.Forms.CheckBox();
            this.checkBox_checkImg2 = new System.Windows.Forms.CheckBox();
            this.checkBox_checkImg3 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(205, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "매크로를 사용하고 계십니까?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(287, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "확인을 위해 간단한 테스트를 진행합니다.";
            // 
            // timer_update
            // 
            this.timer_update.Interval = 16;
            this.timer_update.Tick += new System.EventHandler(this.timer_update_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "남은 시간 :";
            // 
            // label_leftTime
            // 
            this.label_leftTime.AutoSize = true;
            this.label_leftTime.Location = new System.Drawing.Point(100, 66);
            this.label_leftTime.Name = "label_leftTime";
            this.label_leftTime.Size = new System.Drawing.Size(15, 15);
            this.label_leftTime.TabIndex = 3;
            this.label_leftTime.Text = "?";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(247, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "미션 : 사각형이 있다면 체크하세요.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 266);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "미션 : 아래의 버튼을";
            // 
            // label_clickTest
            // 
            this.label_clickTest.AutoSize = true;
            this.label_clickTest.Location = new System.Drawing.Point(165, 266);
            this.label_clickTest.Name = "label_clickTest";
            this.label_clickTest.Size = new System.Drawing.Size(15, 15);
            this.label_clickTest.TabIndex = 9;
            this.label_clickTest.Text = "?";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(186, 266);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "번 클릭하세요.";
            // 
            // button_clickTest
            // 
            this.button_clickTest.Location = new System.Drawing.Point(12, 284);
            this.button_clickTest.Name = "button_clickTest";
            this.button_clickTest.Size = new System.Drawing.Size(186, 42);
            this.button_clickTest.TabIndex = 11;
            this.button_clickTest.Text = "0";
            this.button_clickTest.UseVisualStyleBackColor = true;
            this.button_clickTest.Click += new System.EventHandler(this.button_clickTest_Click);
            // 
            // button_resetClickTest
            // 
            this.button_resetClickTest.Location = new System.Drawing.Point(204, 284);
            this.button_resetClickTest.Name = "button_resetClickTest";
            this.button_resetClickTest.Size = new System.Drawing.Size(120, 42);
            this.button_resetClickTest.TabIndex = 12;
            this.button_resetClickTest.Text = "다시하기";
            this.button_resetClickTest.UseVisualStyleBackColor = true;
            this.button_resetClickTest.Click += new System.EventHandler(this.button_resetClickTest_Click);
            // 
            // button_finish
            // 
            this.button_finish.Location = new System.Drawing.Point(12, 343);
            this.button_finish.Name = "button_finish";
            this.button_finish.Size = new System.Drawing.Size(354, 42);
            this.button_finish.TabIndex = 13;
            this.button_finish.Text = "완료";
            this.button_finish.UseVisualStyleBackColor = true;
            this.button_finish.Click += new System.EventHandler(this.button_finish_Click);
            // 
            // panel_image1
            // 
            this.panel_image1.Location = new System.Drawing.Point(12, 120);
            this.panel_image1.Name = "panel_image1";
            this.panel_image1.Size = new System.Drawing.Size(100, 100);
            this.panel_image1.TabIndex = 5;
            this.panel_image1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_image1_Paint);
            // 
            // panel_image2
            // 
            this.panel_image2.Location = new System.Drawing.Point(118, 120);
            this.panel_image2.Name = "panel_image2";
            this.panel_image2.Size = new System.Drawing.Size(100, 100);
            this.panel_image2.TabIndex = 6;
            this.panel_image2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_image2_Paint);
            // 
            // panel_image3
            // 
            this.panel_image3.Location = new System.Drawing.Point(224, 120);
            this.panel_image3.Name = "panel_image3";
            this.panel_image3.Size = new System.Drawing.Size(100, 100);
            this.panel_image3.TabIndex = 6;
            this.panel_image3.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_image3_Paint);
            // 
            // checkBox_checkImg1
            // 
            this.checkBox_checkImg1.AutoSize = true;
            this.checkBox_checkImg1.Location = new System.Drawing.Point(12, 226);
            this.checkBox_checkImg1.Name = "checkBox_checkImg1";
            this.checkBox_checkImg1.Size = new System.Drawing.Size(74, 19);
            this.checkBox_checkImg1.TabIndex = 14;
            this.checkBox_checkImg1.Text = "사각형";
            this.checkBox_checkImg1.UseVisualStyleBackColor = true;
            // 
            // checkBox_checkImg2
            // 
            this.checkBox_checkImg2.AutoSize = true;
            this.checkBox_checkImg2.Location = new System.Drawing.Point(118, 226);
            this.checkBox_checkImg2.Name = "checkBox_checkImg2";
            this.checkBox_checkImg2.Size = new System.Drawing.Size(74, 19);
            this.checkBox_checkImg2.TabIndex = 15;
            this.checkBox_checkImg2.Text = "사각형";
            this.checkBox_checkImg2.UseVisualStyleBackColor = true;
            // 
            // checkBox_checkImg3
            // 
            this.checkBox_checkImg3.AutoSize = true;
            this.checkBox_checkImg3.Location = new System.Drawing.Point(224, 226);
            this.checkBox_checkImg3.Name = "checkBox_checkImg3";
            this.checkBox_checkImg3.Size = new System.Drawing.Size(74, 19);
            this.checkBox_checkImg3.TabIndex = 16;
            this.checkBox_checkImg3.Text = "사각형";
            this.checkBox_checkImg3.UseVisualStyleBackColor = true;
            // 
            // Form_BlockMacro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 397);
            this.Controls.Add(this.checkBox_checkImg3);
            this.Controls.Add(this.checkBox_checkImg2);
            this.Controls.Add(this.checkBox_checkImg1);
            this.Controls.Add(this.panel_image3);
            this.Controls.Add(this.panel_image2);
            this.Controls.Add(this.panel_image1);
            this.Controls.Add(this.button_finish);
            this.Controls.Add(this.button_resetClickTest);
            this.Controls.Add(this.button_clickTest);
            this.Controls.Add(this.label_clickTest);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label_leftTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_BlockMacro";
            this.Text = "Block Macro";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_BlockMacro_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_BlockMacro_FormClosed);
            this.Load += new System.EventHandler(this.Form_BlockMacro_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer timer_update;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_leftTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_clickTest;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button_clickTest;
        private System.Windows.Forms.Button button_resetClickTest;
        private System.Windows.Forms.Button button_finish;
        private System.Windows.Forms.Panel panel_image1;
        private System.Windows.Forms.Panel panel_image2;
        private System.Windows.Forms.Panel panel_image3;
        private System.Windows.Forms.CheckBox checkBox_checkImg1;
        private System.Windows.Forms.CheckBox checkBox_checkImg2;
        private System.Windows.Forms.CheckBox checkBox_checkImg3;
    }
}