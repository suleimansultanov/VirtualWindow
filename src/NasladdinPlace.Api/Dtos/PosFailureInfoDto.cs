using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Api.Dtos.Pos;

namespace NasladdinPlace.Api.Dtos
{
    public class PosFailureInfoDto : BasePosWsMessageDto
    {
        [Required]
        public string Cause { get; set; }

        [Required]
        public string Source { get; set; }
    }
}