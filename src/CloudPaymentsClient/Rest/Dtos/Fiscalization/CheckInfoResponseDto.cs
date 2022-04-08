using NasladdinPlace.CloudPaymentsCore;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    [DataContract]
    public class CheckInfoResponseDto
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }
        [DataMember(Name = "phone")]
        public string Phone { get; set; }
        [DataMember(Name = "items")]
        public List<FiscalItemDto> Items { get; set; }
        [DataMember(Name = "taxationSystem")]
        public TaxationSystem TaxationSystem { get; set; }
        [DataMember(Name = "amounts")]
        public AmountsDto Amounts { get; set; }
        [DataMember(Name = "isBso")]
        public bool IsBso { get; set; }
        [DataMember(Name = "additionalData")]
        public AdditionalDataDto AdditionalData { get; set; }
    }
}
