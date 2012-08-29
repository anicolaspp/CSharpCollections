using System;
using System.Collections.Generic;

namespace Nikos.Collections.Graphs
{
    public class MatrixGraph : IGraph
    {

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
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IEdge> Edges
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IVertex> Ady(IVertex v)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
    }
}