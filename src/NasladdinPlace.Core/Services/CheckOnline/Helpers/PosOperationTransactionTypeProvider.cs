using System;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.CheckOnline.Helpers
{
    public class PosOperationTransactionTypeProvider : IPosOperationTransactionTypeProvider
    {
        public PosOperationTransactionType GetTransactionType(FiscalizationType fiscalizationType)
        {
            switch (fiscalizationType)
            {
                case FiscalizationType.Income:
                    return PosOperationTransactionType.RegularPurchase;
                case FiscalizationType.Correction:
                    return PosOperationTransactionType.Addition;
                case FiscalizationType.IncomeRefund:
                    return PosOperationTransactionType.Refund;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(fiscalizationType),
                        fiscalizationType,
                        $"Unable to find the specified {nameof(FiscalizationType)}."
                    );
            }
        }
    }
}
