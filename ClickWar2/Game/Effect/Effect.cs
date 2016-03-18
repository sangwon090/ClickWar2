using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar2.Game.Effect
{
    public abstract class Effect
    {
        public Effect()
        {

        }

        public Effect(int centerX, int centerY)
        {
            m_centerX = centerX;
            m_centerY = centerY;
        }

        //#####################################################################################

        protected int m_centerX = 0, m_centerY = 0;
        protected int m_addScale = 0;

        //#####################################################################################

        public void AddLocation(int deltaX, int deltaY)
        {
            m_centerX += deltaX;
            m_centerY += deltaY;
        }

        public void AddScale(int deltaScale)
        {
            m_addScale += deltaScale;
        }

        //#####################################################################################

        public abstract void UpdateAndDraw(Graphics g);
        public abstract bool IsEnd();
    }
}
