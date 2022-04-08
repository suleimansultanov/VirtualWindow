using System;
using System.Globalization;

namespace NasladdinPlace.Utilities.DateTimeConverter
{
    internal class DateTimeToStringConverter : IDateTimeToStringConverter
    {
        private const string DateFormat = "dd/MM/yyyy";
        private const string TimeFormatIgnoringSeconds = "HH:mm";
        private const string TimeFormat = TimeFormatIgnoringSeconds + ":ss";
        private const string DateTimeIgnoringSecondsFormat = DateFormat + " " + TimeFormatIgnoringSeconds;
        private const string DateTimeFormat = DateFormat + " " + TimeFormat;

        public string ConvertDatePart(DateTime dateTime)
        {
            return ToFormatedString(DateFormat, dateTime);
        }
        public string ConvertTimePart(DateTime dateTime)
        {
            return ToFormatedString(TimeFormatIgnoringSeconds, dateTime);
        }

        public string ConvertDateHourMinuteParts(DateTime dateTime)
        {
            return ToFormatedString(DateTimeIgnoringSecondsFormat, dateTime);
        }

        public string Convert(DateTime dateTime)
        {
            return ToFormatedString(DateTimeFormat, dateTime);
        }

        private static string ToFormatedString(string format, DateTime dateTime)
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
