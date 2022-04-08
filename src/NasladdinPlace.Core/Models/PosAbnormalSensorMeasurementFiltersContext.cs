using System;

namespace NasladdinPlace.Core.Models
{
    public class PosAbnormalSensorMeasurementFiltersContext
    {
        public int? PosId { get; set; }
        public DateTime? PosAbnormalSensorMeasurementDateFrom { get; set; }
        public DateTime? PosAbnormalSensorMeasurementDateUntil { get; set; }
    }
}