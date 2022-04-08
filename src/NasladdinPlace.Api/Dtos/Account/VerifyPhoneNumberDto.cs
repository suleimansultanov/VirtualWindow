using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Account
{
    public class VerifyPhoneNumberDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
