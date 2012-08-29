using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikos.Graphs.Concepts;

namespace Nikos.Graphs.Representation
{
    public class AdjacenyListGraph : IGraph
    {
        private List<IVertex> m_Vertices;
        private readonly bool m_IsDirected;

        public AdjacenyListGraph(IEnumerable<IVertex> vertices, bool isDirected)
        {
            if (vertices == null)
                throw new ArgumentNullException("vertices");

            m_Vertices = new List<IVertex>(vertices);
            m_IsDirected = isDirected;
        }

        #region Implementation of IGraph

        public bool IsDirected
        {
            get { return m_IsDirected; }
        }

        public IVertex FirstVertex
        {
            get { return VertexList.Count > 0 ? VertexList[0] : null; }
            set { VertexList[0] = value; }
        }

        public List<IVertex> VertexList
        {
            get { return m_Vertices; }
        }

        public List<IEdge> EdgeList
        {
            get 
            {
                List<IEdge> result = new List<IEdge>();

                foreach (IVertex vertex in m_Vertices)
                    result.AddRange(vertex.Out);

                return result;
            }
        }

        #endregion
    }
}