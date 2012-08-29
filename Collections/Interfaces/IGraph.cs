using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos.Collections.Graphs
{
    public interface IGraph :  ICloneable<IGraph>
    {
        IEnumerable<IVertex> Vertexs { get; }
        IEnumerable<IEdge> Edges { get; }
        IEnumerable<IVertex> Ady(IVertex v);
        IEnumerable<IEdge> Edg(IVertex v);
        int Degree(IVertex v);
        IEnumerable<IGraph> ConectedComponents();

        int Edge_Count { get; }
        int Vertex_Count { get; }
    }

}
