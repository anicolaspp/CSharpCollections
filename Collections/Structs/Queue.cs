using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos.Collections
{
    public class Queue<T> : IEnumerable<T>
    {
        private LinkedList<T> data;

        public int Count { get { return data.Count; } }
        public int Capacity { get; protected set; }

        public Queue(int capacity = 16)
        {
            Capacity = capacity;
            data = new LinkedList<T>();
        }

        public Queue(IEnumerable<T> source)
        {
            data = new LinkedList<T>(source);
            Capacity = data.Count;
        }

        public void EnQueue(T value)
        {
            if (Count < Capacity)
                data.AddLast(value);
            else
                throw new InvalidOperationException();
        }

        public T DeQueue()
        {
            var result = data.First.Value;
            data.RemoveFirst();
            return result;
        }

        public T Top()
        {
            return data.First.Value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in data)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
