using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.Discounts
{
    public class DiscountViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int DiscountId { get; set; }

        [Display(Name = "Дата создания")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime DateTimeCreated { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Скидка в процентах")]
        [Render(Control = RenderControl.Decimal)]
        public decimal DiscountInPercentage { get; set; }

        [Display(Name = "Область действия")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(DiscountArea))]
        public int DiscountArea { get; set; }

        [Display(Name = "Скидка активна")]
        [Render(Control = RenderControl.YesNo)]
        public bool IsEnabled { get; set; }

        [Display(Name = "Применяется к витринам")]
        [Render(Control = RenderControl.Input, FilterState = FilterState.Disable, SortState = SortState.Disable)]
        public string AppliedTo { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/Shared/Actions/_editActionControl.cshtml")]
        public string Action { get; set; }
        
        [Render(Ignore = true)]
        public override Type EntityType() => typeof(Discount);
    }
}
