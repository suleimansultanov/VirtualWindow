namespace NasladdinPlace.Core.Services.Configuration.Validators.Implementations
{
    public class ByteConfigurationValueValidator : BaseConfigurationValueValidator
    {
        public new bool Validate(string value)
        {
            return base.Validate(value) && byte.TryParse(value, out _);
        }
    }
}