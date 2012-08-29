using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nikos.Collections;

namespace Nikos.Collections
{
    ///<summary>
    ///Pattern of heap standart functions
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public interface IPriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        /// <summary>
        /// Count of items into the heap
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Capacity of heap
        /// </summary>
        int Capacity { get; }
        /// <summary>
        /// Change the value of priority to a especific item and reorganize the heap
        /// </summary>
        /// <param name="item">Item to change the priority</param>
        /// <param name="priority">New priority to item</param>
        void DecreaseKey(T item, int priority);
        /// <summary>
        /// Enqueue item into heap with a priority
        /// </summary>
        /// <param name="item">item to enqueue</param>
        /// <param name="priority">priority of heap</param>
        void EnQueue(T item, int priority);
        /// <summary>
        /// Get the item in the top of heap without remove it
        /// </summary>
        /// <returns></returns>
        T Peek();
        /// <summary>
        /// Get the item in the top of heap and remove it
        /// </summary>
        /// <returns></returns>
        T DeQueue();
        /// <summary>
        /// Get the type of heap based in priority
        /// </summary>
        HeapTypePriority HeapType { get; }
        /// <summary>
        /// Get if heap is empty
        /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// Determine if the heap constain a especific item with a especific priority
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        bool Contains(T item, int priority);
        bool Contains(T item);
    }
}
