namespace NasladdinPlace.Core.Services.Configuration.Validators.Contracts
{
    public interface IConfigurationValueValidator
    {
        bool Validate(string value);
    }
}