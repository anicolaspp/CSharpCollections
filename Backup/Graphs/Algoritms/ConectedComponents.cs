using System.Collections.Generic;
using Nikos.Graphs.Concepts;
using Nikos.Graphs.Representation;

namespace Nikos.Graphs.Algoritms
{
    public static class ConectedComponents
    {
        public static IEnumerable<IGraph> Run(IGraph graph)
        {
            List<IGraph> result = new List<IGraph>();
            
            foreach (IVertex vertex in graph.VertexList)
                if (!vertex.Visited)
                    result.Add(new AdjacenyListGraph(DFS.Run(vertex, DFS.DSF_Type.Recursive), graph.IsDirected));
            
            return result;
        }
    }
}