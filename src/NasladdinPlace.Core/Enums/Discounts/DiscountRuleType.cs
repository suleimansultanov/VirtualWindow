using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums.Discounts
{
    public enum DiscountRuleType
    {
        [EnumDescription("Время начала покупки")]
        PurchaseStartDate = 0,

        [EnumDescription("С окончанием срока годности")]
        ExpirationDate = 1
    }
}
