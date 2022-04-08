using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Account
{
    public class UserFormDto
    {
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string FullName { get; set; }

        [Range(0, 2)]
        public int? Gender { get; set; }

        [Range(0, 2)]
        public int? Goal { get; set; }

        [Range(0, 4)]
        public int? Activity { get; set; }

        [Range(0, 4)]
        public int? Pregnancy { get; set; }
        public int? Age { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public long? BirthDateMillis { get; set; }
    }
}