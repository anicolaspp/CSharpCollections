using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikos.Extensions.Collections;
using Nikos.Graphs.Concepts;

namespace Nikos.Graphs.Algoritms
{
    public static class BFS
    {
        public static IEnumerable<IVertex> Run(IGraph graph)
        {
            Queue<IVertex> m_Queue = new Queue<IVertex>();
            m_Queue.Enqueue(graph.FirstVertex);

            while (m_Queue.Count > 0)
            {
                var vertex = m_Queue.Dequeue();
                vertex.Visited = true;
                yield return vertex;
                m_Queue.AddRange(vertex.Out.Where(x => !x.Target.Visited).Select(x => x.Target));
            }
        }
    }
}