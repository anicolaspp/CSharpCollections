using System;

namespace Nikos
{
    /// <summary>
    /// A Interface to determine the size of structure
    /// </summary>
    public interface ISizable
    {
        /// <summary>
        /// Retorna el tamanno en byte de la estructura
        /// </summary>
        /// <returns></returns>
        long get_Size();
    }
}