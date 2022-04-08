using System;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Payment.Models
{
    public class CheckPaymentResult
    {
        public static CheckPaymentResult NoPaymentRequired()
        {
            return new CheckPaymentResult(
                CheckPaymentStatus.NoPaymentRequired, 
                transactionId: null, 
                paymentCardId: null, 
                paymentInfo: CheckPaymentInfo.Zero, 
                error: Error.Empty
            );
        }

        public static CheckPaymentResult Paid(int transactionId, int paymentCardId, CheckPaymentInfo checkPaymentInfo)
        {
            if (checkPaymentInfo == null)
                throw new ArgumentNullException(nameof(checkPaymentInfo));
            
            return new CheckPaymentResult(
                CheckPaymentStatus.Paid, 
                transactionId, 
                paymentCardId, 
                checkPaymentInfo, 
                error: Error.Empty
            );
        }

        public static CheckPaymentResult PaidViaBonuses(CheckPaymentInfo checkPaymentInfo)
        {
            if (checkPaymentInfo == null)
                throw new ArgumentNullException(nameof(checkPaymentInfo));

            return new CheckPaymentResult(
                CheckPaymentStatus.Paid,
                transactionId: null,
                paymentCardId: null,
                paymentInfo: checkPaymentInfo,
                error: Error.Empty
            );
        }

        public static CheckPaymentResult Failure(Error error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));
            
            return new CheckPaymentResult(
                CheckPaymentStatus.Error,
                transactionId: null,
                paymentCardId: null,
                paymentInfo: null,
                error: error
            );
        }

        public static CheckPaymentResult FailureWithBankRequisites(
            Error error, 
            int? transactionId,
            int? paymentCardId,
            CheckPaymentInfo checkPaymentInfo,
            string bankError)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            return new CheckPaymentResult(
                CheckPaymentStatus.Error,
                transactionId: transactionId,
                paymentCardId: paymentCardId,
                paymentInfo: checkPaymentInfo,
                error: error
            )
            {
                BankError = bankError
            };
        }

        public CheckPaymentStatus PaymentStatus { get;}
        public int? TransactionId { get; }
        public int? PaymentCardId { get; }
        public CheckPaymentInfo PaymentInfo { get; }
        public Error Error { get; }
        public string BankError { get; private set; }

        public bool HasBankRequisites()
        {
            return PaymentInfo != null 
                   && TransactionId.HasValue 
                   && PaymentCardId.HasValue;
        }

        private CheckPaymentResult(
            CheckPaymentStatus paymentStatus, 
            int? transactionId, 
            int? paymentCardId,
            CheckPaymentInfo paymentInfo, 
            Error error)
        {
            PaymentStatus = paymentStatus;
            TransactionId = transactionId;
            PaymentCardId = paymentCardId;
            PaymentInfo = paymentInfo;
            Error = error;
        }
    }
}