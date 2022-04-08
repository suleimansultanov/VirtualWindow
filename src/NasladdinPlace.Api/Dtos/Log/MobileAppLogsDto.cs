using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Log
{
    public class MobileAppLogsDto
    {
        [Required]
        public string UserPhoneNumber { get; set; }
        
        [Required]
        public string Content { get; set; }
    }
}