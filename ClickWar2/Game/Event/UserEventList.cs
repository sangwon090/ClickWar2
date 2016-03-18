using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Game.Event
{
    public class UserEventList
    {
        public UserEventList()
        {

        }

        //#####################################################################################

        public delegate void UserEventDelegate(string userName, object[] args);
        protected List<UserEventDelegate> m_eventList = new List<UserEventDelegate>();

        //#####################################################################################

        public void SetEvent(int eventNum, UserEventDelegate callback)
        {
            while (eventNum >= m_eventList.Count)
                m_eventList.Add(null);

            m_eventList[eventNum] = callback;
        }

        public void StartEvent(int eventNum,
            string userName, object[] args = null)
        {
            if (eventNum < m_eventList.Count
                && m_eventList[eventNum] != null)
            {
                m_eventList[eventNum](userName, args);
            }
        }
    }
}
