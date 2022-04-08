using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Utilities.ValidationAttributes.Number
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DoubleValueAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "Double value is incorrect.";

        public DoubleValueAttribute()
        {
            ErrorMessage = DefaultErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (double.TryParse(value.ToString(), out var _))
            {
                return ValidationResult.Success;
            }

            ErrorMessage = DefaultErrorMessage;
            return new ValidationResult(ErrorMessage);
        }
    }
}