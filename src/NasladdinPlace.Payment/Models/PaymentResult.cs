namespace NasladdinPlace.Payment.Models
{
    public class PaymentResult
    {
        public static PaymentResult Require3Ds(int transactionId, Info3Ds info3Ds)
        {
            return new PaymentResult(transactionId, PaymentStatus.Require3Ds, info3Ds);
        }

        public static PaymentResult NotPaid(int transactionId, string reason, string localizedReason)
        {
            return new PaymentResult(transactionId, PaymentStatus.NotPaid, null)
            {
                Error = reason,
                LocalizedError = localizedReason
            };
        }

        public static PaymentResult Paid(int transactionId, PaymentCardInfo paymentCardInfo)
        {
            return new PaymentResult(transactionId, PaymentStatus.Paid, null)
            {
                PaymentCardInfo = paymentCardInfo
            };
        }
        
        public int TransactionId { get; }
        public PaymentStatus PaymentStatus { get; }
        public Info3Ds Info3Ds { get; }
        public PaymentCardInfo PaymentCardInfo { get; private set; }
        public string Error { get; private set; }
        public string LocalizedError { get; private set; }

        private PaymentResult(int transactionId, PaymentStatus paymentStatus, Info3Ds info3Ds)
        {
            TransactionId = transactionId;
            PaymentStatus = paymentStatus;
            Info3Ds = info3Ds;
            Error = string.Empty;
            LocalizedError = string.Empty;
        }
    }
}