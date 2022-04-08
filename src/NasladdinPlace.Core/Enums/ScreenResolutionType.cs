using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum ScreenResolutionType
    {
        [EnumDescription("1366X768")]
        R1366X768 = 0,
        [EnumDescription("1680X1050")]
        R1680X1050 = 1,
        [EnumDescription("1280X800")]
        R1280X800 = 2,
        [EnumDescription("976X610")]
        R976X610 = 3,
        [EnumDescription("1344X840")]
        R1344X840 = 4
    }
}