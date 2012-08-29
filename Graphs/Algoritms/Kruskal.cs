using System.Collections.Generic;

using Nikos.Extensions.Collections;
using Nikos.Graphs.Concepts;
using Nikos.Collections.Bases;

namespace Nikos.Graphs.Algoritms
{
    public static class Kruskal
    {
        public static IEnumerable<IEdge> Run(IGraph graph)
        {
            var result = new List<IEdge>();
            DisJoinSet set = new DisJoinSet(graph.VertexList.ToArray());

            var edges = graph.EdgeList.ToArray().QuickSort((x, y) => x.CompareTo(y));

            foreach (IEdge edge in edges)
                if (set.SetOf(edge.Source) != set.SetOf(edge.Target))
                {
                    result.Add(edge);
                    set.SetOf(edge.Source).Add(edge.Target);
                }

            return result;
        }
    }

    public static class Topological_Sort
    {
        public static IEnumerable<IVertex> Run(IGraph graph)
        {
            return null;
        }
    }
}