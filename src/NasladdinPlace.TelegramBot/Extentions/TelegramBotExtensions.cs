using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.TelegramBot.Factories;
using NasladdinPlace.TelegramBot.Factories.Contracts;
using System.Collections.Generic;
using Telegram.Bot;

namespace NasladdinPlace.TelegramBot.Extentions
{
    public static class TelegramBotExtensions
    {
        public static void AddTelegramBot(this IServiceCollection services,
            string apiKey,
            string telegramChannelId)
        {
            if (!long.TryParse(telegramChannelId, out var telegramChannelChatId))
                throw new ArgumentNullException(nameof(telegramChannelId), "Make sure you have the correct application configurations.");

            services.AddSingleton<ITelegramBotClient>(sp =>
            {
                var telegramBotClient = new TelegramBotClient(apiKey);

                return telegramBotClient;
            });

            services.AddSingleton<IKeyboardButtonFactory, KeyboardButtonFactory>();
            services.AddSingleton<IMessageSender, TelegramMessageSender>();
            services.AddSingleton<ITelegramChannelMessageSender>(sp =>
            {
                var messageSender = sp.GetRequiredService<IMessageSender>();
                return new TelegramChannelMessageSender(telegramChannelChatId, messageSender);
            });
            services.AddSingleton<ITelegramChannelsFactory>(sp =>
            {
                var telegramMessageSender = sp.GetRequiredService<IMessageSender>();
                var telegramChatIdByChannelDictionary = new Dictionary<Core.Services.MessageSender.Telegram.Models.TelegramChannel, long>
                {
                    {Core.Services.MessageSender.Telegram.Models.TelegramChannel.Sales, telegramChannelChatId}
                };

                return new TelegramChannelsFactory(telegramMessageSender, telegramChatIdByChannelDictionary);
            });
        }
    }
}
