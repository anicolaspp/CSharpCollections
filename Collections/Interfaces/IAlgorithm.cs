using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos.Collections.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAlgorithm
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        object Run(params object[] args);
    }
}
