using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.Check
{
    [DataContract]
    public class CurrencyDto
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        
        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "isoCode")]
        public string IsoCode { get; set; }
    }
}