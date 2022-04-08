using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Api.Dtos.Country;

namespace NasladdinPlace.Api.Dtos.City
{
    public class CityDto
    {
        public CountryDto Country { get; set; }

        public int Id { get; set; }

        [Required]
        public int? CountryId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }
}
