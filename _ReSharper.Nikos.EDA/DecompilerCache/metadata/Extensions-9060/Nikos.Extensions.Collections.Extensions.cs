// Type: Nikos.Extensions.Collections.Extensions
// Assembly: Extensions, Version=1.0.0.0, Culture=neutral
// Assembly location: D:\Projects\Collections.Extensions\Collections.Extensions\bin\Debug\Extensions.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Nikos.Extensions.Collections
{
    public static class Extensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> source);
        public static IEnumerable ToEnumerable(this IEnumerator source);
        public static void AddRange<T>(this Stack<T> dest, IEnumerable<T> source);
        public static void AddRange<T>(this Queue<T> dest, IEnumerable<T> source);
        public static IEnumerable<T> QuickSort<T>(this IEnumerable<T> source, Comparison<T> comparison);
        public static IEnumerable<T> QuickSort<T>(this IEnumerable<T> source) where T : IComparable;

        public static IEnumerable<T> QuickSort<T>(this IEnumerable<T> source, Comparison<T> comparison, int Inicio,
                                                  int Final);

        public static IEnumerable<T> MergeSort<T>(this IEnumerable<T> source, Comparison<T> comparison);
        public static IEnumerable<T> MergeSort<T>(this IEnumerable<T> source) where T : IComparable;

        public static IEnumerable<T> MergeSort<T>(this IEnumerable<T> source, Comparison<T> comparison, int Inicio,
                                                  int Final);

        public static bool[] Remove<T>(this ICollection<T> source, params T[] items);
        public static int[] Kmp<T>(this IEnumerable<T> source, IEnumerable<T> secuence, Comparison<T> comparison);
        public static int[] Kmp<T>(this IEnumerable<T> source, IEnumerable<T> secuence) where T : IComparable;
        public static int CompareTo<T>(this T source, IComparable obj, Func<T, IComparable> func);
        public static int CompareTo<T, K>(this T source, K obj, Func<T, IComparable> func_1, Func<K, IComparable> func_2);
        public static IEnumerable<T[]> Split<T>(this IEnumerable<T> source, params int[] indexs);
        public static string C_ToString(this IEnumerable<char> source);
        public static IEnumerable<T> SubSec<T>(this IEnumerable<T> source, int start, int end);
    }
}
