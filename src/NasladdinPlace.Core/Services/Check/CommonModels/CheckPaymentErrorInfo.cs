using System;

namespace NasladdinPlace.Core.Services.Check.CommonModels
{
    public sealed class CheckPaymentErrorInfo
    {
        public CheckPaymentErrorInfo(string errorMessage, DateTime paymentDateTime, int? paymentCardId)
        {
            Message = errorMessage;
            PaymentDate = paymentDateTime;
            PaymentCardId = paymentCardId;
        }

        public static readonly CheckPaymentErrorInfo Empty = null;

        public string Message { get; }

        public DateTime PaymentDate { get; }

        public int? PaymentCardId { get; }
    }
}