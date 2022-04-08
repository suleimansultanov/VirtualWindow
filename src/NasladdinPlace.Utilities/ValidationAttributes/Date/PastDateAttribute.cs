using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Utilities.ValidationAttributes.Date
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PastDateAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "Date format is incorrect.";

        public PastDateAttribute()
        {
            ErrorMessage = DefaultErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (!SharedDateTimeConverter.ConvertToUtcDateTime(value.ToString(), out var result))
            {
                return new ValidationResult(ErrorMessage);
            }

            return result <= DateTime.Now ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessage, name);
        }
    }
}
