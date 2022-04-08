using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Dtos
{
    public class SensorMeasurementsDto
    {
        [Required] 
        public int? SensorId { get; set; }

        [Required]
        public double? Temperature { get; set; }

        [Required]
        public double? Humidity { get; set; }

        public double? Amperage { get; set; }

        public int? FrontPanelPosition { get; set; }
    }
}