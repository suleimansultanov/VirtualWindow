using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Utilities.ValidationAttributes.Basic
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class LocalizedRangeAttribute : RangeAttribute
    {
        private const string DefaultErrorMessage = "The field {0} must be between {1} and {2}.";

        public LocalizedRangeAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
            ErrorMessage = DefaultErrorMessage;
        }

        public LocalizedRangeAttribute(int minimum, int maximum) : base(minimum, maximum)
        {
            ErrorMessage = DefaultErrorMessage;
        }

        public LocalizedRangeAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
            ErrorMessage = DefaultErrorMessage;
        }
    }
}
