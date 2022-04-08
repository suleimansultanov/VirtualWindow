using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.Check
{
    [DataContract]
    public class CheckGoodImageDto
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        
        [DataMember(Name = "imagePath")]
        public string ImagePath { get; set; }
    }
}