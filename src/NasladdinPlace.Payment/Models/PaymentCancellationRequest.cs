namespace NasladdinPlace.Payment.Models
{
    public class PaymentCancellationRequest
    {
        public int TransactionId { get; }

        public PaymentCancellationRequest(int transactionId)
        {
            TransactionId = transactionId;
        }
    }
}