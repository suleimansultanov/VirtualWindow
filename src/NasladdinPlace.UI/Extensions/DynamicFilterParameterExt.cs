using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.DAL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.UI.Extensions
{
    public static class DynamicFilterParameterExt
    {
        public static FilterItemModel GetFilter(this IEnumerable<BaseFilterItemModel> list, string name)
        {
            var filterItemModel = list?.OfType<FilterItemModel>().FirstOrDefault(x => x.PropertyName == name);
            return filterItemModel;
        }

        public static string GetValue(this IEnumerable<BaseFilterItemModel> list, string name)
        {
            var filterItemModel = list.GetItem(x => x.PropertyName == name);
            return filterItemModel?.Value;
        }

        public static FilterItemModel GetItem(this IEnumerable<BaseFilterItemModel> list, Func<FilterItemModel, bool> predicate)
        {
            var filterItemModel = list?.OfType<FilterItemModel>().FirstOrDefault(predicate);
            return filterItemModel;
        }

        public static int? GetValueInt(this IEnumerable<BaseFilterItemModel> list, string name)
        {
            int res;
            var value = list.GetValue(name);
            return int.TryParse(value, out res) ? (int?)res : null;
        }

        public static byte? GetValueByte(this IEnumerable<BaseFilterItemModel> list, string name)
        {
            byte res;
            var value = list.GetValue(name);
            return byte.TryParse(value, out res) ? (byte?)res : null;
        }

        public static DateTime GetDateTimeValue(this IEnumerable<BaseFilterItemModel> list, string name)
        {
            return list.GetFilter(name).GetDateTimeValue();
        }

        public static DateTime GetDateTimeValue(this FilterItemModel item)
        {
            var value = item?.Value;
            if (value == null)
                throw new ArgumentException("Не удалось найти значение фильтра");
            var dynamicFilterDateFormat = value.ToDynamicFilterDateFormat();

            if (dynamicFilterDateFormat == null)
                throw new ArgumentException("Не удалось получить значение фильтра");

            return dynamicFilterDateFormat.Value;
        }

        public static int? GetValueIntNullable(this FilterItemModel item)
        {
            var value = item?.Value;
            if (value == null)
                throw new ArgumentException("Не удалось найти значение фильтра");

            return int.TryParse(value, out var res) ? (int?)res : null;
        }

        public static decimal? GetValueDecimalNullable(this FilterItemModel item)
        {
            var value = item?.Value;
            if (value == null)
                throw new ArgumentException("Не удалось найти значение фильтра");

            return decimal.TryParse(value, out var res) ? (decimal?)res : null;
        }

        public static DateTime? GetDateTimeValueNullable(this FilterItemModel item)
        {
            var value = item?.Value;
            return value == null
                ? (DateTime?)null
                : item.GetDateTimeValue();
        }
    }
}
