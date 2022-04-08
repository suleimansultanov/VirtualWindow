using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NasladdinPlace.Core.Utils;

namespace NasladdinPlace.UI.ViewModels.Discounts
{
    public class DiscountInfoViewModel
    {
        public int DiscountId { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Скидка в процентах")]
        [Render(Control = RenderControl.Decimal)]
        public decimal DiscountInPercents { get; set; }

        [Display(Name = "Область действия")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(DiscountArea))]
        public int DiscountArea { get; set; }

        [Display(Name = "Скидка активна")]
        [Render(Control = RenderControl.YesNo)]
        public bool IsEnabled { get; set; }

        public List<DiscountRuleViewModel> Rules { get; set; }

        public List<PosBasicInfoViewModel> PointsOfSale { get; set; }

        protected bool Equals(DiscountInfoViewModel other)
        {
            return DiscountId == other.DiscountId
                   && string.Equals(Name, other.Name)
                   && DiscountInPercents == other.DiscountInPercents
                   && DiscountArea == other.DiscountArea
                   && IsEnabled == other.IsEnabled
                   && !Rules.Except(other.Rules).Any() && !other.Rules.Except(Rules).Any()
                   && !PointsOfSale.Except(other.PointsOfSale).Any() && !other.PointsOfSale.Except(PointsOfSale).Any();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiscountInfoViewModel)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var salt = HashHelper.Salt;
                var hashCode = DiscountId;
                hashCode = (hashCode * salt) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * salt) ^ DiscountInPercents.GetHashCode();
                hashCode = (hashCode * salt) ^ DiscountArea;
                hashCode = (hashCode * salt) ^ IsEnabled.GetHashCode();
                hashCode = (hashCode * salt) ^ (Rules != null ? Rules.GetHashCode() : 0);
                hashCode = (hashCode * salt) ^ (PointsOfSale != null ? PointsOfSale.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
