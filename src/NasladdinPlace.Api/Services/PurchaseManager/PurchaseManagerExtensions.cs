using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters;
using NasladdinPlace.Core.Services.Purchase.Completion.Printer.Formatters.Contracts;
using NasladdinPlace.Core.Services.Purchase.Factory;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.PushNotifications;
using NasladdinPlace.Core.Services.PushNotifications.Models.PushNotification;
using Serilog;
using System;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Check.Simple.Payment;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Helpers;
using NasladdinPlace.Core.Services.Printers.Localization;

namespace NasladdinPlace.Api.Services.PurchaseManager
{
    public static class PurchaseManagerExtensions
    {
        public static void AddPurchaseManager(this IServiceCollection services, string detailedCheckAdminPageBaseUrl)
        {
           services.AddSingleton<IPrintedCheckLinkFormatter>(sp =>
                new PrintedCheckLinkFormatter(detailedCheckAdminPageBaseUrl));
            services.AddSingleton<IPurchaseCompletionResultPrinter, PurchaseCompletionResultRussianPrinter>();
            services.AddTransient<IPaymentInfoCreator, PaymentInfoCreator>();
            services.AddSingleton<ICheckPaymentService, CheckPaymentService>();

            services.AddSingleton(sp => new PurchaseManagerFactory(sp).Create());
        }
        
        public static void UsePurchaseManagerLogging(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var checkPrinterFactory = services.GetRequiredService<ILocalizedPrintersFactory<SimpleCheck>>();
            var checkPrinter = checkPrinterFactory.CreatePrinter(Language.English);

            SubscribeForPurchaseCompletionResult(services, checkPrinter, (purchaseCompletionResult, printedCheck) =>
            {
                var userId = purchaseCompletionResult.User.Id;
                
                var logger = services.GetRequiredService<ILogger>();
                
                if (purchaseCompletionResult.Status == PurchaseCompletionStatus.Success)
                {
                    logger.Information(
                        $"User {userId} has completed operation {purchaseCompletionResult.Operation.Id}. " +
                        $"One's check is {printedCheck}"
                    );
                } 
                else if (purchaseCompletionResult.Status == PurchaseCompletionStatus.PaymentError)
                {
                    logger.Error($"User {userId} has tried to pay for the purchase but the payment operation has failed " +
                                 $"because {purchaseCompletionResult.Error.Description}.");
                }
                else
                {
                    logger.Error($"User {userId} cannot complete purchase because {purchaseCompletionResult.Error}");
                }
            });
        }

        public static void UsePurchaseManagerTelegramNotifications(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var checkPrinterFactory = services.GetRequiredService<ILocalizedPrintersFactory<SimpleCheck>>();
            var checkPrinter = checkPrinterFactory.CreatePrinter(Language.English);

            SubscribeForPurchaseCompletionResult(services, checkPrinter, (purchaseCompletionResult, printedCheck) =>
            {            
                var telegramMessageSender = services.GetRequiredService<ITelegramChannelMessageSender>();
                var purchaseManagerPrinter = services.GetRequiredService<IPurchaseCompletionResultPrinter>();

                if (purchaseCompletionResult.Status == PurchaseCompletionStatus.UnknownError)
                    return;

                var purchaseCompletionResultMessage = purchaseManagerPrinter.Print(purchaseCompletionResult, printedCheck);

                if (string.IsNullOrWhiteSpace(purchaseCompletionResultMessage)) return;

                telegramMessageSender.SendAsync(purchaseCompletionResultMessage);
            });
        }

        public static void UsePurchaseManagerPushNotifications(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var checkPrinterFactory = services.GetRequiredService<ILocalizedPrintersFactory<SimpleCheck>>();
            var checkPrinter = checkPrinterFactory.CreatePrinter(Language.Russian, false);

            SubscribeForPurchaseCompletionResult(services, checkPrinter,(purchaseCompletionResult, printedCheck) =>
            {
                var check = purchaseCompletionResult.Check;

                var user = purchaseCompletionResult.User;

                if (!purchaseCompletionResult.IsSuccess || check.Summary.CostSummary.IsEmpty)
                    return;

                var firebaseToken = user.FindFirebaseTokenByBrand(purchaseCompletionResult.Operation.Brand);

                if (firebaseToken == null)
                    return;

                var pushNotificationsService = services.GetRequiredService<IPushNotificationsService>();

                var pushNotificationContent = new PushNotificationContent(
                    title: "Чек",
                    body: printedCheck
                )
                {
                    AdditionalInfo =
                    {
                        { "push_intent",  "true" },
                        { "check_id", check.Id.ToString() }
                    }
                };
                
                var pushNotification = new PushNotification(firebaseToken.Token, pushNotificationContent, area: NotificationArea.Purchase);

                pushNotificationsService.SendNotificationAsync(pushNotification);
            });
        }

        private static void SubscribeForPurchaseCompletionResult(
            IServiceProvider services,
            ILocalizedPrinter<SimpleCheck> simpleCheckPrinter,
            Action<PurchaseCompletionResult, string> purchaseCompletionResultWithCheckHandler)
        {
            var purchaseManager = services.GetRequiredService<IPurchaseManager>();
            purchaseManager.PurchaseCompleted += (sender, purchaseCompletionResult) =>
            {
                var printedCheck = simpleCheckPrinter.Print(purchaseCompletionResult.Check);
                purchaseCompletionResultWithCheckHandler(purchaseCompletionResult, printedCheck);
            };
        }
    }
}