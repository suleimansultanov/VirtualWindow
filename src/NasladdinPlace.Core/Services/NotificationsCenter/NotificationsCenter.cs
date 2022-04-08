using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Printers.Factory;

namespace NasladdinPlace.Core.Services.NotificationsCenter
{
    public class NotificationsCenter : INotificationsCenter
    {
        private readonly IEnumerable<INotificationChannel> _notificationChannels;
        private readonly IMessagePrintersFactory _messagePrintersFactory;

        public NotificationsCenter(
            IEnumerable<INotificationChannel> notificationChannels,
            IMessagePrintersFactory messagePrintersFactory)
        {
            _notificationChannels = notificationChannels.ToImmutableList();
            _messagePrintersFactory = messagePrintersFactory;
        }

        public void PostNotification<T>(string title, T body)
        {
            PostNotification(new Notification<T>(title, body));
        }

        public void PostNotification<T>(Notification<T> notification)
        {
            Task.Run(async () =>
            {
                try
                {
                    var messagePrinter = _messagePrintersFactory.CreatePrinterFor<T>();
                    var message = messagePrinter.Print(notification.Body);

                    var messageWithTitle = $"{notification.Title}\n{message}";

                    var messageHandlingTasks = _notificationChannels
                        .Select(notificationChannel => notificationChannel.TransmitMessageAsync(messageWithTitle))
                        .ToImmutableList();

                    await Task.WhenAll(messageHandlingTasks);
                }
                catch (Exception)
                {
                    // do nothing
                }
            });
        }
    }
}