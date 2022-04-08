using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos.Country
{
    public class CountryDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }
}
