using NasladdinPlace.Application.Dtos.Feedback;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Errors
{
    public class MobileAppErrorDto
    {
        [Required]
        public string Error { get; set; }

        [Required]
        public AppInfoDto AppInfo { get; set; }

        [Required]
        public DeviceInfoDto DeviceInfo { get; set; }

        public string PhoneNumber { get; set; }

        public string ErrorSource { get; set; }
    }
}