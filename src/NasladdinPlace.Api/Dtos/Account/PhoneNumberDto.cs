using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Account
{
    public class PhoneNumberDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
