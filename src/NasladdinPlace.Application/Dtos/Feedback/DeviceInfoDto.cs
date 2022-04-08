using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Application.Dtos.Feedback
{
    public class DeviceInfoDto
    {
        [Required]
        [MaxLength(500)]
        public string DeviceName { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string OperatingSystem { get; set; }
    }
}