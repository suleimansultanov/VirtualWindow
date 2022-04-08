using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.PosNotifications.Agent;
using NasladdinPlace.Core.Services.PosNotifications.Agent.Contracts;
using NasladdinPlace.Core.Services.PosNotifications.Manager;
using NasladdinPlace.Core.Services.PosNotifications.Manager.Contracts;
using NasladdinPlace.Core.Services.PosNotifications.Printer;
using NasladdinPlace.Core.Services.PosNotifications.Printer.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.PosNotifications.Extensions
{
    public static class PosNotificationsExtension
    {
        public static void AddPosNotificationsAgent(this IServiceCollection services)
        {
            services.AddSingleton<IPosDisabledNotificationsMessagePrinter, PosDisabledNotificationsRussianMessagePrinter>();
            services.AddSingleton<IPosNotificationsManager>(sp => new PosNotificationsManager(sp.GetRequiredService<IUnitOfWorkFactory>()));
            services.AddSingleton<IPosNotificationsAgent>(sp => new PosNotificationsAgent(
                sp.GetRequiredService<ITasksAgent>(),
                sp.GetRequiredService<IPosNotificationsManager>()));
        }

        public static void UsePosNotificationsAgentTelegramNotifications(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;

            var telegramMessageSender = serviceProvider.GetRequiredService<ITelegramChannelMessageSender>();
            var posNotificationsManager = serviceProvider.GetRequiredService<IPosNotificationsManager>();
            var posDisabledNotificationsMessagePrinter = serviceProvider.GetRequiredService<IPosDisabledNotificationsMessagePrinter>();

            posNotificationsManager.OnPosNotificationsBecomeDisabled += async (sender, posDisabledNotificationsInfos) =>
                {
                    await telegramMessageSender.SendAsync(
                        posDisabledNotificationsMessagePrinter.Print(posDisabledNotificationsInfos));
                };
        }

        public static void RunPosNotificationsAgent(this IApplicationBuilder app, TasksAgentOptions options)
        {
            var serviceProvider = app.ApplicationServices;
            var posNotificationsAgent = serviceProvider.GetRequiredService<IPosNotificationsAgent>();
            posNotificationsAgent.Start(options);
        }
    }
}