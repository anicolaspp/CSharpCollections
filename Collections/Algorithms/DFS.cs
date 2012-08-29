using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikos.Extensions.Collections;

namespace Nikos.Collections.Graphs.Algorithms
{
    public static class DFS
    {
        public enum DSF_Type { Recursive, Iterative };

        public static IEnumerable<IVertex> Run(IGraph graph, DSF_Type dsf_Type)
        {
            return dsf_Type == DSF_Type.Recursive ? DfsRecursive(graph.Vertexs.First(), graph) : DfsIterative(graph.Vertexs.First(), graph);
        }

        private static IEnumerable<IVertex> Run(IVertex vertex, IGraph graph, DSF_Type dsfType)
        {
            return dsfType == DSF_Type.Recursive ? DfsRecursive(vertex, graph) : DfsIterative(vertex, graph);
        }

        private static IEnumerable<IVertex> DfsIterative(IVertex vertex, IGraph graph)
        {
            var marked = new List<IVertex>(new[] { vertex });

            Stack<IVertex> m_Stack = new Stack<IVertex>();

            m_Stack.Push(vertex);

            while (m_Stack.Count > 0)
            {
                vertex = m_Stack.Pop();
                marked.Add(vertex);
                yield return vertex;

                foreach(var item in graph.Ady(vertex).Where(x=>!marked.Contains(x)))
                {
                    marked.Add(item);
                    m_Stack.Push(item);
                }
            }
        }

        private static IEnumerable<IVertex> DfsRecursive(IVertex vertex, IGraph graph)
        {
            var marked = new List<IVertex>(new[] { vertex });
            yield return vertex;

            foreach (var item in graph.Ady(vertex).Where(x => !marked.Contains(x)))
            {
                var query = DfsRecursive(item, graph);
                foreach (var x in query)
                    yield return x;
            }
        }
    }
}