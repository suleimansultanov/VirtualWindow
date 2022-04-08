using NasladdinPlace.Core.Services.MessageSender.Telegram.Models;

namespace NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts
{
    public interface ITelegramChannelsFactory
    {
        ITelegramChannelMessageSender Create(TelegramChannel telegramChannel);
    }
}