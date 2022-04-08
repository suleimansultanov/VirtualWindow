using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Shared
{
    [DataContract]
    public class ResponseDto
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }
        
        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}