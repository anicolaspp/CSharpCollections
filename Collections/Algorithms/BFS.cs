using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikos.Extensions.Collections;

namespace Nikos.Collections.Graphs.Algorithms
{
    public static class BFS
    {
        public static IEnumerable<IVertex> Run(IGraph graph)
        {
            var marked = new List<IVertex>();

            Queue<IVertex> m_Queue = new Queue<IVertex>();
            m_Queue.EnQueue(graph.Vertexs.First());

            while (m_Queue.Count > 0)
            {
                var vertex = m_Queue.DeQueue();
                marked.Add(vertex);
                yield return vertex;

                foreach (var item in graph.Ady(vertex).Where(x => !marked.Contains(x)))
                {
                    marked.Add(item);
                    m_Queue.EnQueue(item);
                }
            }
        }
    }
}