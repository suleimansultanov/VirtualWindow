using System;
using System.Collections.Generic;
using NasladdinPlace.CloudPaymentsCore;

namespace NasladdinPlace.Fiscalization.Models
{
    public class CheckInfoResult
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<FiscalItem> Items { get; }
        public TaxationSystem TaxationSystem { get; }
        public Amounts Amounts { get; }
        public bool IsBso { get; set; }
        public AdditionalData AdditionalData { get; }

        public CheckInfoResult(
            List<FiscalItem> fiscalItems,
            TaxationSystem taxationSystem,
            Amounts amounts,
            AdditionalData additionalData)
        {
            if (fiscalItems == null)
                throw new ArgumentNullException(nameof(fiscalItems));
            if (!Enum.IsDefined(typeof(TaxationSystem), taxationSystem))
                throw new ArgumentException($"Incorrect value {taxationSystem} of the {nameof(TaxationSystem)} enum.");
            if (additionalData == null)
                throw new ArgumentNullException(nameof(additionalData));

            Items = fiscalItems;
            TaxationSystem = taxationSystem;
            // by specification Amounts can be null
            Amounts = amounts;
            AdditionalData = additionalData;
        }
    }
}
