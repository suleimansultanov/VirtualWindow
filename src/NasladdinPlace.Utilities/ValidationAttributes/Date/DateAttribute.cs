using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Utilities.ValidationAttributes.Date
{
    public class DateAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "Date format is incorrect.";

        public DateAttribute() : base(DefaultErrorMessage)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            return SharedDateTimeConverter.ConvertToUtcDateTime(value.ToString(), out var _)
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}
