using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar2.Game.View
{
    public interface IGameInterface
    {
        Point Cursor
        { get; set; }

        int LeftAddPowerCooltime
        { get; set; }

        int LeftAttackCooltime
        { get; set; }

        Point TileCursor
        { get; set; }

        Tile FocusTile
        { get; set; }

        //#####################################################################################

        void AddEventMessage(string msg, Color color);
    }
}
