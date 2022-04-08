using System.Threading.Tasks;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.NotificationsCenter;

namespace NasladdinPlace.TelegramBot
{
    public class TelegramNotificationChannel : INotificationChannel
    {
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;

        public TelegramNotificationChannel(ITelegramChannelMessageSender telegramChannelMessageSender)
        {
            _telegramChannelMessageSender = telegramChannelMessageSender;
        }
        
        public Task TransmitMessageAsync(string message)
        {
            return _telegramChannelMessageSender.SendAsync(message);
        }
    }
}