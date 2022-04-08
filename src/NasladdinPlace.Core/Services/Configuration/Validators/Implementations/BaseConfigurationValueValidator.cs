using NasladdinPlace.Core.Services.Configuration.Validators.Contracts;

namespace NasladdinPlace.Core.Services.Configuration.Validators.Implementations
{
    public class BaseConfigurationValueValidator : IConfigurationValueValidator
    {
        public bool Validate(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
