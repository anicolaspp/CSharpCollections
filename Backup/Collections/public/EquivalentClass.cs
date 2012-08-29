using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nikos.Collections.Advance;

namespace Nikos.Collections.Bases
{
    public class EquivalentClass : IEnumerable<IComparable>, IComparable, ICloneable
    {
        private AVL_Tree<IComparable> data;

        private EquivalentClass(IEnumerable<IComparable> collection)
        {
            data = new AVL_Tree<IComparable>(collection.ToArray());
            Representative = data.Root;
        }
        public EquivalentClass(IComparable x)
        {
            data = new AVL_Tree<IComparable> { x };
            Representative = data.Root;
        }

        public EquivalentClass Merge(EquivalentClass x)
        {
            if (CompareTo(x) != 0)
                return null;

            AVL_Tree<IComparable> result;
            if (Count >= x.Count)
            {
                result = (AVL_Tree<IComparable>)data.Clone();
                foreach (var item in x)
                    result.Add(item);
            }
            else
            {
                result = (AVL_Tree<IComparable>)x.data.Clone();
                foreach (var item in this)
                    result.Add(item);
            }
            return new EquivalentClass(result);
        }
        public bool Add(IComparable item)
        {
            if (Representative.CompareTo(item) != 0)
                return false;
            data.Add(item);
            return true;
        }

        public IComparable Representative { get; protected set; }
        public int Count { get { return data.Count; } }

        #region Implementation of IEnumerable

        /// <summary>
        ///                     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///                     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IComparable> GetEnumerator()
        {
            return data.GetEnumerator();
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

        #region Implementation of IComparable

        /// <summary>
        ///                     Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        ///                     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: 
        ///                     Value 
        ///                     Meaning 
        ///                     Less than zero 
        ///                     This instance is less than <paramref name="obj" />. 
        ///                     Zero 
        ///                     This instance is equal to <paramref name="obj" />. 
        ///                     Greater than zero 
        ///                     This instance is greater than <paramref name="obj" />. 
        /// </returns>
        /// <param name="obj">
        ///                     An object to compare with this instance. 
        ///                 </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="obj" /> is not the same type as this instance. 
        ///                 </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();

            var other = (EquivalentClass)obj;
            return Representative.CompareTo(other.Representative);
        }

        #endregion

        #region Implementation of ICloneable

        /// <summary>
        ///                     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///                     A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            return new EquivalentClass(data);
        }

        #endregion
    }
}