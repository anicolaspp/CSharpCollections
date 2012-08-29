using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nikos.Collections
{
    ///<summary>
    ///
    ///</summary>
    ///<typeparam name="T"></typeparam>
    [Serializable]
    public class RBTree<T> : ICollection<T>, ICloneable<RBTree<T>> where T : IComparable
    {
        private RBNode<T> current;
        private RBNode<T> parent;
        private RBNode<T> grandParent;
        private RBNode<T> greatParent;
        private RBNode<T> header;
        private RBNode<T> nullNode;

        private void HandleReorient(T item)
        {
            current.Color = Color.Red;
            current.Left.Color = Color.Black;
            current.Right.Color = Color.Black;
            if (parent.Color == Color.Red)
            {
                grandParent.Color = Color.Red;

                int x_1 = item.CompareTo(grandParent.Element);
                int x_2 = item.CompareTo(parent.Element);

                if (x_1 < 0 && x_1 != x_2)
                {
                    current = Rotate(item, grandParent);
                    current.Color = Color.Black;
                }
                header.Right.Color = Color.Black;
            }
        }
        private RBNode<T> Rotate(T item, RBNode<T> Parent)
        {
            if (item.CompareTo(Parent.Element) < 0)
                return
                    Parent.Left =
                    item.CompareTo(Parent.Left.Element) < 0
                        ? RotateWithLeftChild(Parent.Left)
                        : RotateWithRightChild(Parent.Left);
            return
                Parent.Right =
                item.CompareTo(Parent.Right.Element) < 0
                    ? RotateWithLeftChild(Parent.Right)
                    : RotateWithRightChild(Parent.Right);
        }

        private RBNode<T> RotateWithRightChild(RBNode<T> k1)
        {
            RBNode<T> k2 = k1.Right;
            k1.Right = k2.Left;
            k2.Left = k1;
            return k2;
        }
        private RBNode<T> RotateWithLeftChild(RBNode<T> node)
        {
            var k1 = node.Left;
            node.Left = k1.Right;
            k1.Right = node;
            return k1;
        }


        ///<summary>
        ///</summary>
        ///<param name="element"></param>
        public RBTree(T element)
        {
            current = new RBNode<T>();
            parent = new RBNode<T>();
            grandParent = new RBNode<T>();
            greatParent = new RBNode<T>();
            nullNode = new RBNode<T>();
            nullNode.Left = nullNode;
            nullNode.Right = nullNode;
            header = new RBNode<T>(element) { Left = nullNode, Right = nullNode };
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        ///<exception cref="InvalidOperationException"></exception>
        public T FindMin()
        {
            if (IsEmpty)
                throw new InvalidOperationException();

            var itrNode = header.Right;
            while (itrNode.Left != nullNode) itrNode = itrNode.Left;
            return itrNode.Element;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        ///<exception cref="InvalidOperationException"></exception>
        public T FindMax()
        {
            if (IsEmpty)
                throw new InvalidOperationException();

            var itrNode = header.Right;
            while (itrNode.Right != nullNode) itrNode = itrNode.Right;
            return itrNode.Element;
        }

        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<returns></returns>
        public RBNode<T> Find(T item)
        {
            var node = header.Right;

            while (true)
            {
                if (node.Element.CompareTo(item) == 0)
                    return node;
                if (node.Element.CompareTo(item) > 0)
                    if (node.Right == nullNode)
                        return null;
                    else
                        node = node.Right;
                else if (node.Element.CompareTo(item) < 0)
                    if (node.Left == nullNode)
                        return null;
                    else
                        node = node.Left;
            }
        }

        ///<summary>
        ///</summary>
        public bool IsEmpty
        {
            get { return header.Right == nullNode; }
        }


        #region Implementation of IEnumerable

        private IEnumerable<ItemColor<T>> InOrden(RBNode<T> Current)
        {
            if (Current.Left!= null)
                InOrden(Current.Left);
            
            yield return new ItemColor<T> { Element = Current.Element, Color = Current.Color };
            
            if (Current.Right != null)
                InOrden(Current.Right);
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public IEnumerable<ItemColor<T>> InOrden()
        {
            return InOrden(header.Right);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return InOrden().Select(x => x.Element).GetEnumerator();
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

        #region Implementation of ICollection<T>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(T item)
        {
            grandParent = header;
            parent = grandParent;
            current = parent;
            nullNode.Element = item;
            while (current.Element.CompareTo(item) != 0)
            {
                RBNode<T> greatParent = grandParent;
                grandParent = parent;
                parent = current;
                current = item.CompareTo(current.Element) < 0 ? current.Left : current.Right;

                if (current.Left.Color == Color.Red && current.Right.Color == Color.Red)
                    HandleReorient(item);
            }

            if (current != nullNode)
                return;
            current = new RBNode<T>(item, nullNode, nullNode);

            if (item.CompareTo(parent.Element) < 0)
                parent.Left = current;
            else
                parent.Right = current;
            HandleReorient(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            header.Right = nullNode;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(T item)
        {
            return Find(item) != null;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
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
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Implementation of ICloneable

        /// <summary>
        ///  Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public RBTree<T> Clone()
        {
            var result = new RBTree<T>(header.Right.Element);

            foreach (T item in this)
                result.Add(item);

            return result;
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

    /// <summary>
    /// Represent the color of a item 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ItemColor<T>
    {
        ///<summary>
        ///</summary>
        public T Element { get; set; }
        ///<summary>
        ///</summary>
        public Color Color { get; set; }
    }
}
