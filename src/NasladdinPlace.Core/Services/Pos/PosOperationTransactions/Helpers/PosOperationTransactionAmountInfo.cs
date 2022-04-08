using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Models.Interfaces;

namespace NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers
{
    public class PosOperationTransactionAmountInfo
    {
        private readonly decimal _bonusAmount;

        public decimal CheckCostWithoutDiscountAndBonusPoints { get; private set; }
        public decimal CheckCostWithDiscount { get; private set; }
        public decimal TotalDiscount { get; private set; }

        public PosOperationTransactionAmountInfo(IReadOnlyCollection<IReadonlyCheckItem> checkItems, decimal? bonusAmount)
        {
            if (checkItems == null)
                throw new ArgumentNullException(nameof(checkItems));
            if (bonusAmount < 0)
                throw new ArgumentOutOfRangeException(nameof(bonusAmount), bonusAmount, "Bonus amount must be greater than zero.");

            CheckCostWithoutDiscountAndBonusPoints = checkItems.Sum(cki => cki.Price);
            CheckCostWithDiscount = checkItems.Sum(cki => cki.PriceWithDiscount);
            TotalDiscount = checkItems.Sum(cki => cki.RoundedDiscountAmount);
            _bonusAmount = bonusAmount ?? 0M;
        }

        public decimal CheckCostInBonuses => IsBonusAmountGreaterThenCheckCost ? CheckCostWithDiscount : _bonusAmount;

        public decimal CheckCostInMoney => !IsBonusAmountGreaterThenCheckCost ? CheckCostWithDiscount - _bonusAmount : 0M;

        private bool IsBonusAmountGreaterThenCheckCost => _bonusAmount > CheckCostWithDiscount;
    }
}
