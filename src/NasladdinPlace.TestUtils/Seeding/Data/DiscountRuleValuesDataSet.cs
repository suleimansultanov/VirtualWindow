using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class DiscountRuleValuesDataSet : DataSet<DiscountRuleValue>
    {
        protected override DiscountRuleValue[] Data => new[]
        {
            new DiscountRuleValue(
                discountRuleId: 1,
                valueType: DiscountRuleValueType.MinValue,
                value: "06:00:00"
            ),
            new DiscountRuleValue(
                discountRuleId: 1,
                valueType: DiscountRuleValueType.MaxValue,
                value: "06:00:00"
            )
        };
    }
}
