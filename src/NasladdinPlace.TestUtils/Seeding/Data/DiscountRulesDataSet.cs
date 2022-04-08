using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class DiscountRulesDataSet : DataSet<DiscountRule>
    {
        protected override DiscountRule[] Data => new[]
        {
            new DiscountRule(
                discountId: 1,
                area: DiscountRuleArea.Check,
                ruleType: DiscountRuleType.PurchaseStartDate
            )
        };
    }
}
