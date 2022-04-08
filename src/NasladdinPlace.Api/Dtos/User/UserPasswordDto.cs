using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.User
{
    public class UserPasswordDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
