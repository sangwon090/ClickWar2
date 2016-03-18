using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Utility
{
    public class SafeList<T> : IEnumerable<T>
    {
        public SafeList()
        {

        }

        //#####################################################################################

        protected List<T> m_list = new List<T>();
        protected readonly object m_lockObj = new object();

        //#####################################################################################

        public int Count
        {
            get
            {
                int count = 0;

                lock(m_lockObj)
                {
                    count = m_list.Count;
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

        //#####################################################################################

        public void Add(T item)
        {
            lock(m_lockObj)
            {
                m_list.Add(item);
            }
        }

        public void Remove(T item)
        {
            lock(m_lockObj)
            {
                m_list.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock(m_lockObj)
            {
                m_list.RemoveAt(index);
            }
        }

        public void Clear()
        {
            lock(m_lockObj)
            {
                m_list.Clear();
            }
        }

        //#####################################################################################

        public void SetItem(int index, T newItem)
        {
            lock(m_lockObj)
            {
                if(index < m_list.Count)
                    m_list[index] = newItem;
            }
        }

        public T GetItem(int index)
        {
            T temp;

            lock(m_lockObj)
            {
                if (index < m_list.Count)
                    temp = m_list[index];
                else
                    temp = default(T);
            }

            return temp;
        }

        //#####################################################################################

        public T this[int index]
        {
            get
            {
                return GetItem(index);
            }
            set
            {
                SetItem(index, value);
            }
        }

        //#####################################################################################

        public IEnumerator<T> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        public T[] GetArray()
        {
            T[] result;

            lock(m_lockObj)
            {
                result = m_list.ToArray();
            }

            return result;
        }

        //#####################################################################################

        public void DoSync(Action<List<T>> func)
        {
            lock(m_lockObj)
            {
                func(m_list);
            }
        }
    }
}
