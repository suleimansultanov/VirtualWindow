using System;

namespace NasladdinPlace.Core.Models
{
    public class PosOperationTransactionCheckItem: Entity
    {
        public CheckItem CheckItem { get; private set; }
        public int CheckItemId { get; private set; }
        public PosOperationTransaction PosOperationTransaction { get; private set; }
        public int PosOperationTransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime CreationDate { get; private set; }
        public decimal CostInBonusPoints { get; private set; }
        public decimal DiscountAmount { get; private set; }

        protected PosOperationTransactionCheckItem()
        {
            // required for EF
        }

        public PosOperationTransactionCheckItem(
            int checkItemId,
            decimal amount, 
            decimal costInBonusPoints, 
            decimal discountAmount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "PosOperationTransactionCheckItem amount money must be greater than zero.");
            if (costInBonusPoints < 0)
                throw new ArgumentOutOfRangeException(nameof(costInBonusPoints), costInBonusPoints, "PosOperationTransactionCheckItem bonus points must be greater than zero.");
            if (discountAmount < 0)
                throw new ArgumentOutOfRangeException(nameof(discountAmount), discountAmount, "PosOperationTransactionCheckItem discount amount must be greater than zero.");

            CheckItemId = checkItemId;
            Amount = amount;
            CreationDate = DateTime.UtcNow;
            CostInBonusPoints = costInBonusPoints;
            DiscountAmount = discountAmount;
        }

        public string GetGoodName()
        {
            return CheckItem.Good.Name;
        }
    }
}
