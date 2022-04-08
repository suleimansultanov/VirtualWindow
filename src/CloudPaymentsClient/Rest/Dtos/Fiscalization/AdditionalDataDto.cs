using System;
using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    [DataContract]
    public class AdditionalDataDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "accountId")]
        public string AccountId { get; set; }
        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }
        [DataMember(Name = "calculationPlace")]
        public string CalculationPlace { get; set; }
        [DataMember(Name = "cashierName")]
        public string CashierName { get; set; }
        [DataMember(Name = "dateTime")]
        public DateTime DateTime { get; set; }
        [DataMember(Name = "deviceNumber")]
        public string DeviceNumber { get; set; }
        [DataMember(Name = "documentNumber")]
        public string DocumentNumber { get; set; }
        [DataMember(Name = "fiscalNumber")]
        public string FiscalNumber { get; set; }
        [DataMember(Name = "fiscalSign")]
        public string FiscalSign { get; set; }
        [DataMember(Name = "invoiceId")]
        public string InvoiceId { get; set; }
        [DataMember(Name = "ofd")]
        public string Ofd { get; set; }
        [DataMember(Name = "ofdReceiptUrl")]
        public string OfdReceiptUrl { get; set; }
        [DataMember(Name = "organizationInn")]
        public string OrganizationInn { get; set; }
        [DataMember(Name = "qrCodeUrl")]
        public string QrCodeUrl { get; set; }
        [DataMember(Name = "regNumber")]
        public string RegNumber { get; set; }
        [DataMember(Name = "senderEmail")]
        public string SenderEmail { get; set; }
        [DataMember(Name = "sessionCheckNumber")]
        public string SessionCheckNumber { get; set; }
        [DataMember(Name = "sessionNumber")]
        public string SessionNumber { get; set; }
        [DataMember(Name = "settlePlace")]
        public string SettlePlace { get; set; }
        [DataMember(Name = "transactionId")]
        public string TransactionId { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
