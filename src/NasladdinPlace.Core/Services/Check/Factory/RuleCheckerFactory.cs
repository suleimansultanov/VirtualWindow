using System;
using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Services.Check.Discounts.Rules;

namespace NasladdinPlace.Core.Services.Check.Factory
{
    public static class RuleCheckerFactory
    {
        public static IRuleChecker Create(DiscountRuleType discountRuleType)
        {
            switch (discountRuleType)
            {
                case DiscountRuleType.ExpirationDate:
                    return new ExpirationDateRuleChecker();
                case DiscountRuleType.PurchaseStartDate:
                    return new PurchaseStartDateRuleChecker();
                default:
                    throw new NotSupportedException(
                        $"Unable to find the specified {nameof(DiscountRuleType)} {discountRuleType}."
                    );
            }
        }
    }
}