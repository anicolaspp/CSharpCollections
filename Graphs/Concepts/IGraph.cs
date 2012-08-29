using System.Collections.Generic;
using Nikos.Graphs.Concepts;

namespace Nikos.Graphs.Concepts
{
    public interface IGraph
    {
        bool IsDirected { get; }
        
        IVertex FirstVertex { get; set; }
        
        List<IVertex> VertexList { get; }

        List<IEdge> EdgeList { get; }
    }
}