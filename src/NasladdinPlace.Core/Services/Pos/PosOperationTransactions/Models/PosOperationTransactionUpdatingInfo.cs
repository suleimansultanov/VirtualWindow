using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Interfaces;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models
{
    public class PosOperationTransactionUpdatingInfo
    {
        public PosOperationTransaction OperationTransaction { get; private set; }
        public IReadOnlyList<IReadonlyCheckItem> CheckItemsToUpdate { get; }
        public decimal BonusPoints { get; private set; }

        public PosOperationTransactionUpdatingInfo(
            PosOperationTransaction operationTransaction,
            IReadOnlyList<IReadonlyCheckItem> checkItemsToUpdate,
            decimal bonusPoints)
        {
            if (operationTransaction == null)
                throw new ArgumentNullException(nameof(operationTransaction));
            if (checkItemsToUpdate == null)
                throw new ArgumentNullException(nameof(checkItemsToUpdate));
            if (bonusPoints < 0)
                throw new ArgumentOutOfRangeException(nameof(bonusPoints), bonusPoints,
                    "bonusPoints must be greater than zero.");

            OperationTransaction = operationTransaction;
            CheckItemsToUpdate = checkItemsToUpdate;
            BonusPoints = bonusPoints;
        }
    }
}
