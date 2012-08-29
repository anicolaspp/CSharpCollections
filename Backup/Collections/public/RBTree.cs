using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos.Collections.Advance
{
    public class RBTree<T> where T : IComparable
    {
        private Node<T> current;
        private Node<T> parent;
        private Node<T> grandParent;
        private Node<T> greatParent;
        private Node<T> header;
        private Node<T> nullNode;

        public RBTree(T element)
        {
            current = new Node<T>();
            parent = new Node<T>();
            grandParent = new Node<T>();
            greatParent = new Node<T>();
            nullNode = new Node<T>();
            nullNode.Left = nullNode;
            nullNode.Right = nullNode;
            header = new Node<T>(element) { Left = nullNode, Right = nullNode };
        }

        public void Insert(T item)
        {
            grandParent = header;
            parent = grandParent;
            current = parent;
            nullNode.Element = item;
            while (current.Element.CompareTo(item) != 0)
            {
                Node<T> greatParent = grandParent;
                grandParent = parent;
                parent = current;
                current = item.CompareTo(current.Element) < 0 ? current.Left : current.Right;

                if (current.Left.Color == Color.Red && current.Right.Color == Color.Red)
                    HandleReorient(item);
            }

            if (!(current == nullNode))
                return;
            current = new Node<T>(item, nullNode, nullNode);

            if (item.CompareTo(parent.Element) < 0)
                parent.Left = current;
            else
                parent.Right = current;
            HandleReorient(item);
        }

        public T FindMin()
        {
            if (this.IsEmpty)
                throw new InvalidOperationException();

            var itrNode = header.Right;
            while (!(itrNode.Left == nullNode)) itrNode = itrNode.Left;
            return itrNode.Element;
        }

        public T FindMax()
        {
            if (this.IsEmpty)
                throw new InvalidOperationException();

            var itrNode = header.Right;
            while (!(itrNode.Right == nullNode)) itrNode = itrNode.Right;
            return itrNode.Element;
        }

        public bool IsEmpty
        {
            get { return header.Right == nullNode; }
        }

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

        private Node<T> Rotate(T item, Node<T> parent)
        {
            if (item.CompareTo(parent.Element) < 0)
                return
                    parent.Left =
                    item.CompareTo(parent.Left.Element) < 0
                        ? RotateWithLeftChild(parent.Left)
                        : RotateWithRightChild(parent.Left);
            else
                return
                    parent.Right =
                    item.CompareTo(parent.Right.Element) < 0
                        ? RotateWithLeftChild(parent.Right)
                        : RotateWithRightChild(parent.Right);
        }

        private Node<T> RotateWithRightChild(Node<T> node)
        {
            throw new NotImplementedException();
        }

        private Node<T> RotateWithLeftChild(Node<T> node)
        {
            var k1 = node.Left;
            node.Left = k1.Right;
            k1.Right = node;
            return k1;
        }
    }
}
