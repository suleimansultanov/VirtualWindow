using NasladdinPlace.Core.Services.MessageSender.Telegram.Models;
using Telegram.Bot.Types.ReplyMarkups;
namespace NasladdinPlace.TelegramBot.Factories.Contracts
{
    public interface IKeyboardButtonFactory
    {
        KeyboardButton MakeKeyboardButton(ButtonType buttonType, string buttonName);
    }
}
