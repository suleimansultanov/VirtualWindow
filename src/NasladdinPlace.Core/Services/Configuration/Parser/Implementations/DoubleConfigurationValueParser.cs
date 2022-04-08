using System;
using NasladdinPlace.Core.Services.Configuration.Extensions;

namespace NasladdinPlace.Core.Services.Configuration.Parser.Implementations
{
    public class DoubleConfigurationValueParser : BaseConfigurationValueParser
    {
        protected override bool TryParseCheckedValue<T>(string value, out T parsedValue)
        {
            if (value.TryParse(out var result))
            {
                parsedValue = (T)(object)result;
                return true;
            }

            parsedValue = default(T);
            return false;
        }

        protected override Type ResultType => typeof(double);
    }
}