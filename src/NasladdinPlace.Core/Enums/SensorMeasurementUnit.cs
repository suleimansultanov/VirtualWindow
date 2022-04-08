using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum SensorMeasurementUnit
    {
        [EnumDescription("Отсутствует")]
        Unknown = 0,

        [EnumDescription("°C")]
        Celsius = 1,

        [EnumDescription("°F")]
        Farenheit = 2,

        [EnumDescription("%")]
        Percents = 3
    }
}