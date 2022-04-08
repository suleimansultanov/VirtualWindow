using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.Promotions
{
    public class PromotionSettingViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int PromotionSettingId { get; set; }

        [Display(Name = "Промо акция")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(PromotionType), ReadOnly = true)]
        public PromotionType PromotionType { get; set; }

        [Display(Name = "Бонусы")]
        [Render(Control = RenderControl.Decimal, Min = "0")]
        public decimal BonusValue { get; set; }

        [Display(Name = "Акция активна")]
        [Render(Control = RenderControl.YesNo)]
        public bool IsEnabled { get; set; }

        [Display(Name = "Разрешить уведомления")]
        [Render(Control = RenderControl.YesNo)]
        public bool IsNotificationEnabled { get; set; }

        [Display(Name = "Время рассылки")]
        [Render(Control = RenderControl.TimeSpan)]
        public TimeSpan NotificationStartTime { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/Shared/Actions/_editActionControl.cshtml")]
        public string Action { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(PromotionSetting);
    }
}
