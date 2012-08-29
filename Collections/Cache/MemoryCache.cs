using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nikos.Cache;

namespace Nikos.Cache
{
    ///<summary>
    /// Memory Stystem Cache
    ///</summary>
    ///<typeparam name="T">Type of elemente on the memory</typeparam>
    public sealed class MemoryCache<T> : IDisposable where T : ISizable, IComparable
    {
        private readonly Random random;

        /// <summary>
        /// Capacity of memory
        /// </summary>
        public long Capacity { get; private set; }
        /// <summary>
        /// The real size of memory
        /// </summary>
        public float RealSize { get; private set; }

        private static object m_Cache = null;
        private readonly List<CacheItem<T>> data;//esto se puede mejorar si usamos un Hashtable o un HasSet.

        private static float FromByteToKiloByte(long numberOfBytes)
        {
            return numberOfBytes / 1024f;
        }
        private CacheItem<T> Get_By_Owner(object owner)
        {
            return data.FirstOrDefault(cacheItem => cacheItem.Equals(owner));
        }

        private MemoryCache(long capacity)
        {
            Capacity = capacity;
            RealSize = 0;
            data = new List<CacheItem<T>>();
            random = new Random(Environment.TickCount);
        }

        /// <summary>
        /// Get acces to the memory cache
        /// </summary>
        /// <param name="capacity">Capacity (in KBs) of memory, if the memory was created this parameter is ignored</param>
        /// <returns></returns>
        public static MemoryCache<T> Get_Intance(long capacity)
        {
            if (m_Cache == null)
                m_Cache = new MemoryCache<T>(capacity);

            return (MemoryCache<T>)m_Cache;
        }
        /// <summary>
        /// Add element to cache
        /// </summary>
        /// <param name="value">Value to adapted</param>
        /// <summary>
        /// Add element to cache
        /// </summary>
        /// <param name="owner">The owner of value</param>
        /// <param name="value">Value to adapted</param>
        public void Add(object owner, T value)
        {
            Add(owner, value, null);
        }
        /// <summary>
        /// Add element to cache
        /// </summary>
        /// <param name="owner">The owner of value</param>
        /// <param name="value">Value to adapted</param>
        /// <param name="method">The method to execute when remove the item from cache</param>
        public void Add(object owner, T value, MethodInvoke method)
        {
            var item = Get_By_Owner(owner);

            if (item != null)
            {
                int index = item.Data.BinarySearch(new Executer<T> { Value = value });
                // si esta lo sustituyo por el nuevo.
                if (index >= 0)
                {
                    RealSize -= FromByteToKiloByte(value.get_Size());
                    item.Data[index] = new Executer<T> { Value = value, m_Info = method };
                    RealSize += FromByteToKiloByte(value.get_Size());
                    return;
                }
                // si no esta y cabe en la memoria
                if (RealSize + FromByteToKiloByte(value.get_Size()) <= Capacity)
                {
                    item.Data.Add(new Executer<T> { Value = value, m_Info = method });
                    RealSize += FromByteToKiloByte(value.get_Size());
                    return;
                }
                //no cabe, pero pudiera caber...
                if (item.get_Size() >= value.get_Size())
                {
                    while (RealSize + FromByteToKiloByte(value.get_Size()) > Capacity)
                    {
                        //eliminacion aleatoria de los elementos del cache
                        int pos = random.Next(item.Data.Count);
                        var x = item.Data[pos];
                        //ejecutando la accion asociada
                        x.Execute();
                        item.Data.RemoveAt(pos);
                        RealSize -= FromByteToKiloByte(x.Value.get_Size());
                    }
                    item.Data.Add(new Executer<T> { Value = value, m_Info = method });
                    RealSize += FromByteToKiloByte(value.get_Size());
                    return;
                }
            }
            else
            {
                if (RealSize + FromByteToKiloByte(value.get_Size()) <= Capacity)
                {
                    data.Add(new CacheItem<T> { Owner = owner, Data = new List<Executer<T>> { new Executer<T> { Value = value, m_Info = method } } });
                    RealSize += FromByteToKiloByte(value.get_Size());
                    return;
                }
            }
            //si no lo puedo meter en el cache ejecutar la accion asociada
            new Executer<T> { Value = value, m_Info = method }.Execute();
        }

        /// <summary>
        /// Remove a value of cache
        /// Execute the asociated method with value before removed
        /// </summary>
        /// <param name="owner">Owner of value</param>
        /// <param name="value">Value to remove</param>
        public bool Remove(object owner, T value)
        {
            var item = Get_By_Owner(owner);
            if (item != null)
            {
                int index = item.Data.BinarySearch(new Executer<T> { Value = value });
                if (index < 0)
                    return false;

                item.Data[index].Execute();
                RealSize -= FromByteToKiloByte(item.Data[index].Value.get_Size());
                item.Data.RemoveAt(index);
                return true;
            }
            return false;
        }

        ///<summary>
        /// Determine if a element is on cache
        ///</summary>
        ///<param name="owner"></param>
        ///<param name="value"></param>
        ///<returns></returns>
        public bool Contains(object owner, T value)
        {
            var item = Get_By_Owner(owner);
            if (item == null)
                return false;

            return item.Data.BinarySearch(new Executer<T> { Value = value }) >= 0 ? true : false;
        }

        /// <summary>
        /// Find a element on cache with specific function
        /// </summary>
        /// <param name="owner">Owner of element</param>
        /// <param name="func">Convert the value to IComparable for search on cache</param>
        /// <param name="value">Value to find</param>
        /// <returns></returns>
        public T Find(object owner, Func<T, IComparable> func, IComparable value)
        {
            var item = Get_By_Owner(owner);
            if (item != null)
                for (int i = 0; i < item.Data.Count; i++)
                    if (func(item.Data[i].Value).CompareTo(value) == 0)
                    {
                        var result = item.Data[i];
                        return result.Value;
                    }
            return default(T);
        }

        ///<summary>
        /// Find all elements on cache with specific function
        ///</summary>
        ///<param name="owner">Owner of element</param>
        ///<param name="func">Convert the value to IComparable for search on cache</param>
        ///<param name="value">Value to find</param>
        ///<returns></returns>
        public IEnumerable<T> FindAll(object owner, Func<T, IComparable> func, IComparable value)
        {
            var item = Get_By_Owner(owner);
            return from t in item.Data where func(t.Value).CompareTo(value) == 0 select t.Value;
        }

        /// <summary>
        /// Clear memory asociated to especific owner
        /// </summary>
        /// <param name="owner">The owner of element to clear</param>
        /// <param name="performanceOperationPendding"></param>
        public void Clear(object owner, bool performanceOperationPendding = true)
        {        
            var own = Get_By_Owner(owner);
            if (own != null)
            {
                if (performanceOperationPendding)
                    foreach (var executer in own.Data)
                        executer.Execute();
                own.Data.Clear();
            }
        }

        /// <summary>
        /// Performance the action asociate to the item of cache for all item of owner
        /// </summary>
        /// <param name="owner">Owner of items</param>
        public void UpDate(object owner)
        {
            var own = Get_By_Owner(owner);
            if (own != null)
                foreach (var executer in own.Data)
                    executer.Execute();
        }

        /// <summary>
        /// Performance the action asociate to all items on cache
        /// </summary>
        public void UpDateAll()
        {
            foreach (var cacheItem in data)
                foreach (var executer in cacheItem.Data)
                    executer.Execute();
        }

        #region Implementation of IDisposable

        /// <summary>
        ///                     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///                     Clear all element for each owner
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var executer in data.SelectMany(item => item.Data))
                executer.Execute();
            data.Clear();
        }

        #endregion
    }
}