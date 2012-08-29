using System;
using System.Collections.Generic;

namespace Nikos.Collections.Bases
{
    class ABBNode<T> where T : IComparable<T>
    {
        public T Value { get; set; }
        public ABBNode<T> Left { get; set; }
        public ABBNode<T> Rigth { get; set; }

        public void Add(T value)
        {
            if (Value.CompareTo(value) != 0)
                if (value.CompareTo(Value) < 0)
                    if (Left != null)
                        Left.Add(value);
                    else
                        Left = new ABBNode<T> { Value = value, Left = null, Rigth = null };
                else
                    if (Rigth != null)
                        Rigth.Add(value);
                    else
                        Rigth = new ABBNode<T> { Value = value, Left = null, Rigth = null };
        }

        public IEnumerable<T> PreOrden()
        {
            yield return Value;
            
            if (Left != null)
                Left.PreOrden();

            if (Rigth != null)
                Rigth.PreOrden();
        }
        public IEnumerable<T> EntreOrden()
        {
            if (Left != null)
                Left.PreOrden();

            yield return Value;
            
            if (Rigth != null)
                Rigth.PreOrden();
        }
        public IEnumerable<T> PosOrden()
        {
            if (Left != null)
                Left.PreOrden();

            if (Rigth != null)
                Rigth.PreOrden();

            yield return Value;
        }
    }
}