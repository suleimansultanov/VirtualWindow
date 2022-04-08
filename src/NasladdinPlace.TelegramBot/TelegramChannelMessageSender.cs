using System.Threading.Tasks;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;

namespace NasladdinPlace.TelegramBot
{
    public class TelegramChannelMessageSender : ITelegramChannelMessageSender
    {
        private readonly long _chatId;
        private readonly IMessageSender _messageSender;

        public TelegramChannelMessageSender(
            long chatId,
            IMessageSender messageSender)
        {
            _chatId = chatId;
            _messageSender = messageSender;
        }
        
        public Task SendAsync(string message)
        {
            return _messageSender.SendMessage(_chatId, message);
        }
    }
}