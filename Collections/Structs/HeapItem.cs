using System;

namespace Nikos.Collections
{
    [Serializable]
    class HeapItem<T> where T: IComparable<T>
    {
        public int Priority { get; set; }
        public T Value { get; set; }

        public HeapItem(T value, int priority)
        {
            Value = value;
            Priority = priority;
        }

        public HeapItem()
        {
        }
    }
}
