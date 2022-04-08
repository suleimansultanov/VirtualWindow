using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Models;
using NasladdinPlace.Logging;
using NasladdinPlace.TelegramBot.Factories.Contracts;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NasladdinPlace.TelegramBot
{
    public class TelegramMessageSender : IMessageSender
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IKeyboardButtonFactory _keyboardButtonFactory;
        private readonly ILogger _logger;

        public TelegramMessageSender(
            ITelegramBotClient telegramBotClient,
            IKeyboardButtonFactory keyboardButtonFactory,
            ILogger logger)
        {
            _telegramBotClient = telegramBotClient;
            _keyboardButtonFactory = keyboardButtonFactory;
            _logger = logger;
        }

        public Task SendButton(
            long chatId, 
            ButtonType buttonType, 
            string buttonName, 
            string additionalMessage)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                _keyboardButtonFactory.MakeKeyboardButton(buttonType, buttonName)
            });

			return Task.Factory.StartNew( () =>
				{
					try
					{
						_telegramBotClient.SendTextMessageAsync( chatId, additionalMessage, replyMarkup: keyboard ).GetAwaiter().GetResult();
					}
					catch ( Exception e )
					{
						_logger.LogError( $"An error has occurred during telegram message send: {e}" );
					}
				}
			);
		}

        public Task SendMessage(long chatId, string message)
        {
	        return Task.Factory.StartNew( () =>
		        {
			        try
			        {
						_telegramBotClient.SendTextMessageAsync( chatId, message, ParseMode.Markdown ).GetAwaiter().GetResult();
					}
			        catch ( Exception e )
			        {
						_logger.LogError( $"An error has occurred during telegram message send: {e}" );
					}
		        }
	        );
        }
    }
}
