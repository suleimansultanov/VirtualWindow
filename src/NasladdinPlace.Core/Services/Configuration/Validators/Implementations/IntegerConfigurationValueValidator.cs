namespace NasladdinPlace.Core.Services.Configuration.Validators.Implementations
{
    public class IntegerConfigurationValueValidator : BaseConfigurationValueValidator
    {
        public new bool Validate(string value)
        {
            return base.Validate(value) && int.TryParse(value, out _);
        }
    }
}