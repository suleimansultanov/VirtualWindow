using System;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Models;

namespace NasladdinPlace.Core.Services.Pos.OperationsManager
{
    public class OperationPaymentInfo
    {
        public static OperationPaymentInfo FromCheckPaymentResult(int userId, CheckPaymentResult checkPaymentResult)
        {
            if (checkPaymentResult == null)
                throw new ArgumentNullException(nameof(checkPaymentResult));

            if (!checkPaymentResult.TransactionId.HasValue || !checkPaymentResult.PaymentCardId.HasValue)
                return ForPaymentViaBonuses(userId, checkPaymentResult.PaymentInfo.CheckCostInBonuses);
            
            var bankTransactionSummary = new BankTransactionSummary(
                checkPaymentResult.PaymentCardId.Value,
                checkPaymentResult.TransactionId.Value,
                checkPaymentResult.PaymentInfo.CheckCostInMoney
            );
            return ForMixPayment(userId, bankTransactionSummary, checkPaymentResult.PaymentInfo.CheckCostInBonuses);
        }
        
        public static OperationPaymentInfo ForMixPayment(
            int userId, BankTransactionSummary bankTransactionSummary, decimal bonusAmount)
        {
            if (bankTransactionSummary == null)
                throw new ArgumentNullException(nameof(bankTransactionSummary));
            
            return new OperationPaymentInfo(userId)
            {
                BankTransactionSummary = bankTransactionSummary,
                BonusAmount = bonusAmount
            };
        }

        public static OperationPaymentInfo ForNoPayment(int userId)
        {
            return new OperationPaymentInfo(userId);
        }

        public static OperationPaymentInfo ForPaymentViaMoney(int userId, BankTransactionSummary bankTransactionSummary)
        {
            if (bankTransactionSummary == null)
                throw new ArgumentNullException(nameof(bankTransactionSummary));

            return new OperationPaymentInfo(userId)
            {
                BankTransactionSummary = bankTransactionSummary
            };
        }

        public static OperationPaymentInfo ForPaymentViaBonuses(int userId, decimal bonusAmount)
        {
            return new OperationPaymentInfo(userId)
            {
                BonusAmount = bonusAmount
            };
        }
        
        public int UserId { get; }

        public BankTransactionSummary BankTransactionSummary { get; private set; }
        public decimal BonusAmount { get; private set; }

        private OperationPaymentInfo(int userId)
        {
            UserId = userId;
            BonusAmount = 0.0M;
        }

        public bool HasBankTransactionSummary => BankTransactionSummary != null;
    }
}