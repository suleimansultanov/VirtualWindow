using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Validators.Contracts;

namespace NasladdinPlace.Core.Services.Configuration.Validators.Factory
{
    public interface IConfigurationValueValidatorsFactory
    {
        IConfigurationValueValidator CreateForDataType(ConfigurationValueDataType valueDataType);
    }
}