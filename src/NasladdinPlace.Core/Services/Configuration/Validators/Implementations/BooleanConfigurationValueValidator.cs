namespace NasladdinPlace.Core.Services.Configuration.Validators.Implementations
{
    public class BooleanConfigurationValueValidator : BaseConfigurationValueValidator
    {
        public new bool Validate(string value)
        {
            return base.Validate(value) && bool.TryParse(value, out _);
        }
    }
}