using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    [DataContract]
    public class FiscalizationResponseDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "errorCode")]
        public int ErrorCode { get; set; }
    }
}
