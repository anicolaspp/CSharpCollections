using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikos.Collections.Bases;
using Nikos.Graphs.Concepts;

namespace Nikos.Graphs.Algoritms
{
    class EdgeResult : IEdge
    {
        private readonly IVertex source;
        private readonly IVertex target;
        private double cost;
        private bool marked;

        public EdgeResult(IVertex source, IVertex target, double cost, bool marked)
        {
            this.source = source;
            this.target = target;
            this.cost = cost;
            this.marked = marked;
        }

        #region Implementation of IComparable<IEdge>

        /// <summary>
        ///                     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///                     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
        ///                     Value 
        ///                     Meaning 
        ///                     Less than zero 
        ///                     This object is less than the <paramref name="other" /> parameter.
        ///                     Zero 
        ///                     This object is equal to <paramref name="other" />. 
        ///                     Greater than zero 
        ///                     This object is greater than <paramref name="other" />. 
        /// </returns>
        /// <param name="other">
        ///                     An object to compare with this object.
        ///                 </param>
        public int CompareTo(IEdge other)
        {
            return source == other.Source && target == other.Target && cost == other.Cost ? 0 : 1;
        }

        #endregion

        #region Implementation of IEdge

        public IVertex Source
        {
            get { return source; }
        }

        public IVertex Target
        {
            get { return target; }
        }

        public double Cost
        {
            get { return cost; }
            set { cost  = value; }
        }

        public bool IsMarked
        {
            get { return marked; }
            set { marked = value; }
        }

        #endregion
    }

    public static class Prim
    {
        /// <summary>
        /// Aplica algoritmo de Prim al grafo para hallar el MSP.
        /// El Indentificador de los nodes tienen que ser de tipo int
        /// </summary>
        /// <param name="graph">Grafo</param>
        /// <returns>El MSP</returns>
        public static IEnumerable<IEdge> Run(IGraph graph)
        {
            int[] distancia = new int[graph.VertexList.Count];
            IVertex[] phi = new IVertex[graph.VertexList.Count];

            foreach (IVertex vertex in graph.VertexList)
            {
                distancia[(int) vertex.Identifier] = int.MaxValue;
                phi[(int) vertex.Identifier] = null;
            }

            distancia[(int)graph.FirstVertex.Identifier] = 0;
            var p_Queue = BinomialHeap<int>.Build_Heap(graph.VertexList, x => distancia[(int) x.Identifier]);

            while (p_Queue.Count > 0)
            {
                var current = p_Queue.DeQueue();
                foreach (IEdge edge in current.Out)
                    if (p_Queue.Contains(edge.Target) && edge.Cost < distancia[(int) edge.Target.Identifier])
                    {
                        distancia[(int) edge.Target.Identifier] = (int) edge.Cost;
                        phi[(int) edge.Target.Identifier] = edge.Source;
                    }
            }

            for (int i = 0; i < graph.VertexList.Count; i++)
            {
                yield return new EdgeResult(graph.VertexList[i], phi[i], distancia[i], false);
            }
        }
    }
}
