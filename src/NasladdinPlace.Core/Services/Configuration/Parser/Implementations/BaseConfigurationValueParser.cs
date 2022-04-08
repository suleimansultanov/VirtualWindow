using System;
using NasladdinPlace.Core.Services.Configuration.Parser.Contracts;

namespace NasladdinPlace.Core.Services.Configuration.Parser.Implementations
{
    public abstract class BaseConfigurationValueParser : IConfigurationValueParser
    {
        public bool TryParse<T>(string value, out T parsedValue)
        {
            parsedValue = default(T);
            
            if (string.IsNullOrWhiteSpace(value) || typeof(T) != ResultType) return false;

            return TryParseCheckedValue(value, out parsedValue);
        }

        protected abstract bool TryParseCheckedValue<T>(string value, out T parsedValue);
        
        protected abstract Type ResultType { get; }
    }
}