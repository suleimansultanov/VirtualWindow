using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.NotificationsCenter
{
    public interface INotificationChannel
    {
        Task TransmitMessageAsync(string message);
    }
}