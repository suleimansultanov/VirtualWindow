using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum Gender
    {
        [EnumDescription("Неопределенный")]
        Undefined = 0,

        [EnumDescription("Мужской")]
        Male = 1,

        [EnumDescription("Женский")]
        Female = 2
    }
}
