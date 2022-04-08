using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.PosOperation
{
    public class PosOperationModeDto
    {
        [Required]
        public PosMode? Mode { get; set; }
    }
}
