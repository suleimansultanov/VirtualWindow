using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NasladdinPlace.Api.Dtos.PosAntennasOutputPower
{
    [DataContract]
    public class PosAntennasOutputPowerDto 
    {
        [DataMember(Name = "posId")]
        public int? PosId { get; set; }
        
        [Required]
        [DataMember(Name = "outputPower")]
        public Core.Enums.PosAntennasOutputPower? OutputPower { get; set; }
    }
}
