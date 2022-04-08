using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.Users
{
    public class UserViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide, Control = RenderControl.Integer)]
        public int Id { get; set; }

        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }
        
        [Display(Name = "Email-адрес")]
        public string Email { get; set; }

        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Подтверждение телефона")]
        [Render(Control = RenderControl.YesNo)]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "Начало регистрации")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime RegistrationInitiationDate { get; set; }

        [Display(Name = "Завершение регистрации")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? RegistrationCompletionDate { get; set; }

        [Display(Name = "Начало подтверждения карты")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? BankingCardVerificationInitiationDate { get; set; }

        [Display(Name = "Завершение подтверждения карты")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? BankingCardVerificationCompletionDate { get; set; }

        [Display(Name = "Последняя покупка")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime? LastPaidDate { get; set; }

        [Display(Name = "День рождения")]
        [Render(Control = RenderControl.DateTime, DisplayType = DisplayType.Hide)]
        public DateTime? Birthdate { get; set; }

        [Display(Name = "Пол")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(Gender), DisplayType = DisplayType.Hide)]
        public Gender? Gender { get; set; }

        [Display(Name = "Бонус")]
        [Render(Control = RenderControl.Decimal, FilterState = FilterState.Disable)]
        public decimal TotalBonus { get; set; }

        [Display(Name = "Карточка пользователя")]
        [Render(Control = RenderControl.Custom, PartialName = "~/Views/Shared/Actions/_editActionControl.cshtml")]
        public string Action { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public bool IsActive { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public List<int> SelectedRoles { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(ApplicationUser);
    }
}
