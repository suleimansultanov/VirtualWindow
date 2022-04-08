namespace NasladdinPlace.Payment.Models
{
    public class Info3Ds
    {
        public string PaReq { get; }
        public string AcsUrl { get; }
        public int TransactionId { get; }

        public Info3Ds(string paReq, string acsUrl, int transactionId)
        {
            PaReq = paReq;
            AcsUrl = acsUrl;
            TransactionId = transactionId;
        }
    }
}