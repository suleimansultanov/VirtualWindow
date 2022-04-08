using System;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Utilities.DateTimeRange
{
    public static class UtcMoscowDateTimeRangeUtilities
    {
        public static Models.DateTimeRange ComputeMoscowDateTimeRangeForTimespanDaysAgo(
            TimeSpanRange reportTimeRange, int daysAgo)
        {
            var previousDateInMoscow =
                UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(DateTime.UtcNow).Date.AddDays(-daysAgo);

            var reportStartUtcDateTime =
                UtcMoscowDateTimeConverter.ConvertToUtcDateTime(previousDateInMoscow.Add(reportTimeRange.Start));
            var reportUntilUtcDateTime =
                UtcMoscowDateTimeConverter.ConvertToUtcDateTime(previousDateInMoscow.Add(reportTimeRange.End));

            return Models.DateTimeRange.From(
                startDateTime: reportStartUtcDateTime,
                dateTimeUntil: reportUntilUtcDateTime
            );
        }
    }
}