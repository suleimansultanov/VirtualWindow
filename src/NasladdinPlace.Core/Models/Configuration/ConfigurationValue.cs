using System;

namespace NasladdinPlace.Core.Models.Configuration
{
    public class ConfigurationValue : Entity
    {
        private string _value;
        
        public ConfigurationKeyIdentifier KeyId { get; private set; }

        public string Value
        {
            get => _value;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));
                
                _value = value;
            }
        }

        protected ConfigurationValue()
        {
            // required for EF
        }

        public ConfigurationValue(ConfigurationKeyIdentifier keyId, string value)
        {
            KeyId = keyId;
            Value = value;
        }
    }
}