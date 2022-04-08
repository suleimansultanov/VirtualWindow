using NasladdinPlace.Core.Enums;
using System;

namespace NasladdinPlace.Core.Models
{
    public class PosAbnormalSensorMeasurement : Entity
    {
        public Pos Pos { get; private set; }

        public int PosId { get; private set; }
        public PosSensorType Type { get; private set; }
        public double MeasurementValue { get; private set; }
        public SensorMeasurementUnit MeasurementUnit { get; private set; }
        public DateTime DateMeasured { get; private set; }
        public SensorPosition SensorPosition { get; private set; }

        protected PosAbnormalSensorMeasurement()
        {
            DateMeasured = DateTime.UtcNow;
        }

        public PosAbnormalSensorMeasurement(
            int posId,
            PosSensorType type,
            double measurementValue,
            SensorMeasurementUnit measurementUnit,
            SensorPosition sensorPosition) : this()
        {
            PosId = posId;
            Type = type;
            MeasurementValue = measurementValue;
            MeasurementUnit = measurementUnit;
            SensorPosition = sensorPosition;
        }
    }
}