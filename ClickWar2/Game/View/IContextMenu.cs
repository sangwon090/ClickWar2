using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar2.Game.View
{
    public interface IContextMenu
    {
        bool Visible
        { get; set; }

        Point Location
        { get; set; }

        Point Cursor
        { get; set; }

        bool Focused
        { get; }

        //#####################################################################################

        void Draw(Graphics g);

        //#####################################################################################

        void AddMenu(string tag, string text, Action callback);
        void RemoveMenu(string tag);
        void Clear();

        //#####################################################################################

        void OnClick(Point cursor);
    }
}
