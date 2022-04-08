using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Pos.Display.Agents;
using NasladdinPlace.Api.Services.Pos.Display.Managers;

namespace NasladdinPlace.Api.Services.Pos.Display
{
    public static class PosDisplayExtensions
    {
        public static void AddPosDisplayServices(this IServiceCollection services)
        {
            services.AddSingleton<IPosDisplayAgent, PosDisplayAgent>();
            services.AddSingleton<IPosDisplayManager, PosDisplayManager>();
            services.AddSingleton<IPosDisplayCommandsManager, PosDisplayCommandsManager>();
            services.AddSingleton<IPosDisplayCommandsQueueManager, PosDisplayCommandsQueueManager>();
        }

        public static void UsePosDisplayServices(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var reportsAgent = serviceProvider.GetRequiredService<IPosDisplayAgent>();
            var commandsManager = serviceProvider.GetRequiredService<IPosDisplayCommandsManager>();

            var posDisplaySettingsManager = serviceProvider.GetRequiredService<IPosDisplaySettingsManager>();
            var posDisplaySettings = posDisplaySettingsManager.GetPosDisplaySettings();

            reportsAgent.StartCheckCommandsForRetrySend(TimeSpan.FromSeconds(posDisplaySettings.CheckCommandsForRetrySendInSeconds));

            reportsAgent.OnPosDisplayActionExecuted += (sender, commands) =>
            {
                foreach (var posDisplayCommand in commands)
                {
                    posDisplayCommand.ScheduleNextExecutionDateTime();

                    commandsManager.RetryExecutePosDisplayCommand(posDisplayCommand);
                }                
            };
        }
    }
}
