using System;

namespace NasladdinPlace.Core.Services.Configuration.Validators.Implementations
{
    public class TimeSpanConfigurationValueValidator : BaseConfigurationValueValidator
    {
        public new bool Validate(string value)
        {
            return base.Validate(value) && TimeSpan.TryParse(value, out _);
        }
    }
}