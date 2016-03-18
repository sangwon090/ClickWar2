using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Game.Effect
{
    public class CircleEffect : Effect
    {
        public CircleEffect()
        {

        }

        public CircleEffect(int centerX, int centerY, int beginRadius, int endRadius, int growSpeed,
            Color color)
            : base(centerX, centerY)
        {
            this.Radius = beginRadius;
            this.BeginRadius = beginRadius;
            this.EndRadius = endRadius;
            this.GrowSpeed = growSpeed;
            this.CircleColor = color;

            m_brush = new SolidBrush(color);
        }

        ~CircleEffect()
        {
            if (m_brush != null)
            {
                m_brush.Dispose();
            }
        }

        //#####################################################################################

        public int Radius
        { get; set; } = 1;

        public int BeginRadius
        { get; set; } = 1;

        public int EndRadius
        { get; set; } = 2;

        public int GrowSpeed
        { get; set; } = 1;

        public Color CircleColor
        { get; set; }

        //#####################################################################################

        protected Brush m_brush = null;

        //#####################################################################################

        public override void UpdateAndDraw(Graphics g)
        {
            int viewRadius = this.Radius + m_addScale;


            this.Radius += this.GrowSpeed;


            if (viewRadius > 0)
            {
                if (m_brush != null)
                {
                    g.FillEllipse(m_brush,
                        m_centerX - viewRadius,
                        m_centerY - viewRadius,
                        viewRadius << 1, viewRadius << 1);


                    m_brush.Dispose();
                    m_brush = null;
                }


                float alpha = 255.0f - (float)this.Radius / this.EndRadius * 255.0f;

                if (alpha >= 1.0f)
                {
                    m_brush = new SolidBrush(Color.FromArgb((int)alpha, this.CircleColor));
                }
            }
        }

        public override bool IsEnd()
        {
            return (this.Radius >= this.EndRadius);
        }
    }
}
