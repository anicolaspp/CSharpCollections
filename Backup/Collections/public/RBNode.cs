using System;

namespace Nikos.Collections.Advance
{
    public class Node<T> where T: IComparable
    {
        public T Element { get; set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }
        public Color Color { get; set; }
        
        private const int RED = 0;
        private const int BLACK = 1;
        
        public Node(T element, Node<T> left, Node<T> right)
        {
            this.Element = element;
            this.Left = left;
            this.Right = right;
            this.Color = Color.Black;
        }
        public Node(T element)
        {
            this.Element = element;
            this.Left = null;
            this.Right = null;
            this.Color = Color.Black; 
        }
        public Node()
            : this(default(T))
        { }
    }
}