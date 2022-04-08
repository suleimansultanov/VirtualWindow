using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification
{
    public class PushNotification
    {
        public string RecipientToken { get; }
        public PushNotificationContent Content { get; }
        public NotificationArea Area { get; set; }

        public PushNotification(string recipientToken, PushNotificationContent content, NotificationArea area)
        {
            RecipientToken = recipientToken;
            Content = content;
            Area = area;
        }
    }
}