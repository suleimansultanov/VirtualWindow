using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Repositories
{
    public interface IConfigurationKeyRepository
    {
        IEnumerable<ConfigurationKey> GetAllIncludingValues();
        Task<ConfigurationKey> GetByIdIncludingValuesAsync(ConfigurationKeyIdentifier keyIdentifier);
        void Add(ConfigurationKey configurationKey);
    }
}