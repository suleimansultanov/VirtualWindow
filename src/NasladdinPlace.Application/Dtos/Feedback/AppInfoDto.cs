using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Application.Dtos.Feedback
{
    public class AppInfoDto
    {
        [Required]
        [StringLength(50)]
        public string AppVersion { get; set; }
    }
}