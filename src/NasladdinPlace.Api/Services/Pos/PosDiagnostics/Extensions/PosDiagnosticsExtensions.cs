using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.PosDiagnostics.Agent;
using NasladdinPlace.Core.Services.PosDiagnostics.Manager;
using NasladdinPlace.Core.Services.PosDiagnostics.Report;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.Pos.PosDiagnostics.Extensions
{
    public static class PosDiagnosticsExtensions
    {
        public static void AddPosDiagnostics(this IServiceCollection services,
            int inventoryTimeoutInSeconds,
            int doorStateTimeoutInSeconds,
            bool includeDoorsStateCheck)
        {
            services.AddSingleton<IPosDiagnosticsReportBuilder, PosDiagnosticsReportBuilder>();
            services.AddSingleton<IPosDiagnosticsManager>(sp => new PosDiagnosticsManager(
                sp.GetRequiredService<IPosInteractor>(),
                sp.GetRequiredService<IUnitOfWorkFactory>(),
                inventoryTimeoutInSeconds: inventoryTimeoutInSeconds,
                includeDoorsStateCheck: includeDoorsStateCheck,
                doorStateTimeoutInSeconds: doorStateTimeoutInSeconds));
            services.AddSingleton<IPosDiagnosticsAgent, PosDiagnosticsAgent>();
        }

        public static void UsePosDiagnosticsReport(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var posDiagnosticsManager = serviceProvider.GetRequiredService<IPosDiagnosticsManager>();
            var posDiagnosticsReportBuilder = serviceProvider.GetRequiredService<IPosDiagnosticsReportBuilder>();
            var telegramChannelMessageSender = serviceProvider.GetRequiredService<ITelegramChannelMessageSender>();

            posDiagnosticsManager.OnDiagnosticsCompleted += (sender, reportContent) =>
            {
                var message = posDiagnosticsReportBuilder.BuildReport(reportContent);
                telegramChannelMessageSender.SendAsync(message);
            };
        }

        public static void RunPosDiagnosticsAgent(this IApplicationBuilder app, TasksAgentOptions options)
        {
            var serviceProvider = app.ApplicationServices;
            var posDiagnosticsAgent = serviceProvider.GetRequiredService<IPosDiagnosticsAgent>();

            posDiagnosticsAgent.Start(options);
        }
    }
}
