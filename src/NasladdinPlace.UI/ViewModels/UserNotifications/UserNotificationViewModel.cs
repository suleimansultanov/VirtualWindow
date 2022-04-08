using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.UserNotifications
{
    public class UserNotificationViewModel : BaseViewModel
    {
        [Display(Name = "Дата сообщения")]
        [Render(Control = RenderControl.DateTime, FilterState = FilterState.Disable)]
        public DateTime DateTimeSent { get; set; }

        [Render(Control = RenderControl.TextReferences, TextReference = "UserName", FilterState = FilterState.Disable)]
        [Display(Name = "Имя пользователя")]
        public int UserId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string UserName { get; set; }

        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Push токен")]
        public string PushNotificationToken { get; set; }

        [Display(Name = "Текст сообщения")]
        public string MessageText { get; set; }

        [Display(Name = "Тип уведомления")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(NotificationType))]
        public byte NotificationType { get; set; }

        [Display(Name = "Область уведомления")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(NotificationArea))]
        public byte NotificationArea { get; set; }
       
        [Render(Ignore = true)]
        public override Type EntityType() => typeof(UserNotification);
    
    }
}
