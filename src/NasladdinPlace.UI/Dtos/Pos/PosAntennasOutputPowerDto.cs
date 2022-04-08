using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.Dtos.Pos
{
    public class PosAntennasOutputPowerDto
    {
        [Required]
        public byte? OutputPower { get; set; }
    }
}
