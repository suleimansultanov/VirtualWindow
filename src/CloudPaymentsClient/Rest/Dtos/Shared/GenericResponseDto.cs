using System.Runtime.Serialization;

namespace CloudPaymentsClient.Rest.Dtos.Shared
{
    [DataContract]
    public class GenericResponseDto<T> : ResponseDto
    {
        [DataMember(Name = "model")]
        public T Model { get; set; }
    }
}