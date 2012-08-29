using System;
using System.Collections;
using System.Collections.Generic;
using Nikos.Extensions.Collections;

namespace Nikos.Collections.Bases
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public class ABB<T>: IEnumerable<T> where T: IComparable<T>
    {
        private readonly ABBNode<T> root;
        ///<summary>
        /// 
        ///</summary>
        public T Root { get { return root.Value; } }

        ///<summary>
        ///</summary>
        ///<param name="Value"></param>
        public ABB(T Value)
        {
            root = new ABBNode<T> { Value = Value, Left = null, Rigth = null };
        }

        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        public virtual void Add(T value)
        {
            root.Add(value);
        }
        ///<summary>
        ///</summary>
        ///<param name="collection"></param>
        public virtual void Add(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                Add(item);
        }

        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        public virtual void Remove(T value)
        {
        }
        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        ///<returns></returns>
        public bool Constain(T value)
        {
            return true;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public IEnumerable<T> PreOrden()
        {
            return GetEnumerator().ToEnumerable();
        }
        ///<summary>
        ///</summary>
        ///<returns></returns>
        public IEnumerable<T> EntreOrden()
        {
            return root.EntreOrden();
        }
        ///<summary>
        ///</summary>
        ///<returns></returns>
        public IEnumerable<T> PosOrden()
        {
            return root.PosOrden();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in root.PreOrden())
            {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
