using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Game.Event
{
    public class GameEventList
    {
        public GameEventList()
        {

        }

        //#####################################################################################

        public delegate void GameEventDelegate(int tileX, int tileY, string activeUser, object[] args);
        protected List<GameEventDelegate> m_eventList = new List<GameEventDelegate>();

        //#####################################################################################

        public void SetEvent(int eventNum, GameEventDelegate callback)
        {
            while (eventNum >= m_eventList.Count)
                m_eventList.Add(null);

            m_eventList[eventNum] = callback;
        }

        public void StartEvent(int eventNum,
            int tileX, int tileY, string activeUser, object[] args = null)
        {
            if (eventNum < m_eventList.Count
                && m_eventList[eventNum] != null)
            {
                m_eventList[eventNum](tileX, tileY, activeUser, args);
            }
        }
    }
}
