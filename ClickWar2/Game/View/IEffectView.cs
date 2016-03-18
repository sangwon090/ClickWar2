using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar2.Game.View
{
    public interface IEffectView
    {
        void MoveAllEffect(int deltaX, int deltaY);
        void AddScaleAllEffect(int deltaScale);

        //#####################################################################################

        void AddCircleEffect(int centerX, int centerY, int beginRadius, int endRadius, int growSpeed,
            Color color);
    }
}
