using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Utils;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;

namespace NasladdinPlace.UI.ViewModels.Discounts
{
    public class DiscountRuleValueViewModel
    {
        [Display(Name = "Время От")]
        [Render(Control = RenderControl.TimeSpan)]
        public TimeSpan TimeFrom { get; set; }

        [Display(Name = "Время По")]
        [Render(Control = RenderControl.TimeSpan)]
        public TimeSpan TimeTo { get; set; }

        [Display(Name = "До окончания срока годности (в часах)")]
        [Render(Control = RenderControl.Integer)]
        public int HoursBeforeExpirationDate { get; set; }

        protected bool Equals(DiscountRuleValueViewModel other)
        {
            return TimeFrom.Equals(other.TimeFrom)
                   && TimeTo.Equals(other.TimeTo)
                   && HoursBeforeExpirationDate == other.HoursBeforeExpirationDate;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiscountRuleValueViewModel)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var salt = HashHelper.Salt;
                var hashCode = TimeFrom.GetHashCode();
                hashCode = (hashCode * salt) ^ TimeTo.GetHashCode();
                hashCode = (hashCode * salt) ^ HoursBeforeExpirationDate;
                return hashCode;
            }
        }
    }
}
