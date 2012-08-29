using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikos.Collections.Graphs;

namespace Nikos.Collections.Graphs
{
    public class AdjacenyListGraph : IGraph
    {
        private List<IVertex> _vertices;
        private List<IEdge> _edges;
        private readonly bool m_IsDirected;

        public AdjacenyListGraph(IEnumerable<IVertex> vertices, bool isDirected)
        {
            if (vertices == null)
                throw new ArgumentNullException("vertices");

            _vertices = new List<IVertex>(vertices);
            m_IsDirected = isDirected;
        }

        #region Implementation of ICloneable

        /// <summary>
        ///  Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public IGraph Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region Implementation of IGraph

        public IEnumerable<IVertex> Vertexs
        {
            get { return _vertices; }
        }

        public IEnumerable<IEdge> Edges
        {
            get { return _edges; }
        }

        public IEnumerable<IVertex> Ady(IVertex v)
        {
            return null;
        }

        public IEnumerable<IEdge> Edg(IVertex v)
        {
            throw new NotImplementedException();
        }

        public int Degree(IVertex v)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGraph> ConectedComponents()
        {
            return null;
        }

        public int Edge_Count
        {
            get { throw new NotImplementedException(); }
        }

        public int Vertex_Count
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public static AdjacenyListGraph operator +(AdjacenyListGraph x, IGraph y)
        {
            var result = x.Clone() as AdjacenyListGraph;

            foreach (var vertex in y.Vertexs.Where(vertex => !x._vertices.Contains(vertex)))
                result._vertices.Add(vertex);

            foreach (var edge in y.Edges.Where(edge=>!x._edges.Contains(edge)))
                result._edges.Add(edge);

            return result;
        }
    }
}