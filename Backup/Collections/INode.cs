using System;
using Nikos.Collections.Advance.HD_Engine;
using Nikos.Collections.Interfaces;

namespace Nikos.Collections.Advance.HD_Engine
{
    public interface INode<T> : ISizable, IComparable<INode<T>>, IComparable where T : NodeItem, ISizable, IComparable<T>
    {
        long Location { get; set; }
        T[] Keys { get; set; }
        long[] Childrens { get; set; }
        bool IsLeaf { get; set; }
        int Degree { get; }
        int KeysCount { get; set; }
        int ChildrensCount { get; set; }
    }
}