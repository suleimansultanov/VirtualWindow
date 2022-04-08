using System;

namespace NasladdinPlace.Utilities.Models
{
    public struct DateTimeRange
    {
        public static readonly DateTimeRange MaxValue = new DateTimeRange(
            startDateTime: DateTime.MinValue,
            endDateTime: DateTime.MaxValue
        );
        
        public DateTime Start { get; }
        public DateTime End { get; }

        private DateTimeRange(DateTime? startDateTime = null, DateTime? endDateTime = null)
        {
            Start = startDateTime ?? DateTime.MinValue;
            End = endDateTime ?? DateTime.MaxValue;

            if (End < Start)
                throw new ArgumentException(
                    $"End date must be greater or equal to start date. " +
                    $"But found start date is {startDateTime}, end date is {endDateTime}"
                );
        }

        public static DateTimeRange Since(DateTime startDateTime)
        {
            return new DateTimeRange(startDateTime);
        }

        public static DateTimeRange Until(DateTime dateTimeUntil)
        {
            return new DateTimeRange(endDateTime: dateTimeUntil);
        }

        public static DateTimeRange From(DateTime startDateTime, DateTime dateTimeUntil)
        {
            return new DateTimeRange(startDateTime, dateTimeUntil);
        }
    }
}
