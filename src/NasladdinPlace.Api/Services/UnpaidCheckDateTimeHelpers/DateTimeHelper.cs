using NasladdinPlace.Utilities.DateTimeConverter.Extensions;
using System;

namespace NasladdinPlace.Api.Services.UnpaidCheckDateTimeHelpers
{
    public static class DateTimeHelper
    {
        public static DateTime CalculateNextPaymentDateTime(DateTime lastPaymentDateTime, TimeSpan paymentTimeout)
        {
            return lastPaymentDateTime.Add(paymentTimeout).Truncate(TimeSpan.FromSeconds(1));
        }
    }
}
