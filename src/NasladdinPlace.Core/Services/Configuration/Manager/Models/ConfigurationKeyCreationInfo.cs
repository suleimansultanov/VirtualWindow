using System;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.Core.Services.Configuration.Manager.Models
{
    public class ConfigurationKeyCreationInfo
    {
        public ConfigurationKeyIdentifier KeyIdentifier { get; }
        public string KeyName { get; }
        public ConfigurationValueDataType ValueDataType { get; }

        public ConfigurationKeyCreationInfo(
            ConfigurationKeyIdentifier keyIdentifier, string keyName, ConfigurationValueDataType valueDataType)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentNullException(nameof(keyName));

            KeyIdentifier = keyIdentifier;
            KeyName = keyName;
            ValueDataType = valueDataType;
        }

        public ConfigurationKey ToConfigurationKey()
        {
            return new ConfigurationKey(KeyIdentifier, KeyName, ValueDataType);
        }
    }
}