using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos.Collections
{
    public interface ITree<T> : ICollection<T> where T: IComparable<T>
    {
        /// <summary>
        /// Tree's root
        /// </summary>
        T Root { get; }
        /// <summary>
        /// Get min value of tree
        /// </summary>
        /// <returns></returns>
        T Min();
        /// <summary>
        /// Get max valur of tree
        /// </summary>
        /// <returns></returns>
        T Max();
        /// <summary>
        /// Get the in order walk of tree
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> InOrderTreeWalk();
    }
}
