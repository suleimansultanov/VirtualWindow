using System;
using System.Globalization;

namespace NasladdinPlace.Utilities.DateTimeConverter
{
    public class UtcStringToDateTimeConverter : IStringToDateTimeConverter
    {
        private const string DateFormat = "dd/MM/yyyy";
        private const string TimeFormatIgnoringSeconds = "HH:mm";
        private const string TimeFormat = TimeFormatIgnoringSeconds + ":ss";
        private const string DateTimeIgnoringSecondsFormat = DateFormat + " " + TimeFormatIgnoringSeconds;
        private const string DateTimeFormat = DateFormat + " " + TimeFormat;

        private static readonly string[] DateTimeFormats =
        {
            DateFormat,
            DateTimeFormat,
            DateTimeIgnoringSecondsFormat,
            DateTimeFormat,
            TimeFormat,
            TimeFormatIgnoringSeconds
        };

        public bool Convert(string dateTimeString, out DateTime dateTime)
        {
            return DateTime.TryParseExact(
                dateTimeString,
                DateTimeFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime
            );
        }
    }
}
