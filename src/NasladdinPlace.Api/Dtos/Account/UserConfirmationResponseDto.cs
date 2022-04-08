using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.Account
{
    public class UserConfirmationResponseDto
    {
        [Required]
        public string Password { get; set; }

        public UserRegistrationStatus UserPreviousRegistrationStatus { get; set; }
    }
}
