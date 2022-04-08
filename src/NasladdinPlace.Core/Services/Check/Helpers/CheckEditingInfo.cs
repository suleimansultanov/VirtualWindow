using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Check.Helpers
{
    public class CheckEditingInfo
    {
        public PosOperation PosOperation { get; }
        public decimal MoneyAmount { get; }
        public decimal BonusPoints { get; private set; }
        public CheckEditingType CheckEditingType { get; }
        public IReadOnlyList<CheckItem> CheckItemsToRefund { get; private set; }

        public CheckEditingInfo(PosOperation posOperation, decimal moneyAmount, CheckEditingType checkEditingType)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            MoneyAmount = moneyAmount;
            PosOperation = posOperation;
            CheckEditingType = checkEditingType;
        }

        public static CheckEditingInfo ForRefund(
            PosOperation posOperation,
            decimal moneyAmount,
            decimal bonusPoints,
            IReadOnlyList<CheckItem> checkItemsToRefund) 
        {
            if (checkItemsToRefund == null)
                throw new ArgumentNullException(nameof(checkItemsToRefund));

            return new CheckEditingInfo(posOperation, moneyAmount, CheckEditingType.Refund)
            {
                CheckItemsToRefund = checkItemsToRefund,
                BonusPoints = bonusPoints
            };
        }
    }
}
