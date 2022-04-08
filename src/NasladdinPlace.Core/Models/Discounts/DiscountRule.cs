using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums.Discounts;

namespace NasladdinPlace.Core.Models.Discounts
{
    public class DiscountRule : Entity
    {
        public int DiscountId { get; private set; }

        public DiscountRuleArea RuleArea { get; private set; }

        public DiscountRuleType RuleType { get; private set; }

        public ICollection<DiscountRuleValue> DiscountRuleValues { get; set; }

        public Discount Discount { get; set; }

        protected DiscountRule() { }

        public DiscountRule(int discountId,
                            DiscountRuleArea area,
                            DiscountRuleType ruleType)
        {
            DiscountId = discountId;
            RuleArea = area;
            RuleType = ruleType;
            DiscountRuleValues = new Collection<DiscountRuleValue>();
        }
    }
}
