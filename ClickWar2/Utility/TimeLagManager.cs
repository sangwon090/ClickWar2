using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Utility
{
    public class TimeLagManager
    {
        public TimeLagManager()
        {

        }

        //#####################################################################################

        protected Dictionary<int, int> m_minTimeLagMap = new Dictionary<int, int>();
        protected Dictionary<int, DateTime> m_prevTimeMap = new Dictionary<int, DateTime>();

        //#####################################################################################

        public bool Set(int id, int minTimeLag)
        {
            if (m_minTimeLagMap.ContainsKey(id))
            {
                m_minTimeLagMap[id] = minTimeLag;
            }
            else
            {
                m_minTimeLagMap.Add(id, minTimeLag);
                m_prevTimeMap.Add(id, DateTime.MinValue);


                return true;
            }


            return false;
        }

        public bool Tick(int id)
        {
            var span = DateTime.Now - m_prevTimeMap[id];
            return (span.TotalMilliseconds >= m_minTimeLagMap[id]);
        }

        public void Update(int id)
        {
            m_prevTimeMap[id] = DateTime.Now;
        }

        public int LeftTime(int id)
        {
            var span = DateTime.Now - m_prevTimeMap[id];
            int leftTime = m_minTimeLagMap[id] - (int)span.TotalMilliseconds;

            if (leftTime < 0) leftTime = 0;

            return leftTime;
        }

        public void Run(int id)
        {
            m_prevTimeMap[id] = DateTime.MinValue;
        }
    }
}
