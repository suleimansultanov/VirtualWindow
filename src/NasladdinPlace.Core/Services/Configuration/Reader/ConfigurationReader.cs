using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Parser.Factory;

namespace NasladdinPlace.Core.Services.Configuration.Reader
{
    public class ConfigurationReader : IConfigurationReader
    {   
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IConfigurationValueParsersFactory _configurationValueParsersFactory;
        private readonly ConcurrentDictionary<ConfigurationKeyIdentifier, ConfigurationKey> _configurationKeyByIdentifierDictionary;

        public ConfigurationReader(
            IUnitOfWorkFactory unitOfWorkFactory,
            IConfigurationValueParsersFactory configurationValueParsersFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (configurationValueParsersFactory == null)
                throw new ArgumentNullException(nameof(configurationValueParsersFactory));
            
            _unitOfWorkFactory = unitOfWorkFactory;
            _configurationValueParsersFactory = configurationValueParsersFactory;
            _configurationKeyByIdentifierDictionary = 
                new ConcurrentDictionary<ConfigurationKeyIdentifier, ConfigurationKey>();
            
        }
        
        public void LoadConfiguration()
        {
            if (!_configurationKeyByIdentifierDictionary.IsEmpty) return;
            
            IEnumerable<ConfigurationKey> configurationKeysWithValues;
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                configurationKeysWithValues = unitOfWork.ConfigurationKeys.GetAllIncludingValues();
            }
            
            SaveInMemory(configurationKeysWithValues);
        }

        public void UnloadConfiguration()
        {
            _configurationKeyByIdentifierDictionary.Clear();
        }

        public bool TryGetValueByKey<T>(ConfigurationKeyIdentifier keyIdentifier, out T value)
        {
            value = default(T);
            
            LoadConfiguration();

            var configurationKey = _configurationKeyByIdentifierDictionary[keyIdentifier];
            
            if (configurationKey == null || !configurationKey.HasSingleValue) return false;

            return configurationKey.TryGetSingleParsedValue(_configurationValueParsersFactory, out value);
        }

        public bool TryGetValuesByKey<T>(ConfigurationKeyIdentifier keyIdentifier, out IEnumerable<T> values)
        {
            values = Enumerable.Empty<T>();
            
            LoadConfiguration();

            var configurationKey = _configurationKeyByIdentifierDictionary[keyIdentifier];

            return configurationKey != null && 
                   configurationKey.TryGetParsedValues(_configurationValueParsersFactory, out values);
        }

        private void SaveInMemory(IEnumerable<ConfigurationKey> configurationKeysWithValues)
        {
            foreach (var configurationKeyWithValue in configurationKeysWithValues)
            {
                _configurationKeyByIdentifierDictionary[configurationKeyWithValue.Id] = configurationKeyWithValue;
            }
        }
    }
}