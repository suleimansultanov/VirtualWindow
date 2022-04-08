using System;

namespace NasladdinPlace.Utilities.DateTimeConverter.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime MinDateTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
        private static readonly string StringDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

        public static long ToMillisecondsSince1970(this DateTime dateTime)
        {
            return Convert.ToInt64(dateTime.Subtract(MinDateTime).TotalMilliseconds);
        }

        public static DateTime ToDateTimeSince1970(this long millisecondsOfDateTime)
        {
            return MinDateTime.AddMilliseconds(millisecondsOfDateTime);
        }

        public static string ToMoscowDateTimeStringMinusDays(this DateTime dateTime, int daysCount)
        {
            return UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(dateTime).AddDays(-daysCount).ToString(StringDateTimeFormat);
        }

        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero) 
                return dateTime; 

            if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue) 
                return dateTime;

            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
    }
}
