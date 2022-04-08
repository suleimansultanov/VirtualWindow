using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Models;

namespace NasladdinPlace.TelegramBot
{
    public class TelegramChannelsFactory : ITelegramChannelsFactory
    {
        private readonly IMessageSender _telegramMessageSender;
        private readonly IDictionary<Core.Services.MessageSender.Telegram.Models.TelegramChannel, long> _telegramChatIdByChannelDictionary;

        public TelegramChannelsFactory(
            IMessageSender telegramMessageSender,
            IDictionary<Core.Services.MessageSender.Telegram.Models.TelegramChannel, long> telegramChatIdByChannelDictionary)
        {
            _telegramMessageSender = telegramMessageSender;
            _telegramChatIdByChannelDictionary = telegramChatIdByChannelDictionary;
        }
        
        public ITelegramChannelMessageSender Create(Core.Services.MessageSender.Telegram.Models.TelegramChannel telegramChannel)
        {
            if (!_telegramChatIdByChannelDictionary.ContainsKey(telegramChannel))
                throw new ArgumentException(nameof(telegramChannel), telegramChannel.ToString());
            
            var chatId = _telegramChatIdByChannelDictionary[telegramChannel];
            
            return new TelegramChannelMessageSender(chatId, _telegramMessageSender);
        }
    }
}