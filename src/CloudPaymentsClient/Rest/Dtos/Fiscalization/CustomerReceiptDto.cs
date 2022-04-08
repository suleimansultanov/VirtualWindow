using NasladdinPlace.CloudPaymentsCore;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    [DataContract]
    public class CustomerReceiptDto
    {
        [DataMember(Name = "items")]
        public List<FiscalItemDto> Items { get; set; }

        [DataMember(Name = "calculationPlace")]
        public string CalculationPlace { get; set; }

        [DataMember(Name = "taxationSystem")]
        public TaxationSystem TaxationSystem { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        [DataMember(Name = "customerInfo")]
        public string CustomerInfo { get; set; }

        [DataMember(Name = "customerInn")]
        public string CustomerInn { get; set; }

        [DataMember(Name = "isBso")]
        public bool IsBso { get; set; }

        [DataMember(Name = "agentSign")]
        public int? AgentSign { get; set; }

        [DataMember(Name = "amounts")]
        public AmountsDto Amounts { get; set; }

        [DataMember(Name = "cashierName")]
        public string CashierName { get; set; }

        public CustomerReceiptDto()
        {
            //intentionally left empty
        }

        public CustomerReceiptDto(
            List<FiscalItemDto> fiscalItems,
            TaxationSystem taxationSystem,
            AmountsDto amounts)
        {
            Items = fiscalItems;
            TaxationSystem = taxationSystem;
            Amounts = amounts;
        }
    }
}
