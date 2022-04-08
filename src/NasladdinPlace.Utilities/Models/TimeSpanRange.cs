using System;

namespace NasladdinPlace.Utilities.Models
{
    public struct TimeSpanRange
    {
        public TimeSpan Start { get; }
        public TimeSpan End { get; }

        public TimeSpanRange(TimeSpan start, TimeSpan end)
        {
            Start = start;
            End = end;
        }

        public bool IsInRange(TimeSpan value)
        {
            return value >= Start && value <= End;
        }

        public DateTimeRange CreateDateTimeRange(DateTime originalDateTime)
        {
            if (Start == TimeSpan.MinValue && End == TimeSpan.MaxValue)
                return DateTimeRange.MaxValue;
            if(Start == TimeSpan.MinValue)
                return DateTimeRange.Until(originalDateTime.Add(End));
            if(End == TimeSpan.MaxValue)
                return DateTimeRange.Since(originalDateTime.Add(Start));

            return DateTimeRange.From(originalDateTime.Add(Start), originalDateTime.Add(End));
        }
    }
}
