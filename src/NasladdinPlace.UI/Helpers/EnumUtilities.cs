using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.UI.Helpers
{
    public static class EnumUtilities
    {
        public static IReadOnlyCollection<T> GetEnumValues<T>() where T: struct, IConvertible
        {
            var typeOfT = typeof(T);
            
            if (!typeOfT.IsEnum)
                throw new ArgumentException($"{typeOfT.Name} is not a enum.");

            return Enum.GetValues(typeOfT).Cast<T>().ToImmutableList();
        }

        public static IReadOnlyCollection<T> GetEnumValuesExcept<T>(params T[] excludedValues)
            where T : struct, IConvertible
        {
            return GetEnumValues<T>().Except(excludedValues).ToImmutableList();
        }
    }
}