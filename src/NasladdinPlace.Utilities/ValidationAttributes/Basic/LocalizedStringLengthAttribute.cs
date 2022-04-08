using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Utilities.ValidationAttributes.Basic
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class LocalizedStringLengthAttribute : StringLengthAttribute
    {
        private const string DefaultErrorMessage = "The field {0} must be a string with a maximum length of {1}.";

        public LocalizedStringLengthAttribute(int maximumLength) : base(maximumLength)
        {
            ErrorMessage = DefaultErrorMessage;
        }
    }
}
