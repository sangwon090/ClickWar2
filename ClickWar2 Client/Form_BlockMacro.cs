using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ClickWar2_Client
{
    public partial class Form_BlockMacro : Form
    {
        public Form_BlockMacro()
        {
            InitializeComponent();


            int tempCount = m_rand.Next(1, 64);
            for (int i = 0; i < tempCount; ++i)
                m_rand.Next();


            this.Location = new Point(this.Location.X + m_rand.Next(512),
                this.Location.Y + m_rand.Next(128));


            m_clickAnswer = m_rand.Next(3, 8);

            this.label_clickTest.Text = m_clickAnswer.ToString();

            m_clickAnswer -= 42;


            for (int i = 0; i < m_isRect.Length; ++i)
                m_isRect[i] = (m_rand.Next(0, 2) != 0);
            m_isRect[m_rand.Next(0, 2)] = true;

            for (int i = 0; i < m_drawSize.Length; ++i)
                m_drawSize[i] = m_rand.Next(16, 64);

            for (int i = 0; i < m_drawInfo.Length; ++i)
                m_drawInfo[i] = new Point(m_rand.Next(10, 80 - m_drawSize[i]), m_rand.Next(10, 80 - m_drawSize[i]));
        }

        //#####################################################################################

        protected Random m_rand = new Random(DateTime.Now.Millisecond + DateTime.Now.Second * 60000);

        protected long m_maxTime = 30 * 1000;

        protected Stopwatch m_timer = new Stopwatch();

        protected int m_clickAnswer;
        protected int m_clickCount = 0;

        protected bool[] m_isRect = new bool[3];
        protected Point[] m_drawInfo = new Point[3];
        protected int[] m_drawSize = new int[3];

        protected int m_tryCount = 0;

        public bool Pass
        { get; set; } = false;

        //#####################################################################################

        private void Form_BlockMacro_Load(object sender, EventArgs e)
        {
            this.timer_update.Start();


            m_timer.Restart();
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            long leftTime = m_maxTime - m_timer.ElapsedMilliseconds;
            this.label_leftTime.Text = string.Format("{0}", leftTime / 1000);

            if (leftTime <= 0)
            {
                Pass = false;

                this.Close();
            }
        }

        private void button_clickTest_Click(object sender, EventArgs e)
        {
            ++m_clickCount;

            this.button_clickTest.Text = m_clickCount.ToString();
        }

        private void button_resetClickTest_Click(object sender, EventArgs e)
        {
            m_clickCount = 0;

            this.button_clickTest.Text = m_clickCount.ToString();
        }

        private void button_finish_Click(object sender, EventArgs e)
        {
            ++m_tryCount;


            int realClickAnswer = m_clickAnswer + 42;

            if (realClickAnswer == m_clickCount
                && this.checkBox_checkImg1.Checked == m_isRect[0]
                && this.checkBox_checkImg2.Checked == m_isRect[1]
                && this.checkBox_checkImg3.Checked == m_isRect[2])
            {
                Pass = true;


                this.Close();
            }
            else if (m_tryCount >= 3)
            {
                Pass = false;


                this.Close();
            }
        }

        private void Form_BlockMacro_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void Form_BlockMacro_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        protected void DrawTestImage(int index, Graphics g)
        {
            if (m_isRect[index])
            {
                g.DrawRectangle(Pens.Black, m_drawInfo[index].X, m_drawInfo[index].Y,
                    m_drawSize[index], m_drawSize[index]);
            }
            else
            {
                g.DrawEllipse(Pens.Black, m_drawInfo[index].X, m_drawInfo[index].Y,
                    m_drawSize[index], m_drawSize[index]);
            }
        }

        private void panel_image1_Paint(object sender, PaintEventArgs e)
        {
            this.DrawTestImage(0, e.Graphics);
        }

        private void panel_image2_Paint(object sender, PaintEventArgs e)
        {
            this.DrawTestImage(1, e.Graphics);
        }

        private void panel_image3_Paint(object sender, PaintEventArgs e)
        {
            this.DrawTestImage(2, e.Graphics);
        }
    }
}
