using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.ActivityManagement.PosComponents.InactivityAlertSender;
using NasladdinPlace.Api.Services.ActivityManagement.PosComponents.InactivityAlertSender.Contracts;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Contracts;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Printer;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Printer.Contracts;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.ActivityManagement.PosComponents.Extensions
{
    public static class PosComponentsActivityMonitorExtensions
    {
        public static void AddPosDisplayActivityMonitor(this IServiceCollection services, IConfigurationReader configurationReader)
        {
            var repeatNotificationInterval = configurationReader.GetRepeatNotificationInMinutes();
            services.AddSingleton<IPosComponentsActivityManager, PosComponentsActivityManager>();
            services.AddSingleton<IInactivePosComponentsAlertMessagePrinter, InactivePosComponentsAlertRussianMessagePrinter>();

            services.AddSingleton<IPosComponentsInactivityAlertSender>(sp => new PosComponentsInactivityAlertSender(
                sp.GetRequiredService<IInactivePosComponentsAlertMessagePrinter>(),
                sp.GetRequiredService<ITelegramChannelMessageSender>(),
                repeatNotificationInterval));

            services.AddSingleton<IPosComponentsActivityMonitor>(sp => new PosComponentsActivityMonitor(
                sp.GetRequiredService<ITasksAgent>(),
                sp.GetRequiredService<IPosComponentsActivityManager>()));
        }

        public static void UsePosComponentsInactiveTelegramNotification(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var posDisplaysActivityMonitor = services.GetRequiredService<IPosComponentsActivityMonitor>();
            var posDisplaysInactivityAlertSender = services.GetRequiredService<IPosComponentsInactivityAlertSender>();

            posDisplaysActivityMonitor.OnPosComponentsBecomeInactive += async (sender, posDisplayInactivityInfos) =>
            {
                await posDisplaysInactivityAlertSender.SendAsync(posDisplayInactivityInfos);
            };
        }

        public static void UsePosDisplaysActivityMonitor(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;
            var configurationReader = services.GetRequiredService<IConfigurationReader>();
            var posDisplaysActivityMonitor = services.GetRequiredService<IPosComponentsActivityMonitor>();
            
            var minBatteryLevel = configurationReader.GetPosDeviceMinBatteryLevel();
            var timeIntervalBetweenNextStart = configurationReader.GetTimeIntervalBetweenNextStartInSeconds();
            var inactiveTimeout = configurationReader.GetPosInactiveTimeoutInMinutes();
            
            posDisplaysActivityMonitor.Start(TimeSpan.FromSeconds(timeIntervalBetweenNextStart), 
                TimeSpan.FromMinutes(inactiveTimeout), minBatteryLevel);
        }
    }
}