using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Payment.Refund
{
    [DataContract]
    public class RefundRequestDto
    {
        [DataMember(Name = "transactionId")]
        public int TransactionId { get; }
        
        [DataMember(Name = "amount")]
        public decimal Amount { get; }

        public PaymentExtraParamsDto ExtraParams { get; set; }

        public RefundRequestDto(int transactionId, decimal amount)
        {
            TransactionId = transactionId;
            Amount = amount;
        }
    }
}