using System;

namespace Nikos.Collections.Graphs
{
    public interface IVertex : IComparable<IVertex>
    {
        string Tag { get; set; }
        int Degree { get; }
        int Identifier { get; set; }
    }
}