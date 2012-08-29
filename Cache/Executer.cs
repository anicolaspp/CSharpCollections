using System;
using Nikos.Collections.Interfaces;


namespace Nikos.Cache
{
    ///<summary>
    /// Invoke the method asociate with the executer
    ///</summary>
    ///<param name="args">Arguments of method</param>
    public delegate void MethodInvoke(params object[] args);

    ///<summary>
    /// Es un mecanismo para ejecutar un metodo asociado a los elementos de la memoria cache antes de ser eliminados
    ///</summary>
    ///<typeparam name="T">El valor del elemento en la memoria cache</typeparam>
    class Executer<T> : IComparable<Executer<T>>, IComparable  where T : ISizable, IComparable
    {
        /// <summary>
        /// The value on memory cache
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// Method asociate with Value
        /// </summary>
        public MethodInvoke m_Info { get; set; }

        public Executer(object value = null, MethodInvoke method = null)
        {
            if (value == null) return;
            if (value is T)
                Value = (T) value;
            else throw new ArgumentException("value");
            m_Info = method;
        }

        /// <summary>
        /// Execute the asociated method
        /// </summary>
        public void Execute()
        {
            if (m_Info != null) m_Info(Value);
        }

        public static void Execute(MethodInvoke method, params object[] args)
        {
            if (method != null)
                method(args);
        }

        #region Implementation of IComparable<Executer<T>>

        /// <summary>
        ///                     Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        ///                     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
        ///                     Value 
        ///                     Meaning 
        ///                     Less than zero 
        ///                     This object is less than the <paramref name="other" /> parameter.
        ///                     Zero 
        ///                     This object is equal to <paramref name="other" />. 
        ///                     Greater than zero 
        ///                     This object is greater than <paramref name="other" />. 
        /// </returns>
        /// <param name="other">
        ///                     An object to compare with this object.
        ///                 </param>
        public int CompareTo(Executer<T> other)
        {
            return Value.CompareTo(other.Value);
        }

        #endregion

        #region Implementation of IComparable

        /// <summary>
        ///                     Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        ///                     A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: 
        ///                     Value 
        ///                     Meaning 
        ///                     Less than zero 
        ///                     This instance is less than <paramref name="obj" />. 
        ///                     Zero 
        ///                     This instance is equal to <paramref name="obj" />. 
        ///                     Greater than zero 
        ///                     This instance is greater than <paramref name="obj" />. 
        /// </returns>
        /// <param name="obj">
        ///                     An object to compare with this instance. 
        ///                 </param>
        /// <exception cref="T:System.ArgumentException"><paramref name="obj" /> is not the same type as this instance. 
        ///                 </exception><filterpriority>2</filterpriority>
        public int CompareTo(object obj)
        {
            return CompareTo((Executer<T>)obj);
        }

        #endregion
    }
}