using System.Threading.Tasks;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Manager.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Configuration.Manager.Contracts
{
    public interface IConfigurationManager
    {
        Task<Result> TryCreateKeyIfNotExistsAndSetValueAsync(
            ConfigurationKeyCreationInfo configurationKeyCreationInfo, string value
        );
        Task<Result> TrySetValueAsync(ConfigurationKeyIdentifier keyIdentifier, string value);
        Task<ValueResult<string>> TryGetValueByKeyAsync(ConfigurationKeyIdentifier keyIdentifier);
    }
}