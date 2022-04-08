namespace NasladdinPlace.Api.ViewModels
{
    public class Form3DsViewModel
    {
        public string AcsUrl { get; }
        public string PaReq { get; }
        public int TransactionId { get; }
        public string TermUrl { get; }

        public Form3DsViewModel(string acsUrl, string paReq, int transactionId, string termUrl)
        {
            AcsUrl = acsUrl;
            PaReq = paReq;
            TransactionId = transactionId;
            TermUrl = termUrl;
        }
    }
}