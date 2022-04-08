using System.Runtime.Serialization;
using CloudPaymentsClient.Rest.Dtos.Fiscalization;

namespace CloudPaymentsClient.Rest.Dtos.Payment.Recurrent
{
    [DataContract]
    public class RecurrentPaymentRequestDto
    {
        [DataMember(Name = "amount")]
        public decimal Amount { get; }
        
        [DataMember(Name = "currency")]
        public string Currency { get; }
        
        [DataMember(Name = "accountId")]
        public string UserIdentifier { get; }
        
        [DataMember(Name = "token")]
        public string CardToken { get; }
        
        #region Optional parameters

        [DataMember(Name = "invoiceId")]
        public string InvoiceId { get; set; }
        
        [DataMember(Name = "description")]
        public string Description { get; set; }
        
        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; set; }
        
        [DataMember(Name = "email")]
        public string Email { get; set; }
        
        [DataMember(Name = "jsonData")]
        public FiscalDataDto ExtraParams { get; set; }
        
        #endregion

        public RecurrentPaymentRequestDto(decimal amount, string currency, string userIdentifier, string cardToken)
        {
            Amount = amount;
            Currency = currency;
            UserIdentifier = userIdentifier;
            CardToken = cardToken;
        }
    }
}