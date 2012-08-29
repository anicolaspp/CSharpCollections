using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nikos.Collections
{
    [Serializable]
    public class BinomialHeap<T> : IPriorityQueue<T>, ICloneable<BinomialHeap<T>> where T : IComparable<T>
    {
        private HeapItem<T>[] m_data;
        private readonly HeapTypePriority m_type;
        private readonly int m_factor;

        private static int FatherPosition(int position)
        {
            return position % 2 == 0 ? (position - 2) / 2 : (position - 1) / 2;
        }

        private void HeadpifyUp(int position)
        {
            while (position > 0 && m_data[position].Priority < m_data[FatherPosition(position)].Priority)
            {
                var temp = m_data[position];
                m_data[position] = m_data[FatherPosition(position)];
                m_data[FatherPosition(position)] = temp;

                position = FatherPosition(position);
            }
        }
        private void HeadpifyDown(int position)
        {
            if (2 * position + 2 >= Count)
                return;
            if (m_data[position].Priority < m_data[2 * position + 1].Priority && m_data[position].Priority < m_data[2 * position + 2].Priority)
                return;
            if (m_data[position].Priority > m_data[2 * position + 1].Priority && m_data[position].Priority < m_data[2 * position + 2].Priority)
            {
                var temp = m_data[position];
                m_data[position] = m_data[2 * position + 1];
                m_data[2 * position + 1] = temp;
                HeadpifyDown(2 * position + 1);
            }
            else if (m_data[position].Priority < m_data[2 * position + 1].Priority && m_data[position].Priority > m_data[2 * position + 2].Priority)
            {
                var temp = m_data[position];
                m_data[position] = m_data[2 * position + 2];
                m_data[2 * position + 2] = temp;
                HeadpifyDown(2 * position + 2);
            }
            else
            {
                var minPosition = m_data[2 * position + 1].Priority <= m_data[2 * position + 2].Priority ? 2 * position + 1 : 2 * position + 2;

                var temp = m_data[position];
                m_data[position] = m_data[minPosition];
                m_data[minPosition] = temp;
                HeadpifyDown(minPosition);
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="heapType"></param>
        public BinomialHeap(HeapTypePriority heapType = HeapTypePriority.MinHeap)
            : this(16, heapType)
        {
        }
        ///<summary>
        ///</summary>
        ///<param name="capacity"></param>
        ///<param name="heapType"></param>
        public BinomialHeap(int capacity, HeapTypePriority heapType = HeapTypePriority.MinHeap)
        {
            Capacity = capacity;
            Count = 0;
            m_data = new HeapItem<T>[capacity];
            m_type = heapType;

            if (m_type == HeapTypePriority.MinHeap) m_factor = 1; else m_factor = -1;
        }
        
        private BinomialHeap(HeapItem<T>[] data, HeapTypePriority heapType)
        {
            Capacity = 2 * data.Length;
            Count = data.Length;
            m_data = new HeapItem<T>[Capacity];
            data.CopyTo(m_data, 0);
            m_type = heapType;

            if (m_type == HeapTypePriority.MinHeap) m_factor = 1; else m_factor = -1;
        }

        ///<summary>
        /// Create a heap based in a IEnumerable with data, the construction is O(n) amortized cost
        ///</summary>
        ///<param name="souce">Data to build the heap</param>
        ///<param name="func">Extract the priority from the item</param>
        ///<param name="heapType"></param>
        ///<typeparam name="TK"></typeparam>
        ///<returns></returns>
        public static BinomialHeap<TK> Build_Heap<TK>(IEnumerable<TK> souce, Func<TK, int> func, HeapTypePriority heapType = HeapTypePriority.MinHeap)
            where TK : IComparable<TK>
        {
            //nice declaration for inline-function...
            Func<TK, int> pFunc = heapType == HeapTypePriority.MaxHeap ? x => -1 * func(x) : func;

            var data = (from k in souce
                       select new HeapItem<TK>(k, pFunc(k))).ToArray();

            var result = new BinomialHeap<TK>(data, heapType);
            for (int i = result.m_data.Length/2 - 1; i >= 0; i--)
                result.HeadpifyUp(i);

            return result;
        }

        public void DecreaseKey(T item, int priority)
        {
            for (int i = 0; i < Count; i++)
                if (m_data[i].Value.CompareTo(item) == 0)
                {
                    if (priority >= m_data[i].Priority) return;

                    m_data[i].Priority = priority;
                    HeadpifyUp(i);
                    return;
                }
        }

        /// <summary>
        /// Get the item in the top of heap without remove it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (Count > 0)
                return m_data[0].Value;
            throw new InvalidOperationException("Heap empty");
        }

        /// <summary>
        /// Get the itemin the top of heap and remove it
        /// </summary>
        /// <returns></returns>
        public T DeQueue()
        {
            if (Count > 0)
            {
                T result = m_data[0].Value;
                m_data[0] = m_data[Count - 1];
                HeadpifyDown(0);
                Count--;

                return result;
            }
            throw new InvalidOperationException("Heap empty");
        }

        /// <summary>
        /// Enqueue item into heap with a priority
        /// </summary>
        /// <param name="item">item to enqueue</param>
        /// <param name="priority">priority of heap</param>
        public void EnQueue(T item, int priority)
        {
            if (Count == Capacity)
            {
                var x = (HeapItem<T>[])m_data.Clone();
                m_data = new HeapItem<T>[Capacity * 2];
                Array.Copy(x, m_data, Count);
                Capacity *= 2;
            }

            m_data[Count++] = new HeapItem<T>(item, priority * m_factor);
            HeadpifyUp(Count - 1);
        }

        /// <summary>
        /// Determine if the heap constain a especific item with a especific priority
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public bool Contains(T item, int priority)
        {
            priority *= m_factor;
            int pos = Count - 1;

            while (pos > 0 && m_data[pos].Priority <= priority)
            {
                if (m_data[pos].Value.CompareTo(item) == 0 && m_data[pos].Priority == priority)
                    return true;
                pos = FatherPosition(pos);
            }
            return false;
        }

        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<returns></returns>
        public bool Contains(T item)
        {
            return m_data.Any(heapItem => heapItem.Value.CompareTo(item) == 0);
        }
        
        public int Capacity { get; protected set; }
        
        public int Count { get; protected set; }
        
        public HeapTypePriority HeapType { get { return m_type; } }

        /// <summary>
        /// Get if heap is empty
        /// </summary>
        public bool IsEmpty { get { return Count == 0; } }

        #region Implementation of IEnumerable

        /// <summary>
        ///                     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///                     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            var temp = Clone();
            while (!temp.IsEmpty)
                yield return temp.DeQueue();
        }

        /// <summary>
        ///                     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///                     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICloneable

        /// <summary>
        ///  Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public BinomialHeap<T> Clone()
        {
            return new BinomialHeap<T>(m_data, m_type);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}