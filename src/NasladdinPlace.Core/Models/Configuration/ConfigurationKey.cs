using NasladdinPlace.Core.Services.Configuration.Validators.Factory;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Core.Services.Configuration.Parser.Factory;

namespace NasladdinPlace.Core.Models.Configuration
{
    public sealed class ConfigurationKey : IEquatable<ConfigurationKey>
    {
        private string _name;
        private string _description;

        public ICollection<ConfigurationKey> Children { get; private set; }
        public IList<ConfigurationValue> Values { get; private set; }

        public ConfigurationKeyIdentifier Id { get; private set; }

        public ConfigurationValueDataType ValueDataType { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                _name = value;
            }
        }

        public string Description
        {
            get => _description;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                _description = value;
            }
        }

        public ConfigurationKeyIdentifier? ParentId { get; set; }

        private ConfigurationKey()
        {
            Values = new List<ConfigurationValue>();
            Children = new Collection<ConfigurationKey>();
            _description = string.Empty;
        }

        public ConfigurationKey(ConfigurationKeyIdentifier id, string name, ConfigurationValueDataType valueDataType)
            : this()
        {
            Id = id;
            Name = name;
            ValueDataType = valueDataType;
        }

        public bool Equals(ConfigurationKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ConfigurationKey) obj);
        }

        public override int GetHashCode()
        {
            return (int) Id;
        }

        public static bool operator ==(ConfigurationKey left, ConfigurationKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ConfigurationKey left, ConfigurationKey right)
        {
            return !Equals(left, right);
        }

        public bool IsValidValue(
            ConfigurationValue value,
            IConfigurationValueValidatorsFactory configurationValueValidatorsFactory)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (configurationValueValidatorsFactory == null)
                throw new ArgumentNullException(nameof(configurationValueValidatorsFactory));

            var validator = configurationValueValidatorsFactory.CreateForDataType(ValueDataType);
            return validator.Validate(value.Value);
        }

        public bool TrySetValue(
            ConfigurationValue value,
            IConfigurationValueValidatorsFactory configurationValueValidatorsFactory)
        {
            if (!IsValidValue(value, configurationValueValidatorsFactory)) return false;

            if (Values.Count > 0)
            {
                var existingValue = Values.First();
                existingValue.Value = value.Value;
            }
            else
            {
                Values.Add(value);
            }

            return true;
        }

        public bool TryAddValue(
            ConfigurationValue value,
            IConfigurationValueValidatorsFactory configurationValueValidatorsFactory)
        {
            if (!IsValidValue(value, configurationValueValidatorsFactory)) return false;

            Values.Add(value);

            return true;
        }

        public ConfigurationValue SingleValue() => Values.Single();

        public bool HasSingleValue => Values.Count == 1;

        public bool TryGetSingleParsedValue<T>(
            IConfigurationValueParsersFactory configurationValueParsersFactory, out T parsedValue)
        {
            if (configurationValueParsersFactory == null)
                throw new ArgumentNullException(nameof(configurationValueParsersFactory));

            parsedValue = default(T);

            if (!HasSingleValue) return false;

            var configurationValueParser = configurationValueParsersFactory.Create(ValueDataType);
            return configurationValueParser.TryParse(SingleValue().Value, out parsedValue);
        }

        public bool TryGetParsedValues<T>(
            IConfigurationValueParsersFactory configurationValueParsersFactory, out IEnumerable<T> parsedValues)
        {
            if (configurationValueParsersFactory == null)
                throw new ArgumentNullException(nameof(configurationValueParsersFactory));

            var parsedValuesCollection = new Collection<T>();

            parsedValues = parsedValuesCollection;

            var parser = configurationValueParsersFactory.Create(ValueDataType);

            foreach (var configurationValue in Values)
            {
                if (parser.TryParse(configurationValue.Value, out T parsedValue))
                {
                    parsedValuesCollection.Add(parsedValue);
                }
                else
                {
                    return false;
                }
            }

            parsedValues = parsedValuesCollection.ToImmutableList();

            return true;
        }
    }
}