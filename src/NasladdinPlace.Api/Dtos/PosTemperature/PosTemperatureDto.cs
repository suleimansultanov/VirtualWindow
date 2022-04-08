using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Api.Dtos.Pos;

namespace NasladdinPlace.Api.Dtos.PosTemperature
{
    public class PosTemperatureDto : BasePosWsMessageDto
    {   
        [Required]
        public double? Value { get; set; }
    }
}