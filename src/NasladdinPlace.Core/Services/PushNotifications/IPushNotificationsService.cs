using NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification;
using NasladdinPlace.Core.Services.PushNotifications.Models.Token;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PushNotifications
{
    public interface IPushNotificationsService
    {
        Task<PushNotificationDeliveryResult> SendNotificationAsync(PushNotification pushNotification);
        Task<ValueResult<PushNotificationTokenInfo>> GetTokenInfoAsync(string token);
    }
}