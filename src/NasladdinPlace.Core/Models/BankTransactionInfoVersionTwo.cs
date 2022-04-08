using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using System;

namespace NasladdinPlace.Core.Models
{
    public class BankTransactionInfoVersionTwo : Entity
    {
        public static BankTransactionInfoVersionTwo CreateFromSummary(BankTransactionSummary bankTransactionSummary)
        {
            return ForPayment(
                bankTransactionSummary.PaymentCardId,
                bankTransactionSummary.BankTransactionId,
                bankTransactionSummary.Amount
            );
        }

        public static BankTransactionInfoVersionTwo ForPayment(int paymentCardId, int bankTransactionId, decimal amount)
        {
            return new BankTransactionInfoVersionTwo(bankTransactionId, amount, BankTransactionInfoType.Payment, null)
            {
                PaymentCardId = paymentCardId
            };
        }

        public static BankTransactionInfoVersionTwo ForRefund(int bankTransactionId, decimal amount)
        {
            return new BankTransactionInfoVersionTwo(bankTransactionId, amount, BankTransactionInfoType.Refund, null);
        }

        public static BankTransactionInfoVersionTwo ForError(int paymentCardId, int bankTransactionId, decimal amount, string comment)
        {
            return new BankTransactionInfoVersionTwo(bankTransactionId, amount, BankTransactionInfoType.Error, comment)
            {
                PaymentCardId = paymentCardId
            };
        }

        public static BankTransactionInfoVersionTwo ForRefundError(int bankTransactionId, decimal amount, string comment)
        {
            return new BankTransactionInfoVersionTwo(bankTransactionId, amount, BankTransactionInfoType.Error, comment);
        }

        public PaymentCard PaymentCard { get; private set; }
        public int BankTransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DateCreated { get; private set; }
        public BankTransactionInfoType Type { get; private set; }
        public string Comment { get; private set; }
        public int? PaymentCardId { get; private set; }
        public int PosOperationTransactionId { get; private set; }
        public PosOperationTransaction PosOperationTransaction { get; private set; }

        protected BankTransactionInfoVersionTwo()
        {
            // required for EF
        }

        private BankTransactionInfoVersionTwo(int bankTransactionId, decimal amount, BankTransactionInfoType type, string comment)
        {
            BankTransactionId = bankTransactionId;
            Amount = amount;
            Type = type;
            Comment = comment;
            DateCreated = DateTime.UtcNow;
        }
    }
}
