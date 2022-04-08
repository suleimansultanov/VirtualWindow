using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.Maker
{
    [DataContract]
    public class MakerDto
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        [Required]
        [StringLength(1000)]
        public string Name { get; set; }
    }
}
