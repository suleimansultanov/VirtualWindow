using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Application.Dtos.Feedback
{
    public class SenderInfoDto
    {
        [Phone]
        [Required]
        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [Required]
        public DeviceInfoDto DeviceInfo { get; set; }
    }
}