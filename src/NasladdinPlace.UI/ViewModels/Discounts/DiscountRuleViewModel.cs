using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Utils;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;

namespace NasladdinPlace.UI.ViewModels.Discounts
{
    public class DiscountRuleViewModel
    {
        [Display(Name = "Область действия условия")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(DiscountRuleArea))]
        public int DiscountRuleArea { get; set; }

        [Display(Name = "Критерий")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(DiscountRuleType))]
        public int DiscountRuleType { get; set; }

        public DiscountRuleValueViewModel RuleValue { get; set; }

        protected bool Equals(DiscountRuleViewModel other)
        {
            return DiscountRuleArea == other.DiscountRuleArea
                   && DiscountRuleType == other.DiscountRuleType
                   && Equals(RuleValue, other.RuleValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiscountRuleViewModel)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var salt = HashHelper.Salt;
                var hashCode = DiscountRuleArea;
                hashCode = (hashCode * salt) ^ DiscountRuleType;
                hashCode = (hashCode * salt) ^ (RuleValue != null ? RuleValue.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
