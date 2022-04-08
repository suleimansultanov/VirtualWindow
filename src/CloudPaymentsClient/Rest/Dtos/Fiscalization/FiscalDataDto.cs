using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    public class FiscalDataDto
    {
        [DataMember(Name = "inn")]
        public string Inn { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "customerReceipt")]
        public CustomerReceiptDto CustomerReceipt { get; set; }

        public FiscalDataDto()
        {
            // intentionally left empty
        }

        public FiscalDataDto(string inn, string type, CustomerReceiptDto customerReceipt)
        {
            Inn = inn;
            Type = type;
            CustomerReceipt = customerReceipt;
        }
    }
}
