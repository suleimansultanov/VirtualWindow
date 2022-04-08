using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum SensorControllerType
    {
        [EnumDescription("1-AM, 2-DS, 3-DS, 4-AM")]
        New = 0,
        [EnumDescription("0-AM, 1-AM, 2-DS")]
        Legacy = 1,
        [EnumDescription("ESP8266")]
        Esp = 2
    }
}