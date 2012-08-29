using System;

namespace Nikos.Collections
{
    class SkipNode<T> : IComparable<SkipNode<T>> where T : IComparable
    {
        public int key;
        public T value;
        public SkipNode<T>[] link;
        public SkipNode(int level, int key, T value)
        {
            this.key = key;
            this.value = value;
            link = new SkipNode<T>[level];
        }

        #region Implementation of IComparable<in SkipNode<T>>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(SkipNode<T> other)
        {
            return value.CompareTo(other.value);
        }

        #endregion
    }
}