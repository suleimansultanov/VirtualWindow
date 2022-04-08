using System;

namespace NasladdinPlace.Api.Dtos.Check
{
    [Obsolete("Used in order to support older versions of mobile apps. " +
              "iOS version 2.0 or lower. " +
              "Android version 1.9 or lower.")]
    public class CheckInfoDto
    {
        public CheckInfoDto(decimal pricePerGood, string currency, int quantity)
        {
            PricePerGood = pricePerGood;
            Currency = currency;
            Quantity = quantity;
            TotalPrice = PricePerGood * Quantity;
        }

        public decimal PricePerGood { get; private set; }
        public decimal TotalPrice { get; private set; }
        public int Quantity { get; private set; }
        public string Currency { get; private set; }
    }
}
