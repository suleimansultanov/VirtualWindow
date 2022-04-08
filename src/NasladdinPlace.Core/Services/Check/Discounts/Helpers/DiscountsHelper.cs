using System;

namespace NasladdinPlace.Core.Services.Check.Discounts.Helpers
{
    public static class DiscountsHelper
    {
        public static decimal Round(decimal value)
        {
            return Math.Ceiling(value);
        }
    }
}
