using System;

namespace Nikos.Collections.Bases
{
    [Serializable]
    public class Pair<T, T1> : IComparable where T : IComparable
    {
        public T Key { get; set; }
        public T1 Value { get; set; }

        #region Implementation of IComparable

        /// <summary>
        /// Compares the current instance with another object of the same type.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than <paramref name="obj" />. Zero This instance is equal to <paramref name="obj" />. Greater than zero This instance is greater than <paramref name="obj" />. 
        /// </returns>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="obj" /> is not the same type as this instance. </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            var other = obj as Pair<T, T1>;

            return other != null ? Key.CompareTo(other.Key) : 1;
        }

        #endregion
    }
}