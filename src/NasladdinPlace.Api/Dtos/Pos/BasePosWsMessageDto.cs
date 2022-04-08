using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Pos
{
    public class BasePosWsMessageDto
    {
        [Required]
        public int? PosId { get; set; }
    }
}