using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos
{
    public class LocationDto
    {
        [Required]
        public double? Latitude { get; set; }

        [Required]
        public double? Longitude { get; set; }
    }
}
