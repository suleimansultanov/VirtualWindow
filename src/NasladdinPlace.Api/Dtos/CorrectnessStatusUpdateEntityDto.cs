using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Dtos
{
    public class CorrectnessStatusUpdateEntityDto
    {
        [Required]
        public int? Value { get; set; }
    }
}
