using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.UI.Dtos.PosOperation
{
    public class PosOperationModeDto
    {
        [Required]
        public PosMode? Mode { get; set; }
    }
}
