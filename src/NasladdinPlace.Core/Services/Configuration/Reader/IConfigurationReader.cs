using System.Collections.Generic;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
    public interface IConfigurationReader
    {
        void LoadConfiguration();
        void UnloadConfiguration();
        bool TryGetValueByKey<T>(ConfigurationKeyIdentifier keyIdentifier, out T value);
        bool TryGetValuesByKey<T>(ConfigurationKeyIdentifier keyIdentifier, out IEnumerable<T> values);
    }
}