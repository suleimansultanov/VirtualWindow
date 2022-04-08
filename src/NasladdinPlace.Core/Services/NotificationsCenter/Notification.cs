namespace NasladdinPlace.Core.Services.NotificationsCenter
{
    public class Notification<T>
    {
        public string Title { get; }
        public T Body { get; }

        public Notification(string title, T body)
        {
            Title = title;
            Body = body;
        }

        public Notification(T body)
        {
            Title = string.Empty;
            Body = body;
        }
    }
}