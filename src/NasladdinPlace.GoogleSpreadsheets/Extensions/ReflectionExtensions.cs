using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NasladdinPlace.Spreadsheets.Extensions
{
    public static class ReflectionExtensions
    {
        public static IList<object> GetFieldsValues(this object type)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public;

            return type.GetType()
                .GetFields(bindingFlags)
                .Select(field => field.GetValue(type)).ToList();
        }

        public static IList<object> GetFieldsNames(this Type type, BindingFlags bindingFlags = BindingFlags.Instance |
                                                                                              BindingFlags.Public)
        {
            var properties = type.GetProperties(bindingFlags);

            var fieldsName = properties.Select(property => (object) (
                property.GetCustomAttributes(typeof(DisplayAttribute), true)
                    .Cast<DisplayAttribute>()
                    .SingleOrDefault()?.Name ?? property.Name)).ToList();

            return fieldsName;
        }
    }
}
