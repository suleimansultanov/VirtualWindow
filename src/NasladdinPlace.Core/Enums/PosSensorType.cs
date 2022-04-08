using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum PosSensorType
    {
        [EnumDescription("Датчик температуры")]
        Temperature = 0,

        [EnumDescription("Датчик влажности")]
        Humidity = 1
    }
}