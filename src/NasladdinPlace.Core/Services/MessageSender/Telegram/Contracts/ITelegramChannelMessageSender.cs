using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts
{
    public interface ITelegramChannelMessageSender
    {
        Task SendAsync(string message);
    }
}