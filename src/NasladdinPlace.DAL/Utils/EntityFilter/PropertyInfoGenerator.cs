using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace NasladdinPlace.DAL.Utils.EntityFilter
{
    public static class PropertyInfoGenerator
    {
        public static PropertyInfo GetValidProperty(string propertyName, Type type)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            if (!propertyName.Contains("."))
            {
                return GetProperty(propertyName, type);
            }

            PropertyInfo result = null;

            var splitProperty = propertyName.Split('.');
            var currentType = type;

            foreach (var item in splitProperty)
            {
                var propertyInfo = GetProperty(item, currentType);
                if (propertyInfo == null)
                {
                    result = null;
                    break;
                }

                currentType = propertyInfo.PropertyType;
                result = propertyInfo;
            }

            return result;
        }

        private static PropertyInfo GetProperty(string propertyName, Type type)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            var prop = type.GetProperties().FirstOrDefault(x => x.Name == propertyName);
            if (prop?.GetCustomAttributes(typeof(NotMappedAttribute), false).FirstOrDefault() is NotMappedAttribute)
                return null;

            return prop;
        }

    }
}