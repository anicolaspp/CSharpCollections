using System;
using System.Runtime.InteropServices;

namespace Nikos
{
    ///<summary>
    /// Supports cloning with generic type
    ///</summary>
    ///<typeparam name="T"></typeparam>
    [ComVisible(true)]
    public interface ICloneable<T> : ICloneable
    {
        /// <summary>
        ///  Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        new T Clone();
    }
}