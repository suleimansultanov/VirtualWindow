using System.Globalization;

namespace NasladdinPlace.Core.Services.Configuration.Extensions
{
    public static class DoubleConfigurationValuesParserExtensions
    {
        public static bool TryParse(this string value, out double result)
        {
            NumberFormatInfo provider = new NumberFormatInfo { NumberDecimalSeparator = "." };
            return double.TryParse(value, NumberStyles.Float, provider, out result);
        }
    }
}
