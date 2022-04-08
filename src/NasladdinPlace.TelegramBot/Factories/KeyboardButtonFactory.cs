using NasladdinPlace.Core.Services.MessageSender.Telegram.Models;
using NasladdinPlace.TelegramBot.Factories.Contracts;
using Telegram.Bot.Types.ReplyMarkups;

namespace NasladdinPlace.TelegramBot.Factories
{
    public class KeyboardButtonFactory : IKeyboardButtonFactory
    {
        public KeyboardButton MakeKeyboardButton(ButtonType buttonType, string buttonName)
        {
            var button = new KeyboardButton(buttonName);

            switch (buttonType)
            {
                case ButtonType.Contact:
                    button.RequestContact = true;
                    break;
                case ButtonType.Location:
                    button.RequestLocation = true;
                    break;
                default:
                    return button;
            }

            return button;
        }
    }
}
