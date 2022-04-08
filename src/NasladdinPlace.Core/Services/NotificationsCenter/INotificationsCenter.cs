namespace NasladdinPlace.Core.Services.NotificationsCenter
{
    public interface INotificationsCenter
    {
        void PostNotification<T>(string title, T body);
        void PostNotification<T>(Notification<T> notification);
    }
}