using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Pos.Agents;
using NasladdinPlace.Api.Services.Pos.Agents.Contracts;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Services.Pos.Doors;
using NasladdinPlace.Core.Services.Pos.Doors.Contracts;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureAgent;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureManager;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.Pos.Temperature.Extensions
{
    public static class PosStateServicesExtensions
    {
        public static void AddPosStateServices(this IServiceCollection services, IConfigurationReader configurationReader)
        {
            services.AddSingleton<IPosTemperatureAgent, PosTemperatureAgent>();
            services.AddSingleton<IPosTemperatureManager, PosTemperatureManager>();
            services.AddSingleton<IPointsOfSaleStateHistoricalDataDeletingAgent, PointsOfSaleStateHistoricalDataDeletingAgent>();
            services.AddSingleton<IPosDoorsStateTracker, PosDoorsStateTracker>();
        }

        public static void RunPointsOfSaleStateHistoricalDataDeletingAgent(this IApplicationBuilder app, IConfigurationReader configurationReader)
        {
            var services = app.ApplicationServices;
            var historicalDataSettings =
                services.GetRequiredService<IPosStateSettingsProvider>().GetHistoricalDataSettings();

            var agentOptions = TasksAgentOptions.DailyStartAtFixedTime(historicalDataSettings.DeletingObsoleteHistoricalDataStartTime);

            var pointsOfSaleStateHistoricalDataDeletingAgent = services.GetRequiredService<IPointsOfSaleStateHistoricalDataDeletingAgent>();
            pointsOfSaleStateHistoricalDataDeletingAgent.Start(agentOptions);
        }
    }
}
