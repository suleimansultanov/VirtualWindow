using System.Threading.Tasks;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Models;

namespace NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts
{
    public interface IMessageSender
    {
        Task SendButton(long chatId, ButtonType buttonType, string buttonName, string additionalMessage);
        Task SendMessage(long chatId, string message);
    }
}
