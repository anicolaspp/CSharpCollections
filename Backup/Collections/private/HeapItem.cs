using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos.Collections.Bases
{
    [Serializable]
    class HeapItem<T>
    {
        public int Priority { get; set; }
        public T Value { get; set; }

        public HeapItem(T value, int priority)
        {
            Value = value;
            Priority = priority;
        }
    }
}
