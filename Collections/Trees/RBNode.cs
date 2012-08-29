using System;

namespace Nikos.Collections
{
    [Serializable]
    public class RBNode<T> where T: IComparable
    {
        public T Element { get; set; }
        public RBNode<T> Left { get; set; }
        public RBNode<T> Right { get; set; }
        public Color Color { get; set; }

        public RBNode(T element, RBNode<T> left, RBNode<T> right)
        {
            Element = element;
            Left = left;
            Right = right;
            Color = Color.Black;
        }
        public RBNode(T element)
        {
            Element = element;
            Left = null;
            Right = null;
            Color = Color.Black; 
        }
        public RBNode()
            : this(default(T))
        { }
    }
}