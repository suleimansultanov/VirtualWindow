using System.Collections.Generic;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Check.Helpers.Contracts
{
    public interface ICheckManagerBonusPointsHelper
    {
        decimal CalculateBonusPointsAmountThatCanBeWrittenOff(PosOperation posOperation, PosOperationTransactionType transactionType, decimal amountPaidViaMoney);
        decimal RefundBonusPointsForCheckItemsAndReturnCalculatedBonusPoints(PosOperation posOperation, IEnumerable<CheckItem> checkItemsToDelete);
        void RefundBonusPoints(PosOperation posOperation, decimal bonusPoints);
        void SubtractBonusPointsFromUserAndAddToPosOperation(PosOperation posOperation, decimal bonusPoints);
    }
}
