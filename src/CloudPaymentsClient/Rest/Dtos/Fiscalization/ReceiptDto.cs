using System;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    [Serializable]
    public class ReceiptDto
    {
        public string Id { get; set; }
        public string DocumentNumber { get; set; }
        public string SessionNumber { get; set; }
        public string Number { get; set; }
        public string FiscalSign { get; set; }
        public string DeviceNumber { get; set; }
        public string RegNumber { get; set; }
        public string FiscalNumber { get; set; }
        public string Inn { get; set; }
        public string Type { get; set; }
        public string Ofd { get; set; }
        public string Url { get; set; }
        public string QrCodeUrl { get; set; }
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }
        public string InvoiceId { get; set; }
        public string AccountId { get; set; }
        public CustomerReceiptDto Receipt { get; set; }
        public string RecCalculationPlaceeipt { get; set; }
        public string CashierName { get; set; }
        public string SettlePlace { get; set; }
        public string DocumentInfo { get; set; }
    }
}
