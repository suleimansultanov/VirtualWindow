using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums.Discounts
{
    public enum DiscountRuleArea
    {
        [EnumDescription("Чек")]
        Check = 0,

        [EnumDescription("Экземпляр Товара")]
        LabeledGood = 1
    }
}
