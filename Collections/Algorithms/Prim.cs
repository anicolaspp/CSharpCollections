using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos.Collections.Graphs.Algorithms
{
    public static class Prim
    {
         //<summary>
         //Aplica algoritmo de Prim al grafo para hallar el MST.
         //El Indentificador de los nodes tienen que ser de tipo int
         //</summary>
         //<param name="graph">Grafo</param>
         //<returns>El MSP</returns>
        public static IEnumerable Run(IGraph graph)
        {
            int[] distancia = new int[graph.Vertex_Count];
            IVertex[] phi = new IVertex[graph.Vertex_Count];

            foreach (IVertex vertex in graph.Vertexs)
            {
                distancia[vertex.Identifier] = int.MaxValue;
                phi[vertex.Identifier] = null;
            }

            distancia[graph.Vertexs.First().Identifier] = 0;
            var pQueue = BinomialHeap<IVertex>.Build_Heap(graph.Vertexs, x => distancia[x.Identifier]);

            while (pQueue.Count > 0)
            {
                var current = pQueue.DeQueue();
                foreach (IEdge edge in graph.Edg(current))
                    if (pQueue.Contains(edge.V_2) && edge.Cost < distancia[edge.V_2.Identifier])
                    {
                        distancia[edge.V_2.Identifier] = (int) edge.Cost;
                        phi[edge.V_2.Identifier] = edge.V_1;
                    }
            }


            var q = graph.Vertexs.ToArray();
            for (int i = 0; i < q.Length; i++)
            {
                yield return new { V_1 = q[i], V_2 = phi[i], Cost = distancia[i] };
            }
        }
    }
}
