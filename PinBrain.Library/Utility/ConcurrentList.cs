using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Collections;

namespace Pinbrain.Library.Utility
{
    /// <summary>
    /// This is a ConcurrentList class.  It locks on every call as well as provides a 
    /// ThreadSafe Enumerator
    /// </summary>
    public class ConcurrentList<T> : IList<T>, IList
    {
        private readonly List<T> underlyingList = new List<T>();
        private readonly object syncRoot = new object();
        private readonly ConcurrentQueue<T> underlyingQueue;
        private bool requiresSync;
        private bool isDirty;

        public ConcurrentList()
        {
            underlyingQueue = new ConcurrentQueue<T>();
        }

        public ConcurrentList(IEnumerable<T> items)
        {
            underlyingQueue = new ConcurrentQueue<T>(items);
        }

        private void UpdateLists()
        {
            if (!isDirty)
                return;
            lock (syncRoot)
            {
                requiresSync = true;
                T temp;
                while (underlyingQueue.TryDequeue(out temp))
                    underlyingList.Add(temp);
                requiresSync = false;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// WARNING:  ENUMERATOR.DISPOSE MUST BE EXPLICITLY CALLED OR DEADLOCK WILL OCCUR.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// WARNING:  ENUMERATOR.DISPOSE MUST BE EXPLICITLY CALLED OR DEADLOCK WILL OCCUR.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// ThreadSafe.  Adds an object to the end of the List T. 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (requiresSync)
                lock (syncRoot)
                    underlyingQueue.Enqueue(item);
            else
                underlyingQueue.Enqueue(item);
            isDirty = true;
        }

        public int Add(object value)
        {
            if (requiresSync)
                lock (syncRoot)
                    underlyingQueue.Enqueue((T)value);
            else
                underlyingQueue.Enqueue((T)value);
            isDirty = true;
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.IndexOf((T)value);
            }
        }
        /// <summary>
        /// ThreadSafe.  Adds the elements of the specified collection to the end of the List T. 
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            if (requiresSync)
                lock (syncRoot)
                {
                    foreach (T item in items)
                    {
                        underlyingQueue.Enqueue(item);
                    }
                }
            else
                foreach (T item in items)
                {
                    underlyingQueue.Enqueue(item);
                }
            isDirty = true;
        }

        public bool Contains(object value)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.Contains((T)value);
            }
        }

        public int IndexOf(object value)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.IndexOf((T)value);
            }
        }

        public void Insert(int index, object value)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.Insert(index, (T)value);
            }
        }

        public void Remove(object value)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.Remove((T)value);
            }
        }

        public void RemoveAt(int index)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (syncRoot)
                {
                    UpdateLists();
                    return underlyingList[index];
                }
            }
            set
            {
                lock (syncRoot)
                {
                    UpdateLists();
                    underlyingList[index] = value;
                }
            }
        }

        T IList<T>.this[int index]
        {
            get
            {
                lock (syncRoot)
                {
                    UpdateLists();
                    return underlyingList[index];
                }
            }
            set
            {
                lock (syncRoot)
                {
                    UpdateLists();
                    underlyingList[index] = value;
                }
            }
        }

        object IList.this[int index]
        {
            get { return ((IList<T>)this)[index]; }
            set { ((IList<T>)this)[index] = (T)value; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.Remove(item);
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.CopyTo((T[])array, index);
            }
        }

        public int Count
        {
            get
            {
                lock (syncRoot)
                {
                    UpdateLists();
                    return underlyingList.Count;
                }
            }
        }

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public int IndexOf(T item)
        {
            lock (syncRoot)
            {
                UpdateLists();
                return underlyingList.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.Insert(index, item);
            }
        }

        public void Sort()
        {
            lock (syncRoot)
            {
                UpdateLists();
                underlyingList.Sort();
            }
        }
    }

    public static class ConcurrentExtensions
    {
        public static T[] ToArray<T>(this IList<T> ListToConvert)
        {
            T[] ret = new T[ListToConvert.Count];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ListToConvert[i];
            }
            return ret;
        }
    }
}
