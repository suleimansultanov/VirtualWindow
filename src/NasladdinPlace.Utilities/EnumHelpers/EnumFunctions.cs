using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace NasladdinPlace.Utilities.EnumHelpers
{
    /// <summary>
    /// Функции по работе с перечислениями
    /// </summary>
    public static class EnumFunctions
    {

        #region public static string GetDescription(this Enum en)
        /// <summary>
        /// Возвращает описание элемента перечисления (enum)
        /// </summary>
        /// <param name="en">Объект, для которого необходимо получить описание</param>
        /// <returns></returns>
        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo == null || memInfo.Length == 0) return en.ToString();

            // получаем атрибуты объекта
            object[] attrs = memInfo[0].GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

            if (attrs != null && attrs.Length > 0)
                return ((EnumDescriptionAttribute)attrs[0]).Text;

            return en.ToString();
        }
        #endregion


        #region GetEnumSequence
        /// <summary>
        /// Возвращает последовательность всех возможных значений (<see cref="Enum"/>) указанного типа перечесления (<paramref name="enumType"/>)
        /// </summary>
        /// <param name="enumType">Тип перечесления</param>
        /// <exception cref="ArgumentNullException">Параметр enumType имеет значение null.</exception>
        /// <exception cref="ArgumentException">Текущий тип не является перечислением.</exception>
        public static IEnumerable<Enum> GetEnumSequence(this Type enumType)
        {
            // В Enum.GetValues уже есть проверка на null и проверка на enum
            return Enum.GetValues(enumType).Cast<Enum>();
        }

        /// <summary>
        /// Возвращает последовательность всех возможных значений (<see cref="Enum"/>) указанного типа перечесления
        /// </summary>
        /// <typeparam name="T">Тип перечесления</typeparam>
        public static IEnumerable<T> GetEnumSequence<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            // В Enum.GetValues уже есть проверка на null и проверка на enum
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Возвращает последовательность объектов вида <see cref="KeyValueTriplet{TKey,TNumericKey,TValue}"/>: 
        /// Key - значение enum, Value - описание значения (<see cref="EnumDescriptionAttribute"/>), NumericValue: числовое значение enum
        /// </summary>
        /// <param name="enumType">Тип enum</param>
        /// <param name="excludeItems">Список объектов для исключения значений при получении списка</param>
        public static IEnumerable<KeyValueTriplet<Enum, T, string>> GetExtendedEnumSequence<T>(this Type enumType, params T[] excludeItems)
        {
            return from e in enumType.GetEnumSequence()
                   let numericValue = CastEnum<T>(e)
                   where excludeItems == null || !excludeItems.Contains(numericValue)
                   select new KeyValueTriplet<Enum, T, string>(e, numericValue, e.GetDescription());
        }
        #endregion


        #region public static IList<KeyValueTriplet<Enum, T, string>>  ToExtendedList<T>(this Type type, IList<T> excludeItems = null)
        /// <summary>
        /// Осуществляет генерацию списка, чтобы объекты типа Enum можно было использовать в качестве DataSource
        /// </summary>
        /// <param name="enumType">Тип enum для преобразования</param>
        /// <param name="excludeItems">Список объектов для исключения значений при получении списка</param>
        public static IList<KeyValueTriplet<Enum, T, string>> ToExtendedList<T>(this Type enumType, params T[] excludeItems)
        {
            return enumType.GetExtendedEnumSequence(excludeItems).ToList();
        }

        private static T CastEnum<T>(Enum value)
        {
            var realType = typeof(T);
            if (realType.IsGenericType && realType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                realType = typeof(T).GetGenericArguments()[0];
            }

            return (T)Convert.ChangeType(value, realType, CultureInfo.InvariantCulture);
        }
        #endregion

        public static Dictionary<T, string> ToExtendeDictionary<T>(this Type enumType)
        {
            return enumType
                .GetExtendedEnumSequence<T>()
                .ToDictionary(a => a.NumericKey, a => a.Value);
        }

        public static Dictionary<string, int> ToDictionary(this Type enumType)
        {
            return enumType.GetEnumSequence().ToDictionary(e => e.ToString(), Convert.ToInt32);
        }

        public static Dictionary<int, string> ToDictionaryDescription(this Type enumType)
        {
            return enumType.GetEnumSequence().ToDictionary(Convert.ToInt32, GetDescription);
        }

        public static Dictionary<T, string> GetDictionaryDescription<T>() where T : struct, IComparable, IConvertible, IFormattable
        {
            return typeof(T).GetEnumValues().Cast<object>().ToDictionary(e => (T)e, e => GetDescription((Enum)e));
        }

        /// <summary>
        /// Функция вычисления разницы множеств
        /// </summary>        
        /// <param name="collection">Множество енума</param>
        /// <returns>разница множеств енума TEnum и collection</returns>
        public static List<Enum> GetDiffCollection<TEnum>(IReadOnlyCollection<Enum> collection)
            where TEnum : struct, IComparable, IConvertible, IFormattable
            => Enum.GetValues(typeof(TEnum)).Cast<Enum>().Where(i => !collection.Contains(i)).ToList();

    }
}
