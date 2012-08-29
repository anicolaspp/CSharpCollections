using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Nikos.Extensions.Collections;

namespace Nikos.Collections
{
    public class Trie : ITree<string>, ICloneable<Trie>, IPrint
    {
        private TrieNodeList m_header;
        ///<summary>
        ///</summary>
        public const char SpecialChar = '^';

        private TrieNode FindOnHead(char x)
        {
            return m_header.FirstOrDefault(node => node.Char == x);
        }
        private TrieNode FindOnNode(char x, TrieNode node)
        {
            return node.Links.FirstOrDefault(link => link.Char == x);
        }

        private void Insert(string word, TrieNode x)
        {
            if (word == "")
                return;
            if (x.Links.Count == 0)
            {
                x.Links.Add(new TrieNode { Char = word[0], IsEndWord = word.Length == 1 });
                Insert(word.Substring(1), x.Links[x.Links.Count - 1]);
            }
            else
            {
                var result = FindOnNode(word[0], x);
                if (result == null)
                {
                    x.Links.Add(new TrieNode { Char = word[0], IsEndWord = word.Length == 1 });
                    Insert(word.Substring(1), x.Links[x.Links.Count - 1]);
                }
                else
                    Insert(word.Substring(1), result);
            }
        }
        private void Insert(string word)
        {
            var x = FindOnHead(word[0]);
            if (x == null)
            {
                m_header.Add(new TrieNode { Char = word[0], IsEndWord = word.Length == 1 });
                Insert(word.Substring(1), m_header[m_header.Count - 1]);
            }
            else
                Insert(word.Substring(1), x);
        }

        private IEnumerable<string> Sufixs(TrieNode x)
        {
            if (x.IsEndWord)
            {
                yield return x.Char.ToString();
            }

            foreach (TrieNode link in x.Links)
            {
                var p = Sufixs(link);
                foreach (string cad in p)
                {
                    yield return x.Char + cad;
                }
            }
        }

        ///<summary>
        ///</summary>
        public Trie()
        {
            m_header = new TrieNodeList();
        }

        ///<summary>
        ///</summary>
        ///<param name="pWord"></param>
        ///<returns></returns>
        public IEnumerable<string> Prefixs(string pWord)
        {
            if (pWord == "")
                foreach (TrieNode trieNode in m_header)
                {
                    var p = Sufixs(trieNode);
                    foreach (string cad in p)
                        yield return cad;
                }
            else
            {
                TrieNode x = FindOnHead(pWord[0]);
                if (x == null)
                    yield break;

                TrieNode node = x, aux = null;
                int i = 1;
                while (i < pWord.Length)
                {
                    aux = FindOnNode(pWord[i++], node);
                    if (aux == null)
                        yield break;

                    node = aux;
                }

                if (node.IsEndWord)
                    yield return pWord.Substring(0, i);

                foreach (TrieNode link in node.Links)
                {
                    var p = Sufixs(link);
                    foreach (string cad in p)
                    {
                        yield return pWord.Substring(0, i) + cad;
                    }
                }
            }
        }

        ///<summary>
        /// Return a new Trie sorted
        ///</summary>
        ///<returns></returns>
        public Trie Sorted()
        {
            var result = Clone();
            foreach (TrieNode link in m_header)
                link.Sort();
            
            result.m_header.Sort();

            return result;
        }

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator()
        {
            return Prefixs("").GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICloneable

        /// <summary>
        ///  Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public Trie Clone()
        {
            var result = new Trie();
            foreach (TrieNode node in m_header)
            {
                result.m_header.Add(node.Clone());
            }

            return result;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region Implementation of ICollection<string>

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(string item)
        {
            Insert(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            m_header.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(string item)
        {
            var query = Prefixs(item).ToArray();
            return query.Length == 1;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(string[] array, int arrayIndex)
        {
            var query = Prefixs("").ToArray();

            try
            {
                foreach (var item in query)
                {
                    array[arrayIndex++] = item;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(string item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Implementation of ITree<string>

        /// <summary>
        /// Tree's root
        /// </summary>
        public string Root
        {
            get { return SpecialChar.ToString(); }
        }

        /// <summary>
        /// Get min value of tree
        /// </summary>
        /// <returns></returns>
        public string Min()
        {
            return InOrderTreeWalk().First();
        }

        /// <summary>
        /// Get max valur of tree
        /// </summary>
        /// <returns></returns>
        public string Max()
        {
            return InOrderTreeWalk().Last();
        }

        /// <summary>
        /// Get the in order walk of tree
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> InOrderTreeWalk()
        {
            return Prefixs("").QuickSort();
        }

        #endregion

        public string Print(params object[] args)
        {
            string result = "";

            foreach (var node in m_header)
            {
                result += node.Print();
            }
            
            return result;
        }
    }
}
