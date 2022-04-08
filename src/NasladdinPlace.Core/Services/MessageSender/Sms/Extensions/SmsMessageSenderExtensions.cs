using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.MessageSender.Sms.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.NotificationsLogger;
using NasladdinPlace.Core.Services.WebClient;
using NasladdinPlace.Core.Services.WebClient.Contracts;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Services.MessageSender.Sms.Extensions
{
    public static class SmsMessageSenderExtensions
    {
	    public static void AddSmsMessageSender( this IServiceCollection services,
		    IConfigurationReader configurationReader, bool isDevelopment )
	    {
		    services.AddTransient<IJsonWebClient, JsonWebClient>();

		    var apiId = configurationReader.GetApiId();
		    var url = configurationReader.GetSendApiRequestUrl();
		    var minimumPositiveBalance = configurationReader.GetMinimumPositiveBalance();
		    var senderName = configurationReader.GetSenderName();

		    var settings = new SmsRuApiSettings( apiId, url, isDevelopment, minimumPositiveBalance, senderName );
		    services.AddSingleton<ISmsSender>( sp => new SmsRuMessageSender(
			    sp.GetRequiredService<IJsonWebClient>(),
			    sp.GetRequiredService<INotificationsLogger>(),
			    sp.GetRequiredService<ILogger>(),
			    settings
		    ) );
	    }

	    public static void UseSmsMessageSenderTelegramNotifications(this IApplicationBuilder app)
        {
            var smsSender = app.ApplicationServices.GetRequiredService<ISmsSender>();
            var telegramChannelSender = app.ApplicationServices.GetRequiredService<ITelegramChannelMessageSender>();
            smsSender.BalanceAlmostExceededHandler += (sender, balance) =>
            {
                var formattedBalance = string.Format(new CultureInfo("ru-RU"), "{0:C2}", balance);
                telegramChannelSender.SendAsync($"Баланс для отправки смс сообщений почти закончился и составляет {formattedBalance}.");
            };
            smsSender.SmsServiceErrorHandler += (sender, smsLoggingInfo) =>
            {
                var stringBuilder = new StringBuilder("*Ошибка при отправке смс:*");
                stringBuilder.AppendLine(smsLoggingInfo.Message);
                stringBuilder.AppendLine($"*Телефон:* {smsLoggingInfo.PhoneNumber}");
                telegramChannelSender.SendAsync(stringBuilder.ToString());
            };
        }
    }
}
