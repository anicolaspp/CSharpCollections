using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikos.Graphs.Concepts;

namespace Nikos.Graphs.Concepts
{
    public interface IVertex :  IComparable
    {
        IComparable Identifier { get; }

        IList<IEdge> Out { get; }
        IList<IEdge> In { get; }

        bool Visited { get; set; }
    }
}