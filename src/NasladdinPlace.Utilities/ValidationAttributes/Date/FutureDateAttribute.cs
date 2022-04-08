using NasladdinPlace.Utilities.DateTimeConverter;
using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Utilities.ValidationAttributes.Date
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FutureDateAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "Date format is incorrect.";

        public FutureDateAttribute()
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

            return result > DateTime.UtcNow ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
