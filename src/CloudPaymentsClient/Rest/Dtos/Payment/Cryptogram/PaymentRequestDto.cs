using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Payment.Cryptogram
{
    [DataContract]
    public class PaymentRequestDto
    {
        [DataMember(Name = "amount")]
        public decimal Amount { get; }

        [DataMember(Name = "currency")]
        public string Currency { get; }

        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; }

        [DataMember(Name = "name")]
        public string CardHolderName { get; }

        [DataMember(Name = "cardCryptogramPacket")]
        public string CardCryptogramPacket { get; }

        #region Optional parameters

        [DataMember(Name = "invoiceId")]
        public string InvoceId { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "accountId")]
        public string AccountId { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "jsonData")]
        public PaymentExtraParamsDto ExtraParams { get; set; }

        #endregion

        public PaymentRequestDto(
            decimal amount,
            string currency,
            string ipAddress,
            string cardHolderName,
            string cardCryptogramPacket)
        {
            Amount = amount;
            Currency = currency;
            IpAddress = ipAddress;
            CardHolderName = cardHolderName;
            CardCryptogramPacket = cardCryptogramPacket;
        }
    }
}