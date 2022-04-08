using System;
using NasladdinPlace.Core.Models.Fiscalization;

namespace NasladdinPlace.Core.Models
{
    public class FiscalizationCheckItem : Entity
    {
        public FiscalizationInfo FiscalizationInfo { get; private set; }
        public CheckItem CheckItem { get; private set; }

        public int FiscalizationInfoId { get; private set; }
        public int CheckItemId { get; private set; }
        public decimal Amount { get; private set; }

        protected FiscalizationCheckItem()
        {
            // required for EF
        }

        public FiscalizationCheckItem(CheckItem checkItem, decimal amount)
        {
            if (checkItem == null)
                throw new ArgumentNullException(nameof(checkItem), "Fiscalization checkItem can't be null.");

            if (amount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount), amount, "Fiscalization amount money must be greater zero."
                );

            CheckItem = checkItem;
            Amount = amount;
        }

        public string GetGoodName()
        {
            return CheckItem.Good.Name;
        }
    }
}
