using System;

namespace Nikos.Collections
{
    [Serializable]
    public class Pair<T, K> : IComparable<Pair<T, K>> where T : IComparable<T>
    {
        public T Key { get; set; }
        public K Value { get; set; }



        #region Implementation of IComparable<in Pair<T,K>>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Pair<T, K> other)
        {
            return other != null ? Key.CompareTo(other.Key) : 1;
        }

        #endregion
    }
}