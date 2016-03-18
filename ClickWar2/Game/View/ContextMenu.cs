using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickWar2.Game.View
{
    public class ContextMenu : IContextMenu
    {
        public ContextMenu()
        {

        }

        //#####################################################################################

        protected class Menu
        {
            public string Text
            { get; set; }

            public Action WhenClicked;
        }

        protected Dictionary<string, Menu> m_tagMap = new Dictionary<string, Menu>();
        protected List<Menu> m_menuList = new List<Menu>();

        public int Width
        { get; set; }

        //#####################################################################################

        public bool Visible
        { get; set; } = false;

        public Point Location
        { get; set; }

        public Point Cursor
        { get; set; }

        public bool Focused
        {
            get
            {
                return (this.GetIndexFocused() >= 0);
            }
        }

        //#####################################################################################

        public void Draw(Graphics g)
        {
            if (this.Visible)
            {
                int menuHeight = SystemFonts.DefaultFont.Height + 1;
                int indexFocused = this.GetIndexFocused();


                g.FillRectangle(Brushes.WhiteSmoke,
                    this.Location.X, this.Location.Y,
                    this.Width, m_menuList.Count * menuHeight + 4);

                g.DrawRectangle(Pens.Black,
                    this.Location.X, this.Location.Y,
                    this.Width, m_menuList.Count * menuHeight + 4);


                int currentX = this.Location.X + 2;
                int currentY = this.Location.Y + 4;

                int index = 0;
                foreach (Menu menu in m_menuList)
                {
                    if (index == indexFocused)
                    {
                        g.FillRectangle(Brushes.Aqua,
                            this.Location.X, this.Location.Y + index * menuHeight + 2,
                            this.Width, menuHeight);
                    }


                    g.DrawString(menu.Text, SystemFonts.DefaultFont, Brushes.Black,
                        currentX, currentY);

                    currentY += menuHeight;


                    ++index;
                }
            }
        }

        //#####################################################################################

        public void AddMenu(string tag, string text, Action callback)
        {
            Menu menu = new Menu();
            menu.Text = text;
            menu.WhenClicked = callback;

            if (m_tagMap.ContainsKey(tag))
                m_tagMap[tag] = menu;
            else
                m_tagMap.Add(tag, menu);

            m_menuList.Add(menu);


            var menuSize = TextRenderer.MeasureText(text, SystemFonts.DefaultFont);
            menuSize.Width += 4;

            if (menuSize.Width > this.Width)
                this.Width = menuSize.Width;
        }

        public void RemoveMenu(string tag)
        {
            if (m_tagMap.ContainsKey(tag))
            {
                m_menuList.Remove(m_tagMap[tag]);
                m_tagMap.Remove(tag);
            }
        }

        public void Clear()
        {
            m_menuList.Clear();
            m_tagMap.Clear();

            this.Width = 0;
        }

        //#####################################################################################

        public void OnClick(Point cursor)
        {
            this.Cursor = cursor;
            int index = this.GetIndexFocused();

            if (index >= 0)
            {
                m_menuList[index].WhenClicked();
            }
        }

        //#####################################################################################

        protected int GetIndexFocused()
        {
            if (this.Cursor.X > this.Location.X
                && this.Cursor.X < this.Location.X + this.Width)
            {
                int subY = this.Cursor.Y - this.Location.Y;

                if (subY < 0)
                    return -1;

                int index = subY / (SystemFonts.DefaultFont.Height + 1);
                if (index < 0 || index >= m_menuList.Count)
                    index = -1;

                return index;
            }


            return -1;
        }
    }
}
