using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum PosScreenType
    {
        [EnumDescription("Изображение основного экрана")]
        MainBackground,

        [EnumDescription("Изображение в режиме 'Витрина недоступна'")]
        DisabledBackground
    }
}
