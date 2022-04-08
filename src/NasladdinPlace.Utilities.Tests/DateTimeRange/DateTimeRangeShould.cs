using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace NasladdinPlace.Utilities.Tests.DateTimeRange
{
    public class DateTimeRangeShould
    {
        private static readonly DateTime Now = 
            new DateTime(2014, 11, 6, 4, 32, 56, 78);

        [Theory]
        [MemberData(nameof(Inputs))]
        public void ReturnDateTimeRangeWithExpectedStartAndEndDateTimeWhenUsedCorrectStartAndEndDateTime(DateTime startDateTime, DateTime endDateTime)
        {
            var dateTimeRange = Models.DateTimeRange.From(startDateTime, endDateTime);
            AssertExpectedDateTimeRange(dateTimeRange, startDateTime, endDateTime);
        }

        [Fact]
        public void ReturnMinMaxValueForMaxDateTimeRange()
        {
            var dateTimeRange = Models.DateTimeRange.MaxValue;
            AssertExpectedDateTimeRange(dateTimeRange, DateTime.MinValue, DateTime.MaxValue);
        }

        [Fact]
        public void ReturnMinAndEndDateTimeWhenEndDateTimeIsGiven()
        {
            var dateTimeRange = Models.DateTimeRange.Until(Now);
            AssertExpectedDateTimeRange(dateTimeRange, DateTime.MinValue, Now);
        }

        [Fact]
        public void ReturnStartAndMaxDateTimeWhenStartDateTimeIsGiven()
        {
            var dateTimeRange = Models.DateTimeRange.Since(Now);
            AssertExpectedDateTimeRange(dateTimeRange, Now, DateTime.MaxValue);
        }

        [Fact]
        public void ThrowExceptionWhenStartDateGreaterThanEndDate()
        {
            Action action = () => Models.DateTimeRange.From(DateTime.MaxValue, DateTime.MinValue);
            action.Should().Throw<ArgumentException>();
        }


        private void AssertExpectedDateTimeRange(Models.DateTimeRange dateTimeRange, DateTime expectedStartDateTime, DateTime expectedEndDateTime)
        {
            dateTimeRange.Should().NotBeNull();
            dateTimeRange.Start.Should().Be(expectedStartDateTime);
            dateTimeRange.End.Should().Be(expectedEndDateTime);
        }

        public static IEnumerable<object[]> Inputs => new[]
        {
            new object[] {Now.AddDays(1), Now.AddDays(1)},
            new object[] {Now.AddMinutes(-5), Now},
            new object[] {Now, Now.AddMinutes(5)}
        };
    }
}
