using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Parser.Contracts;

namespace NasladdinPlace.Core.Services.Configuration.Parser.Factory
{
    public interface IConfigurationValueParsersFactory
    {
        IConfigurationValueParser Create(ConfigurationValueDataType valueDataType);
    }
}