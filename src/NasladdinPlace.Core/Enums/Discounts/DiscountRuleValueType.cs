using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums.Discounts
{
    public enum DiscountRuleValueType
    {
        [EnumDescription("Минимальное значение")]
        MinValue,

        [EnumDescription("Максимальное значение")]
        MaxValue, 

        [EnumDescription("Значение")]
        SingleValue
    }
}
