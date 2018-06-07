namespace ClickWar2_Client
{
    partial class Form_Start
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Start));
            this.label_under = new System.Windows.Forms.Label();
            this.timer_check = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label_under
            // 
            this.label_under.AutoSize = true;
            this.label_under.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label_under.Location = new System.Drawing.Point(12, 232);
            this.label_under.Name = "label_under";
            this.label_under.Size = new System.Drawing.Size(0, 15);
            this.label_under.TabIndex = 0;
            // 
            // timer_check
            // 
            this.timer_check.Interval = 16;
            this.timer_check.Tick += new System.EventHandler(this.timer_check_Tick);
            // 
            // Form_Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::ClickWar2_Client.Properties.Resources.loading;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(512, 256);
            this.Controls.Add(this.label_under);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form_Start";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading";
            this.Load += new System.EventHandler(this.Form_Start_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_under;
        private System.Windows.Forms.Timer timer_check;
    }
}