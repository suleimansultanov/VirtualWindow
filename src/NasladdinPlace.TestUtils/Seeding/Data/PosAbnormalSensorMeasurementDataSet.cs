using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class PosAbnormalSensorMeasurementDataSet : DataSet<PosAbnormalSensorMeasurement>
    {
        private readonly int _posId;

        public PosAbnormalSensorMeasurementDataSet(int posId)
        {
            _posId = posId;
        }

        protected override PosAbnormalSensorMeasurement[] Data => new[]
        {
            new PosAbnormalSensorMeasurement(_posId, 
                PosSensorType.Temperature,
                0,
                SensorMeasurementUnit.Celsius,
                SensorPosition.InsidePos),
            new PosAbnormalSensorMeasurement(_posId, 
                PosSensorType.Humidity,
                0, 
                SensorMeasurementUnit.Celsius,
                SensorPosition.InsidePos)
        };
    }
}