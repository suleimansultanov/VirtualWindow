using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum PosActivityStatus
    {
        [EnumDescription("Выведена из эксплуатации")]
        Inactive = 0,

        [EnumDescription("В эксплуатации")]
        Active = 1,

        [EnumDescription("Пуско-наладочный режим")]
        Test = 2
    }
}