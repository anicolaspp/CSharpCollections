using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos.Collections.Bases
{
    /// <summary>
    /// Describe que tipo de Heap se va a utilizar
    /// </summary>
    public enum HeapType
    {
        MaxHeap, MinHeap
    }
    
    [Serializable]
    public class Heap<T> : IEnumerable<T>
    {
        HeapItem<T>[] data;

        public int Capacity { get; protected set; }
        public int Count { get; protected set; }
        public HeapType HeapType { get; protected set; }

        private int FatherPos(int position)
        {
            return position % 2 == 0 ? (position - 1) / 2 : (position - 2) / 2;
        }

        private int Left(int position)
        {
            return 2 * position + 2;
        }
        private int Rigth(int position)
        {
            return 2 * position + 1;
        }

        private void HeapifyUp(int position)
        {
            int value = data[position].Priority;
            int pos = position;

            while (pos != 0 || value > data[FatherPos(pos)].Priority)
            {
                HeapItem<T> temp = data[position];
                data[position] = data[FatherPos(pos)];
                data[FatherPos(pos)] = temp;

                pos = FatherPos(pos);
                value = data[position].Priority;
            }
        }
        private void HeapfyDown(int position)
        {
            int left = Left(position);
            int rigth = Rigth(position);

            if (rigth < Count)
            {
                if (data[left].Priority >= data[rigth].Priority)
                {
                    if (data[position].Priority < data[left].Priority)
                    {
                        var temp = data[position];
                        data[position] = data[left];
                        data[left] = temp;

                        HeapfyDown(left);
                    }
                }
                else
                {
                    if (data[position].Priority < data[rigth].Priority)
                    {
                        var temp = data[position];
                        data[position] = data[rigth];
                        data[rigth] = temp;

                        HeapfyDown(rigth);
                    }
                }
            }
            else if (left < Count)
            {
                if (data[position].Priority < data[left].Priority)
                {
                    var temp = data[position];
                    data[position] = data[left];
                    data[left] = temp;

                    HeapfyDown(left);
                }
            }

            //bool fl = true;
            //if (pB < Count)
            //    fl = data[pA].Priority <= data[pB].Priority;

            //if (pA < Count && data[ChildrensPos(position)[0]].Priority < data[pA].Priority && fl)
            //    //CAMBIAR CON EL HIJO IZQUIERDO
            //{
            //    HeapItem<T> temp = data[pA];
            //    data[pA] = data[position];
            //    data[position] = temp;

            //    HeapfyDown(pA);
            //}
            //if (pB < Count && data[ChildrensPos(position)[1]].Priority < data[pB].Priority && !fl)
            //    //CAMBIAR CON EL HIJO DERECHO
            //{
            //    HeapItem<T> temp = data[pB];
            //    data[pB] = data[position];
            //    data[position] = temp;

            //    HeapfyDown(pB);
            //}
        }

        public Heap(HeapType heapType)
            : this(16, heapType)
        {
        }

        public Heap(int capacity, HeapType heapType)
        {
            this.Capacity = capacity;
            this.Count = 0;
            this.HeapType = heapType;
            data = new HeapItem<T>[capacity];
        }
        public static Heap<L> Build_Heap<L>(IEnumerable<L> collection, Func<L, int> function, HeapType heapType)
        {
            var data = collection.ToArray();
            var result = new Heap<L>(data.Length, heapType)
                             {
                                 data = (heapType == HeapType.MaxHeap
                                             ? (from x in data
                                                select new HeapItem<L>(x, function(x))).ToArray()
                                             : (from x in data
                                                select new HeapItem<L>(x, -function(x))).ToArray())
                             };
            result.Count = data.Length;
            for (int i = result.data.Length / 2; i >= 0; i--)
                result.HeapfyDown(i);
            
            return result;
        }

        public void EnQueue(T item, int priority)
        {
            if (HeapType == HeapType.MinHeap)
                priority = -priority;

            if (Count >= Capacity)                      //DUPLICANDO LA CAPACIDAD
            {
                var temp = (HeapItem<T>[])data.Clone();
                data = new HeapItem<T>[data.Length * 2];
                Array.Copy(temp, data, temp.Length);
            }

            data[Count++] = new HeapItem<T>(item, priority);
            HeapifyUp(Count - 1);
        }
        public void EnQueue(T item, Func<T, int> function)
        {
            EnQueue(item, function(item));
        }
        public void EnQueue(IEnumerable<T> collection, Func<T, int> function)
        {
            foreach (var item in collection)
                EnQueue(item, function);
        }

        public T Peek()
        {
            if (Count > 0)
                return data[0].Value;
            throw new InvalidOperationException("Heap empty");
        }
        public T DeQueue()
        {
            if (Count > 0)
            {
                T result = data[0].Value;
                data[0] = data[Count - 1];
                HeapfyDown(0);
                Count--;

                return result;
            }
            throw new InvalidOperationException("Heap empty");
        }

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
            var temp = (HeapItem<T>[])data.Clone();

            while (Count > 0)
                yield return DeQueue();

            data = temp;
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
    }
}
