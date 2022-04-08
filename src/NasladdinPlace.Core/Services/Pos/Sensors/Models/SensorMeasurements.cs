using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Pos.Sensors.Models
{
    public class SensorMeasurements
    {
        public SensorPosition SensorPosition { get; }
        public double Temperature { get; }
        public double Humidity { get; }
        public double Amperage { get; }
        public FrontPanelPosition FrontPanelPosition { get; }

        public SensorMeasurements(SensorPosition sensorPosition, double temperature, double humidity)
        {
            SensorPosition = sensorPosition;
            Temperature = Round(temperature);
            Humidity = Round(humidity);
        }

        public SensorMeasurements(SensorPosition sensorPosition, double temperature, double humidity, double amperage, FrontPanelPosition frontPanelPosition)
        {
            SensorPosition = sensorPosition;
            Temperature = Round(temperature);
            Humidity = Round(humidity);
            Amperage = Round(amperage);
            FrontPanelPosition = frontPanelPosition;
        }

        public static SensorMeasurements EmptyOfPosition(SensorPosition position)
        {
            return new SensorMeasurements(position, double.NaN, double.NaN, double.NaN, FrontPanelPosition.NotFound);
        }

        private static double Round(double value)
        {
            return Math.Round(value, 1, MidpointRounding.AwayFromZero);
        }
    }
}