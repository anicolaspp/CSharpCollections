using System;

namespace Nikos.Graphs.Concepts
{
    public interface IEdge : IComparable<IEdge>
    {
        IVertex Source { get; }
        IVertex Target { get; }

        double Cost { get; set; }
        bool IsMarked { get; set; }
    }
}