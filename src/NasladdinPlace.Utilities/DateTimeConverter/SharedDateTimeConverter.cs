using System;

namespace NasladdinPlace.Utilities.DateTimeConverter
{
    public class SharedDateTimeConverter
    {
        private static readonly IStringToDateTimeConverter UtcStringToDateTimeConverter = 
            new UtcStringToDateTimeConverter();
        private static readonly IStringToDateTimeConverter StringToMoscowDateTimeConverter =
            new StringToMoscowDateTimeConverter(UtcStringToDateTimeConverter);
        private static readonly IDateTimeToStringConverter DateTimeToStringConverter = 
            new DateTimeToStringConverter();
        private static readonly IStringToDateTimeConverter MoscowStringToUtcDateTimeConverter =
            new MoscowStringToUtcDateTimeConverter(UtcStringToDateTimeConverter);

        private SharedDateTimeConverter()
        {
            // intentionally left empty
        }

        public static string ConvertDatePartToString(DateTime dateTime)
        {
            return DateTimeToStringConverter.ConvertDatePart(dateTime);
        }

        public static string ConvertTimePartToString(DateTime dateTime)
        {
            return DateTimeToStringConverter.ConvertTimePart(dateTime);
        }

        public static string ConvertDateHourMinutePartsToString(DateTime dateTime)
        {
            return DateTimeToStringConverter.ConvertDateHourMinuteParts(dateTime);
        }

        public static string ConvertDateTimeToString(DateTime dateTime)
        {
            return DateTimeToStringConverter.Convert(dateTime);
        }

        public static bool ConvertToUtcDateTime(string dateTimeString, out DateTime dateTime)
        {
            return UtcStringToDateTimeConverter.Convert(dateTimeString, out dateTime);
        }

        public static bool ConvertToMoscowDateTime(string dateTimeString, out DateTime dateTime)
        {
            return StringToMoscowDateTimeConverter.Convert(dateTimeString, out dateTime);
        }

        public static bool ConvertFromMoscowToUtcDateTime(string dateTimeString, out DateTime dateTime)
        {
            return MoscowStringToUtcDateTimeConverter.Convert(dateTimeString, out dateTime);
        }
    }
}
