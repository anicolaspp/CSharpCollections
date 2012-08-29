using System.Collections.Generic;

namespace Nikos.Collections.Graphs.Algorithms
{
    public static class Topological_Sort
    {
        static List<IVertex> visited = new List<IVertex>();
        static List<IVertex> result = new List<IVertex>(); 

        public static IEnumerable<IVertex> Run(IGraph graph)
        {
            foreach (IVertex vertex in graph.Vertexs)
            {
                if (!visited.Contains(vertex))
                    dfs(vertex, graph);
            }

            return result;
        }

        private static void dfs(IVertex vertex, IGraph graph)
        {
            visited.Add(vertex);
            foreach (var item in graph.Ady(vertex))
            {
                if (!visited.Contains(item))
                    dfs(item, graph);
            }
            result.Insert(0, vertex);
        }
    }
}