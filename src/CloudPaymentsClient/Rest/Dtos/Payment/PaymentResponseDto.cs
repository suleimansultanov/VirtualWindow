using System.Runtime.Serialization;
using CloudPaymentsClient.Rest.Dtos.Shared;

namespace CloudPaymentsClient.Rest.Dtos.Payment
{
    [DataContract]
    public class PaymentResponseDto
    {
        [DataMember(Name = "transactionId")]
        public int TransactionId { get; set; }
        
        // 3ds secure
        [DataMember(Name = "paReq")]
        public string PaReq { get; set; }
        
        [DataMember(Name = "acsUrl")]
        public string AcsUrl { get; set; }
        
        // Transaction declined
        [DataMember(Name = "amount")]
        public decimal? Amount { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "currencyCode")]
        public int CurrencyCode { get; set; }

        [DataMember(Name = "paymentAmount")]
        public decimal? PaymentAmount { get; set; }

        [DataMember(Name = "paymentCurrency")]
        public string PaymentCurrency { get; set; }

        [DataMember(Name = "paymentCurrencyCode")]
        public int? PaymentCurrencyCode { get; set; }

        [DataMember(Name = "invoiceId")]
        public string InvoiceId { get; set; }

        [DataMember(Name = "accountId")]
        public string AccountId { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "jsonData")]
        public string JsonData { get; set; }

        [DataMember(Name = "createdDate")]
        public string CreatedDate { get; set; }

        [DataMember(Name = "createdDateIso")]
        public string CreatedDateIso { get; set; }

        [DataMember(Name = "isTestMode")]
        public bool? IsTestMode { get; set; }

        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; set; }

        [DataMember(Name = "ipCountry")]
        public string IpCountry { get; set; }

        [DataMember(Name = "ipCity")]
        public string IpCity { get; set; }

        [DataMember(Name = "ipRegion")]
        public string IpRegion { get; set; }

        [DataMember(Name = "ipDistrict")]
        public string IpDistrict { get; set; }

        [DataMember(Name = "ipLatitude")]
        public float? IpLatitude { get; set; }
        
        [DataMember(Name = "ipLongitude")]
        public float? IpLongitude { get; set; }

        [DataMember(Name = "cardFirstSix")]
        public string CardFirstSix { get; set; }

        [DataMember(Name = "cardLastFour")]
        public string CardLastFour { get; set; }

        [DataMember(Name = "cardExpDate")]
        public string CardExpDate { get; set; }

        [DataMember(Name = "cardType")]
        public string CardType { get; set; }

        [DataMember(Name = "cardTypeCode")]
        public int? CardTypeCode { get; set; }

        [DataMember(Name = "issuer")]
        public string Issuer { get; set; }

        [DataMember(Name = "issuerBankCountry")]
        public string IssuerBankCountry { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "statusCode")]
        public OperationStatusCode? StatusCode { get; set; }

        [DataMember(Name = "reason")]
        public string Reason { get; set; }

        [DataMember(Name = "reasonCode")]
        public ErrorCode? ReasonCode { get; set; }

        [DataMember(Name = "cardHolderMessage")]
        public string CardHolderMessage { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        // Transaction completed
        [DataMember(Name = "authDate")]
        public string AuthDate { get; set; }

        [DataMember(Name = "authDateIso")]
        public string AuthDateIso { get; set; }

        [DataMember(Name = "authCode")]
        public string AuthCode { get; set; }

        [DataMember(Name = "token")]
        public string CardToken { get; set; }

        public PaymentResponseType ResponseType
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PaReq) && !string.IsNullOrEmpty(AcsUrl))
                    return PaymentResponseType.Required3Ds;

                switch (StatusCode)
                {
                        case OperationStatusCode.Completed:
                            return PaymentResponseType.PaymentCompleted;
                        case OperationStatusCode.Authorized:
                            return PaymentResponseType.PaymentAuthorized;
                        default:
                            return PaymentResponseType.Error;
                }
            }
        }

    }
}