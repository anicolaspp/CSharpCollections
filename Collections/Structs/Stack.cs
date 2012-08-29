using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nikos.Collections
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public class Stack<T> : IEnumerable<T>
    {
        private T[] data;
        ///<summary>
        ///</summary>
        public int Count { get; protected set; }
        ///<summary>
        ///</summary>
        public int Capacity { get; protected set; }

        ///<summary>
        ///</summary>
        ///<param name="capacity"></param>
        public Stack(int capacity = 16)
        {
            Capacity = capacity;
            data = new T[capacity];
        }

        ///<summary>
        ///</summary>
        ///<param name="source"></param>
        public Stack(IEnumerable<T> source)
        {
            var values = source.ToArray();
            data = new T[values.Length];
            Capacity = values.Length;

            foreach (var value in values)
                Push(value);
        }

        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        ///<exception cref="InvalidOperationException"></exception>
        public void Push(T value)
        {
            if (Count < Capacity)
                data[Count++] = value;
            else
                throw new InvalidOperationException();
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        ///<exception cref="InvalidOperationException"></exception>
        public T Pop()
        {
            if (Count > 0)
                return data[--Count];
            throw new InvalidOperationException();
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        ///<exception cref="InvalidOperationException"></exception>
        public T Pick()
        {
            if (Count > 0)
                return data[Count - 1];
            throw new InvalidOperationException();
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            if (Count > 0)
                for (int i = Count - 1; i >= 0; i--)
                    yield return data[i];
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
