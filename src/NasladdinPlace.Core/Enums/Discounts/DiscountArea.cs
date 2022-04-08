using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums.Discounts
{
    public enum DiscountArea
    {
        [EnumDescription("Чек")]
        Check = 0,

        [EnumDescription("Позиция")]
        Good = 1
    }
}
