using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Repositories.UserNotification
{
    public class UserNotificationsFilter
    {
        public int? UserId { get; set; }
        public NotificationArea? NotificationArea { get; set; }
        public NotificationType? NotificationType { get; set; }
    }
}