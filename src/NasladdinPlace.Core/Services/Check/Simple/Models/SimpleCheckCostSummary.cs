using System;
using Currency = NasladdinPlace.Core.Models.Currency;

namespace NasladdinPlace.Core.Services.Check.Simple.Models
{
    public class SimpleCheckCostSummary
    {
        public static readonly SimpleCheckCostSummary FreeOfUnknownCurrency =
            new SimpleCheckCostSummary(
                costWithoutDiscount: decimal.Zero,
                discount: decimal.Zero,
                currency: Currency.Unknown,
                itemsQuantity: 0
            );

        public decimal CostWithoutDiscount { get; }
        public decimal Discount { get; }
        public Currency Currency { get; }
        public int ItemsQuantity { get; }

        public SimpleCheckCostSummary(
            decimal costWithoutDiscount,
            decimal discount,
            Currency currency,
            int itemsQuantity)
        {
            if (costWithoutDiscount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(costWithoutDiscount),
                    costWithoutDiscount,
                    $"Cost with discount must be greater or equal to zero. But found: {costWithoutDiscount}."
                );
            if (discount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(discount),
                    discount,
                    $"Discount must be greater or equal to zero. But found: {discount}."
                );
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));
            if (itemsQuantity < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(itemsQuantity),
                    itemsQuantity,
                    $"Goods quantity must greater or equal to zero. But found: {itemsQuantity}"
                );

            CostWithoutDiscount = costWithoutDiscount;
            Discount = discount;
            Currency = currency;
            ItemsQuantity = itemsQuantity;

            if (CostWithDiscount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(costWithoutDiscount),
                    costWithoutDiscount,
                    $"Cost with discount must be greater of equal to zero. But found {CostWithDiscount}."
                );
        }

        public decimal CostWithDiscount => CostWithoutDiscount - Discount;

        public bool IsFree => CostWithDiscount == decimal.Zero;

        public bool IsEmpty => ItemsQuantity == 0;
    }
}