using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Extensions
{
    public static class PosOperationTransactionExtensions
    {
        public static void MarkAs(this PosOperationTransaction transaction, PosOperationTransactionStatus status)
        {
            switch (status)
            {
                case PosOperationTransactionStatus.InProcess:
                    transaction.MarkAsInProcess();
                    break;
                case PosOperationTransactionStatus.PaidByBonusPoints:
                    MarkAs(transaction, PosOperationTransactionStatus.InProcess);
                    transaction.MarkAsPaidByBonusPoints();
                    break;
                case PosOperationTransactionStatus.PaidUnfiscalized:
                    MarkAs(transaction, PosOperationTransactionStatus.InProcess);
                    transaction.MarkAsPaidUnfiscalized();
                    break;
                case PosOperationTransactionStatus.PaidFiscalized:
                    MarkAs(transaction, PosOperationTransactionStatus.PaidUnfiscalized);
                    transaction.MarkAsPaidFiscalized();
                    break;
            }
        }
    }
}
