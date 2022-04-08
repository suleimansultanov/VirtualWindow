using System;

namespace NasladdinPlace.Utilities.TimeSpanConverter
{
    public static class TimeSpanToStringConverter
    {
        public static string ToReadableString(TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} д. ", span.Days) : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} ч.", span.Hours) : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} мин. ", span.Minutes) : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} сек.", span.Seconds) : string.Empty);

            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted)) formatted = "0 сек.";

            return formatted;
        }
    }
}