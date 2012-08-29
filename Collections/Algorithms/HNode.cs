using System;
using Nikos.Collections;

namespace Nikos.Algorithms
{
    class HNode : BinaryNode<char>, IComparable<HNode>
    {
        public int Count { get; set; }

        public int CompareTo(HNode other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}