using System;

namespace NasladdinPlace.Core.Models
{
    public class LabeledGoodPrice
    {
        public decimal Price { get; }
        public int CurrencyId { get; }

        public LabeledGoodPrice()
        {
            Price = 0.0M;
            CurrencyId = 0;
        }
        
        public LabeledGoodPrice(decimal price, int currencyId)
        {
            const decimal maxPrice = 9999M;
            if (price < 0 || price > 0 && price < 1 || price > maxPrice)
                throw new ArgumentOutOfRangeException(
                    nameof(price), price, $"Price should be in the range [1, {maxPrice}] or zero."
                );
            
            Price = price;
            CurrencyId = currencyId;
        }
    }
}