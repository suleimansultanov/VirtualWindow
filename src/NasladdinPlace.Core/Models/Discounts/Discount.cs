using NasladdinPlace.Core.Enums.Discounts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Core.Models.Discounts
{
    public class Discount : Entity
    {
        public bool IsEnabled { get; private set; }

        public DateTime DateTimeCreated { get; private set; }

        public string Name { get; private set; }

        public decimal DiscountInPercentage { get; private set; }

        public DiscountArea DiscountArea { get; private set; }

        public ICollection<DiscountRule> DiscountRules { get; set; }

        public ICollection<PosDiscount> PosDiscounts { get; set; }

        protected Discount() { }

        public Discount(string name, decimal discountInPercentage, DiscountArea discountArea, bool isEnabled)
        {
            IsEnabled = isEnabled;
            Name = name;
            DiscountInPercentage = discountInPercentage;
            DiscountArea = discountArea;
            DateTimeCreated = DateTime.UtcNow;
            DiscountRules = new Collection<DiscountRule>();
            PosDiscounts = new Collection<PosDiscount>();
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Update(string name, decimal discountInPercentage, DiscountArea discountArea, bool isEnabled)
        {
            IsEnabled = isEnabled;
            Name = name;
            DiscountInPercentage = discountInPercentage;
            DiscountArea = discountArea;
        }
    }
}
