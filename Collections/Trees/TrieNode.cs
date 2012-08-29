using System;

namespace Nikos.Collections
{
    class TrieNode : ICloneable<TrieNode>, IComparable<TrieNode>, IPrint
    {
        public TrieNode()
        {
            Links = new TrieNodeList();
        }

        public char Char { get; set; }
        public bool IsEndWord { get; set; }
        public TrieNodeList Links { get; set; }

        internal void Sort()
        {
            foreach (TrieNode link in Links)
            {
                link.Sort();
            }
            Links.Sort();            
        }

        #region Implementation of ICloneable

        /// <summary>
        ///  Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public TrieNode Clone()
        {
            var result = new TrieNode { Char = this.Char, IsEndWord = this.IsEndWord };

            foreach (TrieNode link in Links)
            {
                result.Links.Add(link.Clone());
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

        #region Implementation of IComparable<in TrieNode>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(TrieNode other)
        {
            return Char.CompareTo(other.Char);
        }

        #endregion

        private static string PrintWhiteSpace(int number)
        {
            string result = "";
            for (int i = 0; i < number; i++)
            {
                result += " ";
            }
            return result;
        }


        public string Print(params object[] args)
        {
            int number = args.Length == 0 ? 1 : (int)args[0];

            string result = "";

            result += this.Char.ToString() + '\n' + PrintWhiteSpace(number);
            foreach (var link in Links)
            {
                result += link.Print(number++);
            }

            result += '\n';

            return result;
        }
    }
}