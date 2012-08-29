using System;
using System.Collections.Generic;

namespace Nikos.Cache
{
    class CacheItem<T> : ISizable where T : ISizable, IComparable
    {
        public object Owner { get; set; }
        public List<Executer<T>> Data { get; set; }

        ///<summary>
        ///
        ///                    Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        ///                
        ///</summary>
        ///
        ///<returns>
        ///true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.
        ///                
        ///</returns>
        ///
        ///<param name="obj">
        ///                    The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />. 
        ///                </param>
        ///<exception cref="T:System.NullReferenceException">
        ///                    The <paramref name="obj" /> parameter is null.
        ///                </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return Owner.Equals(obj);
        }

        /// <summary>
        ///             Retorna el tamanno en byte de la estructura
        /// </summary>
        /// <returns>
        /// </returns>
        public long get_Size()
        {
            long result = 0;
            foreach (var executor in Data)
                result += executor.Value.get_Size();
            return result;
        }
    }
}