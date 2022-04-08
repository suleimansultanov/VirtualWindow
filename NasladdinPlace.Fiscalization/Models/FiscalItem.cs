using System;

namespace NasladdinPlace.Fiscalization.Models
{
    public class FiscalItem
    {
        public string Label { get; }
        public decimal Price { get; }
        public decimal Quantity { get; }
        public decimal Amount { get; }
        public VatValues? Vat { get; }

        public FiscalItem(
            string label,
            decimal price,
            decimal quantity,
            VatValues? vat)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));
            if (price <= 0)
                throw new ArgumentOutOfRangeException(nameof(price), price, "price must be greater than zero.");
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "quantity must be greater than zero.");
            
            Label = label;
            Price = Amount = price;
            Quantity = quantity;
            Vat = vat;
        }
    }
}
