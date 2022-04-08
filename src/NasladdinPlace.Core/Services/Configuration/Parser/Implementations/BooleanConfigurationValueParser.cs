using System;

namespace NasladdinPlace.Core.Services.Configuration.Parser.Implementations
{
    public class BooleanConfigurationValueParser : BaseConfigurationValueParser
    {
        protected override bool TryParseCheckedValue<T>(string value, out T parsedValue)
        {
            if (bool.TryParse(value, out var result))
            {
                parsedValue = (T)(object)result;
                return true;
            }

            parsedValue = default(T);
            return false;
        }

        protected override Type ResultType => typeof(bool);
    }
}
