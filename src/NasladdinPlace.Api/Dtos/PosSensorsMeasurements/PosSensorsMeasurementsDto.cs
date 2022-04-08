using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Api.Dtos.Pos;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.Api.Dtos.PosSensorsMeasurements
{
    public class PosSensorsMeasurementsDto : BasePosWsMessageDto
    {
        [Required]
        public ICollection<SensorMeasurementsDto> SensorsMeasurements { get; }

        public PosSensorsMeasurementsDto()
        {
            SensorsMeasurements = new Collection<SensorMeasurementsDto>();
        }
    }
}