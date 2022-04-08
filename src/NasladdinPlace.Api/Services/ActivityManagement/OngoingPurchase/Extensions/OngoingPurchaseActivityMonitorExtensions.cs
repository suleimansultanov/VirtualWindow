using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.ActivityManagement;
using NasladdinPlace.Core.Services.ActivityManagement.Handlers;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Core.Utils.TasksAgent;

namespace NasladdinPlace.Api.Services.ActivityManagement.OngoingPurchase.Extensions
{
    public static class OngoingPurchaseActivityMonitorExtensions
    {
        public static void AddOngoingPurchaseActivityMonitor(
            this IServiceCollection services, IConfigurationReader configurationReader)
        {
            var activityTimeout = configurationReader.GetOngoingPurchaseActivityMonitorTimeoutInSeconds();
            services.AddTransient<IActivityManager<int>, ActivityManager<int>>();
            services.AddTransient<InactivePosHandler>();
            services.AddTransient<InactiveUserHandler>();
            services.AddSingleton<IOngoingPurchaseActivityManager, OngoingPurchaseActivityManager>();
            services.AddSingleton<IOngoingPurchaseActivityMonitor>(sp =>
            {
                var ongoingPurchaseActivityMonitor = new OngoingPurchaseActivityMonitor(
                    sp.GetRequiredService<IOngoingPurchaseActivityManager>(),
                    sp.GetRequiredService<ITasksAgent>(),
                    TimeSpan.FromSeconds(activityTimeout));

                var inactivePosHandler = sp.GetRequiredService<InactivePosHandler>();
                ongoingPurchaseActivityMonitor.OnPosBecomeInactive += (sender, argument) =>
                {
                    Task.Run(() => inactivePosHandler.Handle(argument.Message));
                };

                var inactiveUserHandler = sp.GetRequiredService<InactiveUserHandler>();
                ongoingPurchaseActivityMonitor.OnUserBecomeInactive += (sender, argument) =>
                {
                    Task.Run(() => inactiveUserHandler.Handle(argument.Message));
                };
                
                return ongoingPurchaseActivityMonitor;
            });
        }
    }
}