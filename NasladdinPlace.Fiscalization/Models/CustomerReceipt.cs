using System;
using System.Collections.Generic;
using NasladdinPlace.CloudPaymentsCore;

namespace NasladdinPlace.Fiscalization.Models
{
    public class CustomerReceipt
    {
        public List<FiscalItem> Items { get; }
        public TaxationSystem TaxationSystem { get; }
        public Amounts Amounts { get; }

        public CustomerReceipt(
            List<FiscalItem> fiscalItems, 
            TaxationSystem taxationSystem,
            Amounts amounts)
        {
            if (fiscalItems == null)
                throw new ArgumentNullException(nameof(fiscalItems));
            if (!Enum.IsDefined(typeof(TaxationSystem), taxationSystem))
                throw new ArgumentException($"Incorrect value {taxationSystem} of the {nameof(TaxationSystem)} enum.");
            if (amounts == null)
                throw new ArgumentNullException(nameof(amounts));

            Items = fiscalItems;
            TaxationSystem = taxationSystem;
            Amounts = amounts;
        }
    }
}
