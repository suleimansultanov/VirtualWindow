using NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;

namespace NasladdinPlace.Core.Services.NotificationsLogger
{
    public interface INotificationsLogger
    {
        Task LogSmsAsync(SmsLoggingInfo smsInfo);
        Task LogPushAsync(PushNotification pushNotification);
    }
}
