using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum SensorPosition
    {
        [EnumDescription("Внутри")]
        InsidePos = 0,

        [EnumDescription("Снаружи")]
        OutsidePos = 1,

        [EnumDescription("Инструментарий")]
        Toolbox = 2,
        
        [EnumDescription("Испаритель")]
        Evaporator = 3,

        [EnumDescription("Влажность внутри витрины")]
        HumidityInsidePos = 4,

        [EnumDescription("Неопределенно")]
        Unspecified = 5
    }
}