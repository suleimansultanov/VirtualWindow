using System;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Models.Reference
{
    public abstract class BaseFilterItemModel
    {

        public IEnumerable<BaseFilterItemModel> Items { get; set; }
    }


    public class FilterAnd : BaseFilterItemModel
    {
    }


    public class FilterOr : BaseFilterItemModel
    {
    }

    /// <summary>
    /// Модель для описания фильтра
    /// </summary>
    public class FilterItemModel : BaseFilterItemModel
    {

        public string PropertyName { get; set; }

        public string FilterName { get; set; }

        public FilterTypes FilterType { get; set; }

        public string Value { get; set; }

        public SortTypes SortType { get; set; }

        public int SortOrder { get; set; }

        public string Sort { get; set; }

        public CastTypes ForceCastType { get; set; }

        /// <summary>
        /// Запрет на очистку значения фильтра
        /// </summary>
        public bool IsNotCleanValue { get; set; }
    }



    /// <summary>
    /// Тип фильтра
    /// </summary>
    public enum FilterTypes
    {
        Contains,
        Equals,
        Greater,
        Less,
        GreaterOrEquals,
        LessOrEquals,

        // необходим для передачи параметра через контекст
        None
    }

    /// <summary>
    /// Тип сортировки
    /// </summary>
    public enum SortTypes
    {
        None = 0,
        Asc = 1,
        Desc = 2
    }

    public enum CastTypes
    {
        None = 0,
        Int32,
        DateTime,
        Byte
    }

    /// <summary>
    /// Расширение позволяет получить Type по <see cref="CastTypes"/>
    /// </summary>
    public static class CastTypesHelper
    {
        private static readonly IDictionary<CastTypes, Type> CastTypes2Type = new Dictionary<CastTypes, Type>
        {
            {CastTypes.None, typeof(string) },
            {CastTypes.Int32, typeof(int) },
            {CastTypes.DateTime, typeof(DateTime) },
            {CastTypes.Byte, typeof(byte) }
        };

        public static Type GetTypeToCast(this CastTypes ft) => CastTypes2Type[ft];
    }


    public static class FilterTypeHelper
    {
        private static readonly IDictionary<FilterTypes, string> Signs = new Dictionary<FilterTypes, string>
        {
            {FilterTypes.Contains, "Contains" },
            {FilterTypes.Equals, "=" },
            {FilterTypes.Greater, ">" },
            {FilterTypes.Less, "<" },
            {FilterTypes.GreaterOrEquals, ">=" },
            {FilterTypes.LessOrEquals, "<=" },
        };

        public static string GetSign(this FilterTypes ft) => Signs[ft];
    }
}
