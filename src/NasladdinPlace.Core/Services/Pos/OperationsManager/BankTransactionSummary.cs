namespace NasladdinPlace.Core.Services.Pos.OperationsManager
{
    public class BankTransactionSummary
    {
        public int BankTransactionId { get; }
        public decimal Amount { get; }
        public int PaymentCardId { get; }

        public BankTransactionSummary(int paymentCardId, int bankTransactionId, decimal amount)
        {
            PaymentCardId = paymentCardId;
            BankTransactionId = bankTransactionId;
            Amount = amount;
        }
    }
}