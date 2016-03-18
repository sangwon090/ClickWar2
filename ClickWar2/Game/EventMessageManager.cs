using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace ClickWar2.Game
{
    public class EventMessageManager : IEnumerable<EventMessageManager.EventMessage>
    {
        public EventMessageManager()
        {
            
        }

        //#####################################################################################

        public class EventMessage
        {
            public string Message
            { get; set; }

            public int Count
            { get; set; }

            public Color TextColor
            { get; set; }
        }

        protected List<EventMessage> m_msgList = new List<EventMessage>();
        public int Count
        { get { return m_msgList.Count; } }

        public int MaxEventCount
        { get; set; } = 20;

        //#####################################################################################

        public void AddEvent(string msg, Color color)
        {
            // 가장 최근 이벤트와 내용이 같은지 확인
            if (m_msgList.Count > 0)
            {
                var latestMsg = m_msgList.Last();

                // 같으므로 횟수만 증가시킴
                if (latestMsg.Message == msg)
                {
                    ++latestMsg.Count;
                    return;
                }
            }


            // 새로운 이벤트 이므로 추가함

            var newEvent = new EventMessage()
            {
                Message = msg,
                Count = 1,
                TextColor = color
            };

            m_msgList.Add(newEvent);


            // 등록된 이벤트 개수가 최대값을 넘었으면 가장 오래된 이벤트 제거
            if (m_msgList.Count > this.MaxEventCount)
            {
                m_msgList.RemoveRange(0, m_msgList.Count - this.MaxEventCount);
            }
        }

        //#####################################################################################

        public IEnumerator<EventMessage> GetEnumerator()
        {
            return m_msgList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_msgList.GetEnumerator();
        }
    }
}
