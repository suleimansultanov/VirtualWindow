using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Pos.PosLogs.Agents;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Utils.TasksAgent;
using System;

namespace NasladdinPlace.Api.Services.Pos.PosLogs
{
    public static class PosLogsExtensions
    {
        private static readonly TimeSpan PosLogsRequestMoscowTime = new TimeSpan(00, 10, 00);


        public static void AddDailyPosLogsRequestsAgent(this IServiceCollection services, int storePosLogsForDays)
        {
            services.AddSingleton<IPosLogsAgent>(sp =>
                new PosLogsAgent(
                    sp.GetRequiredService<ITasksAgent>(),
                    sp.GetRequiredService<IUnitOfWorkFactory>(),
                    storePosLogsForDays
                ));
        }

        public static void UsePosLogsRequestsAgent(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var posLogsAgent = serviceProvider.GetRequiredService<IPosLogsAgent>();
            var posInteractor = serviceProvider.GetRequiredService<IPosInteractor>();

            posLogsAgent.OnPerformDailyLogsRequest += (sender, dailyLogs) =>
            {
                //TODO: Переделать работу с логами витрин вне БД. 
                //foreach (var posId in dailyLogs.PosIdsForRequest)
                //{
                //    posInteractor.RequestLogsAsync(posId, PosLogType.Daily);
                //}
            };

            posLogsAgent.Start(TimeSpan.FromDays(1), PosLogsRequestMoscowTime);
        }
    }
}
