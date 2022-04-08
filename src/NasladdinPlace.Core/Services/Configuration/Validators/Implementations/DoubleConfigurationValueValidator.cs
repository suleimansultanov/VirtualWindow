using NasladdinPlace.Core.Services.Configuration.Extensions;

namespace NasladdinPlace.Core.Services.Configuration.Validators.Implementations
{
    public class DoubleConfigurationValueValidator : BaseConfigurationValueValidator
    {
        public new bool Validate(string value)
        {
            return base.Validate(value) && value.TryParse(out _);
        }
    }
}