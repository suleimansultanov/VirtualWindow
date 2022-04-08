using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Fiscalization
{
    [DataContract]
    public class CheckInfoDto
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
    }
}
