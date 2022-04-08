using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace NasladdinPlace.Utilities.EnumHelpers
{
    /// <summary>
    /// Определяет тройку ключ/числовое значение/значение
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TNumericKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct KeyValueTriplet<TKey, TNumericKey, TValue>
    {
        private TKey key;
        private TValue value;
        private TNumericKey numericKey;

        #region Key
        /// <summary>
        /// Возвращает ключ
        /// </summary>
        public TKey Key
        {
            get
            {
                return this.key;
            }
        }
        #endregion

        #region NumericKey
        /// <summary>
        /// Возвращает числовое значение ключа
        /// </summary>
        public TNumericKey NumericKey
        {
            get
            {
                return this.numericKey;
            }
        }
        #endregion

        #region Value
        /// <summary>
        /// Возвращает значение
        /// </summary>
        public TValue Value
        {
            get
            {
                return this.value;
            }
        }
        #endregion

        #region Конструктор
        /// <summary>
        /// Inititalizes a new instance of the <see cref="KeyValueTriplet{TKey, TNumericKey, TValue}"/>
        /// structure with the specified key, numeric key, and value.
        /// </summary>
        public KeyValueTriplet(TKey key, TNumericKey numericKey, TValue value)
        {
            this.key = key;
            this.value = value;
            this.numericKey = numericKey;
        }
        #endregion

        #region ToString
        /// <summary>
        /// Возвращает строковое представление
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            if (this.Key != null)
            {
                builder.Append(this.Key.ToString());
            }
            builder.Append(", ");
            if (this.NumericKey != null)
            {
                builder.Append(this.NumericKey.ToString());
            }
            builder.Append(", ");
            if (this.Value != null)
            {
                builder.Append(this.Value.ToString());
            }
            builder.Append(']');
            return builder.ToString();
        }
        #endregion

    }


    public class Triplet<T1, T2, T3>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }

        public Triplet()
        {
        }

        public Triplet(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
    }
}
