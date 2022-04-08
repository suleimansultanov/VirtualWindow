using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    [DataContract]
    public class AmountsDto
    {
        [DataMember(Name = "electronic")]
        public decimal Electronic { get; set; }
        
        [DataMember(Name = "cash")]
        public decimal Cash { get; set; }

        [DataMember(Name = "advancePayment")]
        public decimal AdvancePayment { get; set; }

        [DataMember(Name = "credit")]
        public decimal Credit { get; set; }

        [DataMember(Name = "provision")]
        public decimal Provision { get; set; }
       
        [DataMember(Name = "sum")]
        public decimal Sum { get; set; }

        public AmountsDto()
        {
            // intentionally left empty
        }

        public AmountsDto (
            decimal electronic,
            decimal advancePayment,
            decimal credit, 
            decimal provision)
        {
            Electronic = electronic;
            AdvancePayment = advancePayment;
            Credit = credit;
            Provision = provision;
        }
    }
}
