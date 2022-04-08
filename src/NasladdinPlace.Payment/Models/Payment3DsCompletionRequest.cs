namespace NasladdinPlace.Payment.Models
{
    public class Payment3DsCompletionRequest
    {
        public int TransactionId { get; }
        
        public string PaRes { get; }

        public Payment3DsCompletionRequest(int transactionId, string paRes)
        {
            TransactionId = transactionId;
            PaRes = paRes;
        }
    }
}