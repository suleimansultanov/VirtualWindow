namespace NasladdinPlace.Payment.Models
{
    public class RefundRequest
    {
        public int TransactionId { get; }
        public decimal Amount { get; }

        public RefundRequest(int transactionId, decimal amount)
        {
            TransactionId = transactionId;
            Amount = amount;
        }
    }
}