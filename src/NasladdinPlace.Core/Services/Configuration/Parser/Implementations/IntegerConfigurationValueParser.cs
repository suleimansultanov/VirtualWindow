using System;

namespace NasladdinPlace.Core.Services.Configuration.Parser.Implementations
{
    public class IntegerConfigurationValueParser : BaseConfigurationValueParser
    {
        protected override bool TryParseCheckedValue<T>(string value, out T parsedValue)
        {
            if (int.TryParse(value, out var result))
            {
                parsedValue = (T)(object)result;
                return true;
            }

            parsedValue = default(T);
            return false;
        }

        protected override Type ResultType => typeof(int);
    }
}