using NasladdinPlace.Core.Enums.Discounts;

namespace NasladdinPlace.Core.Models.Discounts
{
    public class DiscountRuleValue : Entity
    {
        public int DiscountRuleId { get; private set; }

        public DiscountRuleValueType ValueType { get; private set; }

        public string Value { get; private set; }

        public DiscountRule DiscountRule { get; set; }

        protected DiscountRuleValue() { }

        public DiscountRuleValue(int discountRuleId,
                                 DiscountRuleValueType valueType,
                                 object value)
        {
            DiscountRuleId = discountRuleId;
            ValueType = valueType;
            Value = value.ToString();
        }
    }
}
