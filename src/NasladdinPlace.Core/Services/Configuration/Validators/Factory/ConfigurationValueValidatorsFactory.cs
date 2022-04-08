using System;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Validators.Contracts;
using NasladdinPlace.Core.Services.Configuration.Validators.Implementations;

namespace NasladdinPlace.Core.Services.Configuration.Validators.Factory
{
    public class ConfigurationValueValidatorsFactory : IConfigurationValueValidatorsFactory
    {
        public IConfigurationValueValidator CreateForDataType(ConfigurationValueDataType valueDataType)
        {
            switch (valueDataType)
            {
                case ConfigurationValueDataType.String:
                    return new StringConfigurationValueValidator();
                case ConfigurationValueDataType.TimeSpan:
                    return new TimeSpanConfigurationValueValidator();
                case ConfigurationValueDataType.Boolean:
                    return new BooleanConfigurationValueValidator();
                case ConfigurationValueDataType.Integer:
                    return new IntegerConfigurationValueValidator();
                case ConfigurationValueDataType.Byte:
                    return new ByteConfigurationValueValidator();
                case ConfigurationValueDataType.Double:
                    return new DoubleConfigurationValueValidator();
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