using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    [DataContract]
    public class ReceiptResultDto
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }
    }
}
