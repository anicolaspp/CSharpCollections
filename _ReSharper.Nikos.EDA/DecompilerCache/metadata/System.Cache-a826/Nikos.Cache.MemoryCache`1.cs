// Type: Nikos.Cache.MemoryCache`1
// Assembly: System.Cache, Version=1.0.0.0, Culture=neutral
// Assembly location: D:\Projects\System.Cache\System.Cache\bin\Debug\System.Cache.dll

using Nikos.Collections.Interfaces;
using System;
using System.Collections.Generic;

namespace Nikos.Cache
{
    public sealed class MemoryCache<T> : IDisposable where T : ISizable, IComparable
    {
        public long Capacity { get; }
        public float RealSize { get; }

        #region IDisposable Members

        public void Dispose();

        #endregion

        public static MemoryCache<T> Get_Intance(long capacity);
        public void Add(object owner, T value);
        public void Add(object owner, T value, MethodInvoke method);
        public bool Remove(object owner, T value);
        public bool Contains(object owner, T value);
        public T Find(object owner, Func<T, IComparable> func, IComparable value);
        public IEnumerable<T> FindAll(object owner, Func<T, IComparable> func, IComparable value);
        public void Clear(object owner, bool performanceOperationPendding = true);
        public void UpDate(object owner);
        public void UpDateAll();
    }
}
