using System;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.OverdueGoods.Extensions
{
    public static class OverdueTypeExtensions
    {
        public static TimeSpanRange GetTimeSpanRange(this OverdueType type)
        {
            switch (type)
            {
                case OverdueType.Fresh:
                    return new TimeSpanRange(TimeSpan.FromDays(2), TimeSpan.MaxValue);
                case OverdueType.Overdue:
                    return new TimeSpanRange(TimeSpan.MinValue, TimeSpan.Zero);
                case OverdueType.OverdueInDay:
                    return new TimeSpanRange(TimeSpan.Zero, TimeSpan.FromDays(1));
                case OverdueType.OverdueBetweenTommorowAndNextDay:
                    return new TimeSpanRange(TimeSpan.FromDays(1), TimeSpan.FromDays(2));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Does not contains this type.");
            }
        }
    }
}