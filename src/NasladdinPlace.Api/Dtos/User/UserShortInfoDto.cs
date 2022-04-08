using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.User
{
    public class UserShortInfoDto
    {
        public int Id { get; set; }
        
        public string UserName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }
    }
}
