using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Payment.Payment3DsCompletion
{
    [DataContract]
    public class Payment3DsCompletionRequestDto
    {
        [DataMember(Name = "transactionId")]
        public int TransactionId { get; }

        [DataMember(Name = "paRes")]
        public string PaRes { get; }

        public Payment3DsCompletionRequestDto(int transactionId, string paRes)
        {
            TransactionId = transactionId;
            PaRes = paRes;
        }
    }
}