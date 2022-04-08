using System;

namespace NasladdinPlace.Core.Services.Configuration.Parser.Implementations
{
    public class StringConfigurationValueParser : BaseConfigurationValueParser
    {
        protected override bool TryParseCheckedValue<T>(string value, out T parsedValue)
        {
            parsedValue = (T)(object) value;

            return true;
        }

        protected override Type ResultType => typeof(string);
    }
}