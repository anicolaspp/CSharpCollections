using System.Collections.Generic;
using System.Linq;

namespace Nikos.Collections.Graphs.Algorithms
{
    public static class Kruskal
    {
        //public static IEnumerable<IEdge> Run(IGraph graph)
        //{
        //    var result = new List<IEdge>();
        //    DisJoinSet set = new DisJoinSet(graph.Vertexs.ToArray());

        //    var edges = graph.EdgeList.ToArray().QuickSort((x, y) => x.CompareTo(y));

        //    foreach (IEdge edge in edges)
        //        if (set.SetOf(edge.Source) != set.SetOf(edge.Target))
        //        {
        //            result.Add(edge);
        //            set.SetOf(edge.Source).Add(edge.Target);
        //        }

        //    return result;
        //}
    }
}