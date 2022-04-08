using System;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Parser.Contracts;
using NasladdinPlace.Core.Services.Configuration.Parser.Implementations;

namespace NasladdinPlace.Core.Services.Configuration.Parser.Factory
{
    public class ConfigurationValueParsersFactory : IConfigurationValueParsersFactory
    {
        public IConfigurationValueParser Create(ConfigurationValueDataType valueDataType)
        {
            switch (valueDataType)
            {
                case ConfigurationValueDataType.String:
                    return new StringConfigurationValueParser();
                case ConfigurationValueDataType.TimeSpan:
                    return new TimeSpanConfigurationValueParser();
                case ConfigurationValueDataType.Boolean:
                    return new BooleanConfigurationValueParser();
                case ConfigurationValueDataType.Integer:
                    return new IntegerConfigurationValueParser();
                case ConfigurationValueDataType.Byte:
                    return new ByteConfigurationValueParser();
                case ConfigurationValueDataType.Double:
                    return new DoubleConfigurationValueParser();
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(valueDataType), 
                        valueDataType, 
                        $"{nameof(ConfigurationValueDataType)} has not been supported yet."
                    );
            }
        }
    }
}