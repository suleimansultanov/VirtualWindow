namespace NasladdinPlace.Core.Services.Configuration.Parser.Contracts
{
    public interface IConfigurationValueParser
    {
        bool TryParse<T>(string value, out T parsedValue);
    }
}