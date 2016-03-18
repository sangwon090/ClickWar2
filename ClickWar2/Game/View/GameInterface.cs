using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ClickWar2.Game.View
{
    public class GameInterface : IGameInterface
    {
        public GameInterface()
        {

        }

        ~GameInterface()
        {
            m_attackCooltimePen.Dispose();

            m_eventFont.Dispose();
        }

        //#####################################################################################

        protected Pen m_attackCooltimePen = new Pen(Color.Red, 2.5f);

        //#####################################################################################

        public Point Cursor
        { get; set; }

        public int LeftAddPowerCooltime
        { get; set; }

        public int LeftAttackCooltime
        { get; set; }

        public Point TileCursor
        { get; set; }

        public Tile FocusTile
        { get; set; }

        //#####################################################################################
        // 이벤트 목록

        protected EventMessageManager m_eventList = new EventMessageManager();

        protected Font m_eventFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

        /// <summary>
        /// 0~255 사이의 투명도를 얻거나 설정합니다.
        /// </summary>
        public float EventOpacity
        { get; set; } = 255.0f;

        public int EventListMaxViewHeight
        { get { return m_eventList.MaxEventCount * (m_eventFont.Height + 1); } }

        //#####################################################################################

        public void AddEventMessage(string msg, Color color)
        {
            m_eventList.AddEvent(msg, color);


            // 투명도 초기화
            this.EventOpacity = 255.0f;
        }

        //#####################################################################################

        public void DrawCooltime(Graphics g)
        {
            if (this.LeftAddPowerCooltime > 0)
            {
                const int size = 14;

                g.FillEllipse(Brushes.ForestGreen,
                    this.Cursor.X - size / 2, this.Cursor.Y - size / 2,
                    size, size);
            }

            if (this.LeftAttackCooltime > 0)
            {
                float size = (float)this.LeftAttackCooltime / GameValues.MinAttackDelay * 32.0f;

                g.DrawEllipse(m_attackCooltimePen,
                    this.Cursor.X - size / 2, this.Cursor.Y - size / 2,
                    size, size);
            }
        }

        public void DrawCursorTileDetail(Graphics g)
        {
            StringBuilder tileInfo = new StringBuilder();


            tileInfo.Append('(');
            tileInfo.Append(this.TileCursor.X);
            tileInfo.Append(", ");
            tileInfo.Append(this.TileCursor.Y);
            tileInfo.Append(")");


            g.DrawString(tileInfo.ToString(), SystemFonts.DefaultFont, Brushes.Black,
                new Rectangle(this.Cursor.X - 256, this.Cursor.Y - 8, 244, 16),
                new StringFormat()
                {
                    Alignment = StringAlignment.Far
                });


            tileInfo.Clear();


            if (this.FocusTile != null
                && this.FocusTile.Power != 0)
            {
                if (this.FocusTile.HaveOwner)
                {
                    tileInfo.Append("Owner: \"");
                    tileInfo.Append(this.FocusTile.Owner);
                    tileInfo.Append("\"\n");
                }

                tileInfo.Append("Power: ");
                tileInfo.Append(this.FocusTile.Power);


                g.DrawString(tileInfo.ToString(), SystemFonts.DefaultFont, Brushes.Black,
                    this.Cursor.X + 12, this.Cursor.Y,
                    new StringFormat()
                    {
                        // 세로 중앙정렬
                        LineAlignment = StringAlignment.Center
                    });
            }
        }

        public void UpdateAndDrawEventList(Graphics g, int x, int y)
        {
            if (this.EventOpacity >= 1.0f && m_eventList.Count > 0)
            {
                int backHeight = this.EventListMaxViewHeight;
                Rectangle backRect = new Rectangle(x - 4, y - backHeight - 4, 512, backHeight + 4);

                using (Brush backBrh = new System.Drawing.Drawing2D.LinearGradientBrush(backRect,
                    Color.FromArgb((int)(this.EventOpacity * 0.6f), Color.WhiteSmoke),
                    Color.FromArgb(0, Color.WhiteSmoke),
                    System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                {
                    // 배경 그리기
                    g.FillRectangle(backBrh, backRect);


                    // 이벤트 그리기

                    Color prevColor = Color.FromArgb((int)this.EventOpacity, Color.Black);
                    Brush prevBrh = new SolidBrush(prevColor);


                    int msgHeight = m_eventFont.Height + 1;


                    int currentY = y - m_eventList.Count * msgHeight;

                    foreach (var eventInfo in m_eventList)
                    {
                        var color = Color.FromArgb((int)this.EventOpacity, eventInfo.TextColor);

                        if (color != prevColor)
                        {
                            prevColor = color;
                            prevBrh.Dispose();
                            prevBrh = new SolidBrush(color);
                        }


                        StringBuilder eventText = new StringBuilder(eventInfo.Message);

                        if (eventInfo.Count > 1)
                        {
                            eventText.Append(" (");
                            eventText.Append(eventInfo.Count);
                            eventText.Append(')');
                        }

                        g.DrawString(eventText.ToString(), m_eventFont, prevBrh,
                            x, currentY);


                        currentY += msgHeight;
                    }


                    prevBrh.Dispose();


                    // 투명도 갱신
                    this.EventOpacity -= 0.4f;
                }
            }
        }
    }
}
