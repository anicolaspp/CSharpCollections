using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikos.Extensions.Collections;
using Nikos.Graphs.Concepts;

namespace Nikos.Graphs.Algoritms
{
    public static class DFS
    {
        public enum DSF_Type { Recursive, Iterative };
        
        public static IEnumerable<IVertex> Run(IGraph graph, DSF_Type dsf_Type)
        {
            return dsf_Type == DSF_Type.Recursive ? DFS_Recursive(graph.FirstVertex) : DFS_Iterative(graph.FirstVertex);
        }
       
        public static IEnumerable<IVertex> Run(IVertex vertex, DSF_Type dsf_Type)
        {
            return dsf_Type == DSF_Type.Recursive ? DFS_Recursive(vertex) : DFS_Iterative(vertex);
        }

        private static IEnumerable<IVertex> DFS_Iterative(IVertex vertex)
        {
            Stack<IVertex> m_Stack = new Stack<IVertex>();
            m_Stack.Push(vertex);

            while (m_Stack.Count > 0)
            {
                vertex = m_Stack.Pop();
                vertex.Visited = true;
                yield return vertex;

                m_Stack.AddRange(vertex.Out.Where(x => !x.Target.Visited).Select(x => x.Target));
            }
        }

        private static IEnumerable<IVertex> DFS_Recursive(IVertex vertex)
        {
            vertex.Visited = true;
            yield return vertex;

            foreach (IVertex item in vertex.Out.Where(x => !x.Target.Visited).Select(x => x.Target))
            {
                DFS_Recursive(item);
            }
        }
    }
}