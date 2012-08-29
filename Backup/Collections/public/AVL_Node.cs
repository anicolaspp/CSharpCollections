using System;
using System.Collections.Generic;

namespace Nikos.Collections.Advance
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    [Serializable]
    public class AVL_Node<T> where T : IComparable
    {
        public T Key { get; set; }
        public AVL_Node<T> Left { get; set; }
        public AVL_Node<T> Rigth { get; set; }

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

        public int Size { get; set; }
        public int Height { get; set; }
        public int Balance_Factor 
        { 
            get 
            {
                if (Left == null && Rigth == null)
                    return 0;
                if (Left == null)
                    return Rigth.Height;
                if (Rigth == null)
                    return -Left.Height;
                return Rigth.Height - Left.Height; 
            } 
        }
        public void Update()
        {
            if (Left == null && Rigth == null)
            {
                Height = 1;
                Size = 1;
            }
            else if (Left == null)
            {
                Height = 1 + Rigth.Height;
                Size = 1 + Rigth.Size;
            }
            else if (Rigth == null)
            {
                Height = 1 + Left.Height;
                Size = 1 + Left.Size;
            }
            else
            {
                Height = 1 + Math.Max(Left.Height, Rigth.Height);
                Size = Left.Size + Rigth.Size + 1;
            }
        }
    }
}