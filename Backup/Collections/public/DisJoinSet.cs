using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nikos.Collections.Bases
{
	public class DisJoinSet : IEnumerable<EquivalentClass>
	{
        private Dictionary<IComparable, EquivalentClass> data;

        public DisJoinSet()
        {
            data = new Dictionary<IComparable, EquivalentClass>();
        }
        public DisJoinSet(IEnumerable<IComparable> source)
            : this()
        {
            foreach (var item in source)
                data.Add(item, new EquivalentClass(item));

            var p = data.Keys.ToArray();

            for (int i = 0; i < data.Count - 1; i++)
                for (int j = i + 1; j < data.Count; j++)
                {
                    var result = SetOf(p[i]).Merge(SetOf(p[j]));
                    if (result != null)
                    {
                        data[p[i]] = data[p[j]] = result;
                    }
                }
        }

	    public EquivalentClass SetOf(IComparable item)
	    {
            return data[item];
	    }
        public void Add(IComparable item)
        {
            foreach (var value in data.Values)
            {
                if (value.Add(item))
                    return;
            }
            data.Add(item, new EquivalentClass(item));
        }

	    #region Implementation of IEnumerable

	    /// <summary>
	    ///                     Returns an enumerator that iterates through the collection.
	    /// </summary>
	    /// <returns>
	    ///                     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
	    /// </returns>
	    /// <filterpriority>1</filterpriority>
	    public IEnumerator<EquivalentClass> GetEnumerator()
	    {
            return data.Values.GetEnumerator();
	    }

	    /// <summary>
	    ///                     Returns an enumerator that iterates through a collection.
	    /// </summary>
	    /// <returns>
	    ///                     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
	    /// </returns>
	    /// <filterpriority>2</filterpriority>
	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
	    }

	    #endregion
	}
}
