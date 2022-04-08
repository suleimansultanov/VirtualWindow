using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using ILogger = Serilog.ILogger;

namespace NasladdinPlace.Api.Services.BankingCardConfirmation.Extensions
{
    public static class BankingCardConfirmationServiceExtensions
    {
        public static void AddBankingCardConfirmationService(this IServiceCollection services, string termUrlWithUserIdFormat)
        {   
            services.AddSingleton<ViewRender.ViewRender>();
            
            services.AddSingleton<IForm3DsHtmlMaker>(sp => 
                new Form3DsHtmlMaker(sp.GetRequiredService<ViewRender.ViewRender>(), termUrlWithUserIdFormat)
            );

            services.AddSingleton<IPaymentCardConfirmationService, PaymentCardConfirmationService>();
        }

        public static void UseBankingCardConfirmationServiceLogging(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;

            using (var scope = services.CreateScope())
            {
                var bankingCardConfirmationService = scope.ServiceProvider.GetRequiredService<IPaymentCardConfirmationService>();

                bankingCardConfirmationService.ConfirmationProgressUpdated += (sender, notification) =>
                {
                    var logger = services.GetRequiredService<ILogger>();

                    var userId = notification.User.Id;
                    switch (notification.ConfirmationStatus)
                    {
                        case PaymentCardConfirmationStatus.ConfirmationFailed:
                            logger.Error(notification.Error);
                            break;
                        case PaymentCardConfirmationStatus.ConfirmationSucceeded:
                            logger.Information($"User {userId} has confirmed banking card.");
                            break;
                        case PaymentCardConfirmationStatus.Require3DsAuthorization:
                            logger.Information($"User {userId} has started 3D-Secure authorization.");
                            break;
                        case PaymentCardConfirmationStatus.ConfirmationInitiated:
                            logger.Information($"User {userId} has initiated banking card confirmation.");
                            break;
                        case PaymentCardConfirmationStatus.Authorization3DsCompletionInitiated:
                            logger.Information($"User {userId} has initiated 3D-Secure authorization completion.");
                            break;
                    }
                };
            }
        }

        public static void UseBankingCardConfirmationServiceTelegramNotifications(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;

            using (var scope = services.CreateScope())
            {
                var bankingCardConfirmationService = scope.ServiceProvider.GetRequiredService<IPaymentCardConfirmationService>();

                bankingCardConfirmationService.ConfirmationProgressUpdated += (sender, notification) =>
                {
                    var telegramChannelMessageSender = services.GetRequiredService<ITelegramChannelMessageSender>();

                    var userName = notification.User.UserName;

                    if (notification.ConfirmationStatus != PaymentCardConfirmationStatus.ConfirmationSucceeded)
                        return;

                    var telegramMessage = $"Пользователь {userName} успешно подтвердил банковскую карту.";
                    telegramChannelMessageSender.SendAsync(telegramMessage);
                };
            }
        }
        
    }
}