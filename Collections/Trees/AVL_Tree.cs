using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nikos.Collections
{
    ///<summary>
    ///Represent a AVL tree structure; the operations over AVL tree are O(log n)
    ///</summary>
    ///<typeparam name="T"></typeparam>
    [Serializable]
    public class AVL_Tree<T> : ITree<T>, ICloneable<AVL_Tree<T>> where T :  IComparable<T>
    {
        #region Privates

        private AVL_Node<T> Insert(AVL_Node<T> x, T item)
        {
            if (x == null)
            {
                x = new AVL_Node<T> { Key = item, Height = 1, Size = 1, Left = null, Rigth = null };
                Count++;
            }
            else
            {
                if (item.CompareTo(x.Key) == 0) return x;

                if (item.CompareTo(x.Key) < 0)
                    x.Left = Insert(x.Left, item);
                else
                    x.Rigth = Insert(x.Rigth, item);

                x = Balance(x);
            }

            return Balance(x);
        }
        private AVL_Node<T> Balance(AVL_Node<T> x)
        {
            x.Update();

            if (x.Balance_Factor > 1)
            {
                if (x.Rigth.Balance_Factor <= 0)
                    x.Rigth = Rotate(x.Rigth, Direction.Rigth);
                x = Rotate(x, Direction.Left);
            }
            else
                if (x.Balance_Factor < -1)
                {
                    if (x.Left.Balance_Factor >= 0)
                        x.Left = Rotate(x.Left, Direction.Left);
                    x = Rotate(x, Direction.Rigth);
                }

            x.Update();
            return x;
        }
        private AVL_Node<T> Rotate(AVL_Node<T> x, Direction direction)
        {
            AVL_Node<T> y;

            if (direction == Direction.Left) // left
            {
                if (x == null || x.Rigth == null) return x;

                y = x.Rigth;
                x.Rigth = y.Left;
                y.Left = x;
            }
            else // right
            {
                if (x == null || x.Left == null) return x;

                y = x.Left;
                x.Left = y.Rigth;
                y.Rigth = x;
            }

            x.Update();
            y.Update();

            return y;
        }
        private AVL_Node<T> Remove(AVL_Node<T> x, T item)
        {
            if (x == null) return x;
            if (x.Key.CompareTo(item) == 0)
            {
                if (x.Left == null && x.Rigth == null)
                    return null;

                if (x.Left != null && x.Rigth == null)
                    return x.Left;

                if (x.Left == null && x.Rigth != null)
                    return x.Rigth;

                var t = x.Rigth;
                var p = x;
                while (t.Left != null)
                {
                    p = t;
                    t = t.Left;
                }
                if (p == x)
                    return x.Rigth;

                p.Left = t.Rigth;
                t.Rigth = x.Rigth;
                t.Left = x.Left;
                return t;
            }

            if (x.Key.CompareTo(item) > 0)
                x.Left = Remove(x.Left, item);
            else
                x.Rigth = Remove(x.Rigth, item);

            return Balance(x);
        }

        ///<summary>
        ///Create a new AVL tree
        ///</summary>
        public AVL_Tree()
        {
            C_Root = null;
        }

        ///<summary>
        ///Create a new AVL tree
        ///</summary>
        ///<param name="data">Data to add of tree</param>
        public AVL_Tree(params T[] data)
            : this()
        {
            foreach (var item in data)
                Add(item);
        }
        #endregion

        /// <summary>
        /// The root node
        /// </summary>
        public AVL_Node<T> C_Root { get; private set; }

        /// <summary>
        /// The pre-order walk on tree
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> PreOrden()
        {
            return Count > 0 ? C_Root.PreOrden() : C_Root.EmptyEnum();
        }

        #region Implementation of ICollection<T>

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="array" /> is multidimensional.-or-<paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.-or-Type <paramref name="T"></paramref>
        /// 	cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            if (array.Rank > 1)
                throw new ArgumentException();

            var source = GetEnumerator();
            for (int i = arrayIndex; i < array.Length; i++)
            {
                source.MoveNext();
                array[i] = source.Current;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(T item)
        {
            AVL_Node<T> temp;
            if (Contains(item, out temp))
            {
                C_Root = Remove(C_Root, item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(T item)
        {
            C_Root = Insert(C_Root, item);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        bool ICollection<T>.Contains(T item)
        {
            return Contains(item) != null;
        }
        ///<summary>
        ///Determine if a item is in tree
        ///</summary>
        ///<param name="item">Item to searsh</param>
        ///<param name="output">the node that contain the item if the item is on </param>
        ///<returns></returns>
        public bool Contains(T item, out AVL_Node<T> output)
        {
            output = Contains(item);
            return output == null ? false : true;
        }
        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<returns></returns>
        public T ContainsI(T item)
        {
            var temp = Contains(item);
            return temp == null ? default(T) : temp.Key;
        }
        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<returns></returns>
        public AVL_Node<T> Contains(T item)
        {
            AVL_Node<T> x = C_Root;
            for (; ; )
            {
                if (x == null) return null;
                if (item.CompareTo(x.Key) == 0) return x;

                x = item.CompareTo(x.Key) < 0 ? x.Left : x.Rigth;
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear()
        {
            C_Root = null;
            Count = 0;
        }

        #endregion

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return InOrderTreeWalk().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Consult Methods

        ///<summary>
        ///Get the items greather than value in tree
        ///</summary>
        ///<param name="value"></param>
        ///<returns></returns>
        public IEnumerable<T> GreaterThan(T value)
        {
            var x = C_Root;
            while (x != null)
            {
                if (x.Key.CompareTo(value) > 0)
                {
                    foreach (T t in  x.Rigth.InOrden())
                        yield return t;
                    x = x.Left;
                }
                else x = x.Rigth;
            }
        }

        ///<summary>
        ///Get the items less than value in tree
        ///</summary>
        ///<param name="value"></param>
        ///<returns></returns>
        public IEnumerable<T> LessThan(T value)
        {
            var x = C_Root;
            while (x != null)
            {
                if (x.Key.CompareTo(value) < 0)
                {
                    foreach (T t in x.Left.InOrden())
                        yield return t;
                    x = x.Rigth;
                }
                else x = x.Left;
            }
        }

        #endregion

        #region Implementation of ITree<T>

        /// <summary>
        /// Tree's root
        /// </summary>
        public T Root
        {
            get { return C_Root.Key; }
        }

        /// <summary>
        /// Get min value of tree
        /// </summary>
        /// <returns></returns>
        public T Min()
        {
            if (C_Root == null) throw new InvalidOperationException("The tree is empty");

            AVL_Node<T> x = C_Root;
            while (x.Left != null)
                x = x.Left;

            return x.Key;

        }

        /// <summary>
        /// Get max valur of tree
        /// </summary>
        /// <returns></returns>
        public T Max()
        {
            if (C_Root == null) throw new InvalidOperationException("The tree is empty");

            AVL_Node<T> x = C_Root;
            while (x.Rigth != null)
                x = x.Rigth;

            return x.Key;
        }

        /// <summary>
        /// Get the in order walk of tree
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> InOrderTreeWalk()
        {
            return Count > 0 ? C_Root.InOrden() : C_Root.EmptyEnum();
        }

        #endregion

        #region Implementation of ICloneable

        /// <summary>
        ///  Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public AVL_Tree<T> Clone()
        {
            return new AVL_Tree<T>(InOrderTreeWalk().ToArray());
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