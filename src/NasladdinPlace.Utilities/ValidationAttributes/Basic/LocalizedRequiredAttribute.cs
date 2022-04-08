using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Utilities.ValidationAttributes.Basic
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class LocalizedRequiredAttribute : RequiredAttribute
    {
        private const string DefaultErrorMessage = "The {0} field is required.";

        public LocalizedRequiredAttribute()
        {
            ErrorMessage = DefaultErrorMessage;
        }
    }
}
