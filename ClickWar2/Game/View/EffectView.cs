using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar2.Game.View
{
    public class EffectView : IEffectView
    {
        public EffectView()
        {

        }

        //#####################################################################################

        protected int Scale
        { get; set; } = 0;

        protected List<Effect.Effect> m_effectList = new List<Effect.Effect>();

        //#####################################################################################

        public void UpdateAndDrawEffect(Graphics g)
        {
            for (int i = 0; i < m_effectList.Count; ++i)
            {
                var effect = m_effectList[i];


                effect.UpdateAndDraw(g);

                if (effect.IsEnd())
                {
                    m_effectList.RemoveAt(i);
                    --i;
                }
            }
        }

        public void MoveAllEffect(int deltaX, int deltaY)
        {
            foreach (var effect in m_effectList)
            {
                effect.AddLocation(deltaX, deltaY);
            }
        }

        public void AddScaleAllEffect(int deltaScale)
        {
            this.Scale += deltaScale;

            foreach (var effect in m_effectList)
            {
                effect.AddScale(deltaScale);
            }
        }

        //#####################################################################################

        public void AddCircleEffect(int centerX, int centerY, int beginRadius, int endRadius, int growSpeed,
            Color color)
        {
            var newEffect = new Effect.CircleEffect(centerX, centerY, beginRadius, endRadius, growSpeed,
                color);
            newEffect.AddScale(this.Scale);

            m_effectList.Add(newEffect);
        }
    }
}
