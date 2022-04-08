namespace NasladdinPlace.Api.Dtos.Payment
{
    public class Info3DsDto
    {
        public int TransactionId { get; private set; }
        public string AcsUrl { get; private set; }
        public string PaReq { get; private set; }

        private Info3DsDto()
        {
            // intentionally left empty
        }
        
        public Info3DsDto(int transactionId, string acsUrl, string paReq)
        {
            TransactionId = transactionId;
            AcsUrl = acsUrl;
            PaReq = paReq;
        }
    }
}