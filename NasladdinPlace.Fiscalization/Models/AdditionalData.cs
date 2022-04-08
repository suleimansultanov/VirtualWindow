using System;

namespace NasladdinPlace.Fiscalization.Models
{
    public class AdditionalData
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public string CalculationPlace { get; set; }
        public string CashierName { get; set; }
        public DateTime DateTime { get; set; }
        public string DeviceNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string FiscalNumber { get; set; }
        public string FiscalSign { get; set; }
        public string InvoiceId { get; set; }
        public string Ofd { get; set; }
        public string OfdReceiptUrl { get; set; }
        public string OrganizationInn { get; set; }
        public string QrCodeUrl { get; set; }
        public string RegNumber { get; set; }
        public string SenderEmail { get; set; }
        public string SessionCheckNumber { get; set; }
        public string SessionNumber { get; set; }
        public string SettlePlace { get; set; }
        public string TransactionId { get; set; }
        public string Type { get; set; }
    }
}
