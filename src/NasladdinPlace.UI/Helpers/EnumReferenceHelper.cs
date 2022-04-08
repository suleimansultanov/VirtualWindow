using NasladdinPlace.Utilities.EnumHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.UI.Managers.Reference.Models;

namespace NasladdinPlace.UI.Helpers
{
    public static class EnumReferenceHelper
    {
        public static IEnumerable<ReferencesModel> GetEnumsReferences(params Type[] items)
        {
            return items.Select(i => GetEnumReference(i));
        }

        public static ReferencesModel GetEnumReference(Type item, IReadOnlyCollection<Enum> exclude = null)
        {
            var res = new List<ReferenceItemModel>();

            foreach (var enumVal in Enum.GetValues(item))
            {
                if (exclude != null && exclude.Contains(enumVal))
                    continue;

                var type = enumVal.GetType();
                var memInfo = type.GetMember(enumVal.ToString());

                var attributes = memInfo[0].GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    var enumDescriptionAttribute = (EnumDescriptionAttribute)attributes[0];
                    res.Add(new ReferenceItemModel { Value = ToInt32(enumVal), Text = enumDescriptionAttribute.Text });
                }
                else
                {
                    res.Add(new ReferenceItemModel { Value = ToInt32(enumVal), Text = enumVal.ToString() });
                }
            }

            return new ReferencesModel
            {
                ReferenceType = item.FullName,
                Data = res
            };
        }

        public static ReferenceItemModel GetEnumReferenceItem<T>(T value)
        {
            var type = value.GetType();
            var memInfo = type.GetMember(value.ToString());

            var attributes = memInfo[0].GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                var enumDescriptionAttribute = (EnumDescriptionAttribute)attributes[0];
                return new ReferenceItemModel { Value = ToInt32(value), Text = enumDescriptionAttribute.Text };
            }
            return new ReferenceItemModel { Value = ToInt32(value), Text = value.ToString() };
        }

        private static int ToInt32(object obj, int defaultValue = 0)
        {
            return obj == null || string.IsNullOrEmpty(obj.ToString()) ? defaultValue : Convert.ToInt32(obj);
        }
    }
}
