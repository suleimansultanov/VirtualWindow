using System;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Core.Utils
{
    public class DateTimeUtils
    {
        private DateTimeUtils()
        {
            // intentionally left empty
        }

        public static DateTime GetValueOrMax(DateTime? dateTime)
        {
            return dateTime ?? DateTime.MaxValue;
        }

        public static DateTime GetValueOrMin(DateTime? dateTime)
        {
            return dateTime ?? DateTime.MinValue;
        }

        public static string GetLastResponseTimeMessage(DateTime? lastResponse)
        {
            if (!lastResponse.HasValue || lastResponse.Value.Equals(DateTime.MinValue)) return "-";
            var moscowNow = UtcMoscowDateTimeConverter.MoscowNow;
            var lastResponseMoscowTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(lastResponse.Value);
            return moscowNow.Date.Equals(lastResponseMoscowTime.Date)
                ? lastResponseMoscowTime.ToString("HH:mm")
                : lastResponseMoscowTime.ToString("dd.MM HH:mm");
        }
    }
}
