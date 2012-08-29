using System.Collections.Generic;
using System.Linq;
using Nikos.Collections;

namespace Nikos.Algorithms
{
    /// <summary>
    /// 
    /// </summary>
    public static class Huffman
    {
        private static HNode CreateHeap(IEnumerable<HNode> input)
        {
            var heap = BinomialHeap<HNode>.Build_Heap(input, x => x.Count); // O(n) amortized cost

            while (heap.Count > 1)
            {
                var x = heap.DeQueue();
                var y = heap.DeQueue();
                heap.EnQueue(new HNode { Left = x, Rigth = y, Key = ""[0], Count = x.Count + y.Count }, x.Count + y.Count);
            }

            return heap.DeQueue();
        }

        private static void CreateHash(BinaryNode<char> root, IDictionary<char, string> hash, string value)
        {
            if (root.Left == null && root.Rigth == null)
                hash.Add(root.Key, value);
            if (root.Left != null)
                CreateHash(root.Left, hash, value + 0);
            if (root.Rigth != null)
                CreateHash(root.Rigth, hash, value + 1);
        }

        public static Pair<string, IDictionary<char, string>> Encode(string input)
        {
            var hash = new System.Collections.Generic.Dictionary<char, int>();
            foreach (char item in input)
                if (hash.ContainsKey(item))
                    hash[item]++;
                else
                    hash.Add(item, 1);

            var root = CreateHeap(hash.Keys.Select(x => new HNode { Key = x, Count = hash[x] }));
            
            var cod = new System.Collections.Generic.Dictionary<char, string>();
            CreateHash(root, cod, "");

            return new Pair<string, IDictionary<char, string>> { Key = input.Select(item => cod[item]).Aggregate("", (x, y) => x + y), Value = cod };
        }

        public static string DeCode(string input, IDictionary<char, string> hash) // no esta optimizado; tengo que llevarlo a O(n); costo actual: O(n * ln(n))
        {
            int a = 0, b = 0;
            string result = "";

            var cod = new System.Collections.Generic.Dictionary<string, char>();
            foreach (var pair in hash)
                cod.Add(pair.Value, pair.Key);

            while (a < input.Length) // O(n)
            {
                var x = input.Substring(a, b - a + 1);
                if (cod.ContainsKey(x))
                {
                    result += cod[x];
                    b++; a = b;
                }
                else b++;
            }

            return result;
        }
    }
}
