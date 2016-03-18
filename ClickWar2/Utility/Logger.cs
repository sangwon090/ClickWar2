using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Utility
{
    public class Logger
    {
        private Logger()
        {

        }

        //#####################################################################################

        protected static Logger s_instance = null;

        //#####################################################################################

        public static Logger GetInstance()
        {
            if (s_instance == null)
                s_instance = new Logger();

            return s_instance;
        }

        //#####################################################################################

        public event Action<string> WhenLogAdded;

        protected Queue<string> m_log = new Queue<string>();

        public int MaxLogCount
        { get; set; } = 256;

        //#####################################################################################

        public void Log(string message)
        {
            m_log.Enqueue(message);

            while (m_log.Count > this.MaxLogCount)
                m_log.Dequeue();


            if (WhenLogAdded.GetInvocationList().Length > 0)
            {
                WhenLogAdded(message);
            }
        }
    }
}
