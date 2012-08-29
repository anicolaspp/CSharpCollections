using System;
using System.Collections.Generic;

namespace Nikos.Collections
{
    public class BinaryNode<T> where T: IComparable
    {
        public T Key { get; set; }
        public BinaryNode<T> Left { get; set; }
        public BinaryNode<T> Rigth { get; set; }

        public IEnumerable<T> InOrden()
        {
            if (Left != null)
            {
                foreach (var item in Left.InOrden())
                    yield return item;
            }

            yield return Key;

            if (Rigth != null)
            {
                foreach (var item in Rigth.InOrden())
                    yield return item;
            }
        }
        public IEnumerable<T> PreOrden()
        {
            yield return Key;
            if (Left != null)
            {
                var l = Left.PreOrden();
                foreach (var item in l)
                    yield return item;
            }
            if (Rigth != null)
            {
                var r = Rigth.PreOrden();
                foreach (var item in r)
                    yield return item;
            }
        }
        public IEnumerable<T> EmptyEnum()
        {
            return new T[0];
        }
    }
}
