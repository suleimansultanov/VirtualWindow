using System;
using FluentAssertions;
using NasladdinPlace.Utilities.DateTimeConverter;
using NasladdinPlace.Utilities.DateTimeRange;
using NasladdinPlace.Utilities.Models;
using Xunit;

namespace NasladdinPlace.Utilities.Tests.DateTimeRange
{
    public class UtcMoscowDateTimeRangeUtilitiesShould
    {
        [Fact]
        public void ReturnDateTimeRangeSpanOfYesterday()
        {
            var timeSpanRange = new TimeSpanRange(TimeSpan.FromHours(0), TimeSpan.FromHours(23));
            var dateTimeRange = UtcMoscowDateTimeRangeUtilities.ComputeMoscowDateTimeRangeForTimespanDaysAgo(timeSpanRange, 1);

            var day = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(DateTime.UtcNow).Date.AddDays(-1);

            AssertExpectedDateTimeRange(dateTimeRange,
                UtcMoscowDateTimeConverter.ConvertToUtcDateTime(day.Add(TimeSpan.FromHours(0))),
                UtcMoscowDateTimeConverter.ConvertToUtcDateTime(day.Add(TimeSpan.FromHours(23))));
        }

        [Fact]
        public void ReturnDateTimeRangeSpanOfToday()
        {
            var timeSpanRange = new TimeSpanRange(TimeSpan.FromHours(0), TimeSpan.FromHours(23));
            var dateTimeRange = UtcMoscowDateTimeRangeUtilities.ComputeMoscowDateTimeRangeForTimespanDaysAgo(timeSpanRange, 0);

            var day = DateTime.UtcNow;

            AssertExpectedDateTimeRange(dateTimeRange,
                UtcMoscowDateTimeConverter.ConvertToUtcDateTime(UtcMoscowDateTimeConverter
                    .ConvertToMoscowDateTime(day).Date.Add(TimeSpan.FromHours(0))),
                UtcMoscowDateTimeConverter.ConvertToUtcDateTime(UtcMoscowDateTimeConverter
                    .ConvertToMoscowDateTime(day).Date.Add(TimeSpan.FromHours(23))));
        }

        [Fact]
        public void ReturnCorrectDateTimeRangeByMinutes()
        {
            var timeSpanRange = new TimeSpanRange(TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(30));
            var dateTimeRange = UtcMoscowDateTimeRangeUtilities.ComputeMoscowDateTimeRangeForTimespanDaysAgo(timeSpanRange, 0);

            var day = DateTime.UtcNow;

            AssertExpectedDateTimeRange(dateTimeRange,
                UtcMoscowDateTimeConverter.ConvertToUtcDateTime(UtcMoscowDateTimeConverter
                    .ConvertToMoscowDateTime(day).Date.Add(TimeSpan.FromMinutes(0))),
                UtcMoscowDateTimeConverter.ConvertToUtcDateTime(UtcMoscowDateTimeConverter
                    .ConvertToMoscowDateTime(day).Date.Add(TimeSpan.FromMinutes(30))));
        }

        private void AssertExpectedDateTimeRange(Models.DateTimeRange dateTimeRange, DateTime expectedStartDateTime, DateTime expectedEndDateTime)
        {
            dateTimeRange.Should().NotBeNull();
            dateTimeRange.Start.Date.Should().Be(expectedStartDateTime.Date);
            dateTimeRange.End.Date.Should().Be(expectedEndDateTime.Date);
        }
    }
}