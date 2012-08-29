using System;
using Nikos.Collections.Advance.HD_Engine;
using Nikos.Collections.Interfaces;

namespace Nikos.Collections.Advance.HD_Engine
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class B_TreeNode<T> : INode<T> where T : NodeItem, IComparable<T>, ISizable
    {
        private readonly long size;
        private int childrenCount;
        private int degree;
        private int keysCount;
        private long location;

        public B_TreeNode(int degree, long location, long size)
        {
            this.size = size;
            Location = location;
            Degree = degree;
            Keys = new T[2 * Degree - 1];
            Childrens = new long[2 * Degree];
        }

        public B_TreeNode(int degree, long size)
            : this(degree, 0, size)
        {
        }

        #region Implementation of INode<T>

        public long Location
        {
            get { return location; }
            set
            {
                if (value < 0) throw new Exception();
                location = value;
            }
        }

        public T[] Keys { get; set; }

        public long[] Childrens { get; set; }

        public bool IsLeaf
        {
            get { return ChildrensCount == 0; }
            set { throw new NotImplementedException(); }
        }

        public int Degree
        {
            get { return degree; }
            set
            {
                if (value <= 1) throw new Exception();
                degree = value;
            }
        }

        public int KeysCount
        {
            get { return keysCount; }
            set
            {
                if (keysCount < 0) throw new Exception();
                keysCount = value;
            }
        }

        public int ChildrensCount
        {
            get { return childrenCount; }
            set
            {
                if (value < 0) throw new Exception();
                childrenCount = value;
            }
        }

        #endregion

        #region Implementation of ISizable

        /// <summary>
        ///             Retorna el tamanno en byte de la estructura
        /// </summary>
        /// <returns>
        /// </returns>
        public virtual long get_Size()
        {
            //var x = typeof(T).GetConstructor(new Type[] { });
            //if (x == null)
            //    throw new Exception("Type: " + typeof(T) + " do not hava a default constructor");
            //T value = (T)Activator.CreateInstance(typeof(T));
            //return (int)value.get_Size() * (2 * Degree - 1) + 8 * Degree + 8;

            return size;
        }

        #endregion

        #region Implementation of IComparable<INode<T>>

        /// <summary>
        ///                     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///                     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
        ///                     Value 
        ///                     Meaning 
        ///                     Less than zero 
        ///                     This object is less than the <paramref name="other" /> parameter.
        ///                     Zero 
        ///                     This object is equal to <paramref name="other" />. 
        ///                     Greater than zero 
        ///                     This object is greater than <paramref name="other" />. 
        /// </returns>
        /// <param name="other">
        ///                     An object to compare with this object.
        ///                 </param>
        public int CompareTo(INode<T> other)
        {
            return location.CompareTo(other.Location);
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
            return CompareTo((INode<T>)obj);
        }

        #endregion
    }
}