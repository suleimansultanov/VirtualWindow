using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Interfaces;

namespace NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models
{
    public class PosOperationTransactionCreationInfo
    {
        public IReadonlyPosOperation PosOperation { get; }
        public IReadOnlyList<IReadonlyCheckItem> CheckItems { get; }
        public decimal BonusPoints { get; private set; }
        public PosOperationTransactionType TransactionType { get; }

        public decimal AvailableUserBonusPoints => PosOperation.User.TotalBonusPoints;

        public PosOperationTransactionCreationInfo(
            IReadonlyPosOperation posOperation, 
            IReadOnlyList<IReadonlyCheckItem> checkItems, 
            decimal bonusPoints,
            PosOperationTransactionType transactionType)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));
            if (checkItems == null)
                throw new ArgumentNullException(nameof(checkItems));
            if (bonusPoints < 0)
                throw new ArgumentOutOfRangeException(nameof(bonusPoints), bonusPoints,
                    "bonusPoints must be greater than zero.");
            if (!Enum.IsDefined(typeof(PosOperationTransactionType), transactionType))
                throw new ArgumentException(nameof(transactionType));

            PosOperation = posOperation;
            CheckItems = checkItems;
            BonusPoints = bonusPoints;
            TransactionType = transactionType;
        }
    }
}
