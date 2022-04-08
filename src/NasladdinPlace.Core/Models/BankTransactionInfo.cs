using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using System;

namespace NasladdinPlace.Core.Models
{
    [Obsolete("Will be removed in the future releases")]
    public class BankTransactionInfo : Entity
    {
        public static BankTransactionInfo CreateFromSummary(BankTransactionSummary bankTransactionSummary)
        {
            return ForPayment(
                bankTransactionSummary.PaymentCardId,
                bankTransactionSummary.BankTransactionId,
                bankTransactionSummary.Amount
            );
        }

        public static BankTransactionInfo ForPayment(int paymentCardId, int bankTransactionId, decimal amount)
        {
            return new BankTransactionInfo(bankTransactionId, amount, BankTransactionInfoType.Payment, null)
            {
                PaymentCardId = paymentCardId
            };
        }

        public static BankTransactionInfo ForRefund(int bankTransactionId, decimal amount)
        {
            return new BankTransactionInfo(bankTransactionId, amount, BankTransactionInfoType.Refund, null);
        }

        public static BankTransactionInfo ForError(int paymentCardId, int bankTransactionId, decimal amount, string comment)
        {
            return new BankTransactionInfo(bankTransactionId, amount, BankTransactionInfoType.Error, comment)
            {
                PaymentCardId = paymentCardId
            };
        }

        public static BankTransactionInfo ForRefundError(int bankTransactionId, decimal amount, string comment)
        {
            return new BankTransactionInfo(bankTransactionId, amount, BankTransactionInfoType.Error, comment);
        }

        public PaymentCard PaymentCard { get; private set; }

        public int PosOperationId { get; private set; }
        public int PosId { get; private set; }
        public int BankTransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DateCreated { get; private set; }
        public BankTransactionInfoType Type { get; private set; }
        public string Comment { get; private set; }
        public int? PaymentCardId { get; private set; }

        protected BankTransactionInfo()
        {
            // required for EF
        }

        private BankTransactionInfo(int bankTransactionId, decimal amount, BankTransactionInfoType type, string comment)
        {
            BankTransactionId = bankTransactionId;
            Amount = amount;
            Type = type;
            Comment = comment;
            DateCreated = DateTime.UtcNow;
        }
    }
}