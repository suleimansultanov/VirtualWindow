using System;

namespace NasladdinPlace.Core.Services.Check.Refund.Models
{
    public class PermittedRefundTransaction
    {
        public int TransactionId { get; }
        public decimal PermittedRefundAmount { get; }

        public PermittedRefundTransaction(int trnaTransactionId, decimal permittedRefundAmount)
        {
            if (permittedRefundAmount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(permittedRefundAmount), permittedRefundAmount, "Fiscalization amount money must be greater zero.");

            TransactionId = trnaTransactionId;
            PermittedRefundAmount = permittedRefundAmount;
        }
    }
}
