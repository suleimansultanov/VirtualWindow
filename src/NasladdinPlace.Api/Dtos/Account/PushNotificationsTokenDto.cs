using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Account
{
    public class PushNotificationsTokenDto
    {
        [Required]
        public string Token { get; set; }
    }
}