using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Utility
{
    public class SafeQueue<T>
    {
        public SafeQueue()
        {

        }

        //#####################################################################################

        protected Queue<T> m_queue = new Queue<T>();
        protected readonly object m_lockObj = new object();

        //#####################################################################################

        public void Push(T item)
        {
            lock(m_lockObj)
            {
                m_queue.Enqueue(item);
            }
        }

        public T Pop()
        {
            T temp;

            lock(m_lockObj)
            {
                if (m_queue.Count > 0)
                    temp = m_queue.Dequeue();
                else
                    temp = default(T);
            }

            return temp;
        }

        public void Clear()
        {
            lock(m_lockObj)
            {
                m_queue.Clear();
            }
        }

        //#####################################################################################

        public int Count
        {
            get
            {
                int count = 0;

                lock(m_lockObj)
                {
                    count = m_queue.Count;
                }

                return count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.Count <= 0);
            }
        }

        public T Head
        {
            get
            {
                T temp;

                lock (m_lockObj)
                {
                    temp = m_queue.Peek();
                }

                return temp;
            }
        }
    }
}
