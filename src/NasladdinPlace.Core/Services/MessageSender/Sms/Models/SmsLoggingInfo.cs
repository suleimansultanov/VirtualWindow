using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.MessageSender.Sms.Models
{
    public class SmsLoggingInfo
    {
        public int? UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public NotificationArea NotificationArea { get; set; } = NotificationArea.Other;
    }
}
