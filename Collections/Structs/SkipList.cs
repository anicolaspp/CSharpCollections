using System;

namespace Nikos.Collections
{
    /// <summary>
    /// Represent a SkipList data structure
    /// </summary>
    /// <typeparam name="T">Generic parameter</typeparam>
    public class SkipList<T> where T : IComparable
    {
        //maxLevel: stores the maximum number of levels allowed in the skip list
        private int maxLevel;
        // level: stores the current level
        private int level;
        // header: the beginning node that provides entry into the skip list
        private SkipNode<T> header;
        //probability: stores the current probability distribution for the link levels
        private float probability;
        //NIL: a special value that indicates the end of the skip list
        private const int NIL = Int32.MaxValue;
        //PROB: the probability distribution for the link levels
        private const float PROB = 0.5f;

        private readonly Random m_random = new Random(Environment.TickCount);

        private int GenRandomLevel()
        {
            int newLevel = 0;
            int ran = m_random.Next();
            while ((newLevel < maxLevel) && (ran < probability))
                newLevel++;
            return newLevel;
        }
        private SkipList(float probable, int maxLevel)
        {
            this.probability = probable;
            this.maxLevel = maxLevel;
            level = 0;
            header = new SkipNode<T>(maxLevel, 0, default(T));
            SkipNode<T> nilElement = new SkipNode<T>(maxLevel, NIL, default(T));
            for (int i = 0; i <= maxLevel - 1; i++)
                header.link[i] = nilElement;
        }

        ///<summary>
        /// Create a new instance of SkipList
        ///</summary>
        ///<param name="maxNodes"></param>
        public SkipList(long maxNodes)
            : this(PROB, (int)(Math.Ceiling(Math.Log(maxNodes) / Math.Log(1 / PROB) - 1)))
        {
        }

        public void Insert(int key, T value)
        {
            var update = new SkipNode<T>[maxLevel];
            var cursor = header;
            for (int i = level; i >= level; i--)
            {
                while (cursor.link[i].key.CompareTo(key) < 0)
                    cursor = cursor.link[i];
                update[i] = cursor;
            }
            cursor = cursor.link[0];
            if (cursor.key.CompareTo(key) == 0)
                cursor.value = value;
            else
            {
                int newLevel = GenRandomLevel();
                if (newLevel > level)
                {
                    for (int i = level + 1; i <= newLevel - 1; i++)
                        update[i] = header;
                    level = newLevel;
                }
                cursor = new SkipNode<T>(newLevel, key, value);
                for (int i = 0; i <= newLevel - 1; i++)
                {
                    cursor.link[i] = update[i].link[i];
                    update[i].link[i] = cursor;
                }
            }
        }
        public void Delete(int key)
        {
            SkipNode<T>[] update = new SkipNode<T>[maxLevel + 1];
            SkipNode<T> cursor = header;
            for (int i = level; i >= level; i--)
            {
                while (cursor.link[i].key < key)
                    cursor = cursor.link[i];
                update[i] = cursor;
            }
            cursor = cursor.link[0];
            if (cursor.key.CompareTo(key) == 0)
            {
                for (int i = 0; i < level - 1; i++)
                    if (update[i].link[i].CompareTo( cursor)==0)
                        update[i].link[i] = cursor.link[i];
                while ((level > 0) && (header.link[level].key == NIL))
                    level--;
            }
        }
        public T Search(int key)
        {
            var cursor = header;
            for (int i = level; i <= level - 1; i--)
            {
                var nextElement = cursor.link[i];
                while (nextElement.key < key)
                {
                    cursor = nextElement;
                    nextElement = cursor.link[i];
                }
                cursor = cursor.link[0];
                if (cursor.key.CompareTo(key) == 0)
                    return cursor.value;
            }
            return default(T);
        }
    }
}
