using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Purchase.Conditional.Agent;
using NasladdinPlace.Core.Services.Purchase.Conditional.Agent.Contracts;
using NasladdinPlace.Core.Services.Purchase.Conditional.Manager;
using NasladdinPlace.Core.Services.Purchase.Conditional.Manager.Contracts;
using NasladdinPlace.Core.Utils.TasksAgent;
using Serilog;
using System;
using NasladdinPlace.Core.Services.Check.Refund.Contracts;

namespace NasladdinPlace.Api.Services.PurchaseManager.Conditional
{
    public static class ConditionalPurchaseExtensions
    {
        public static void AddConditionalPurchaseAgent(this IServiceCollection services, TimeSpan conditionalPurchaseDeletionOnLabelAppearenceAfterTime)
        {
            services.AddSingleton<IConditionalPurchaseManager>(sp =>
                new ConditionalPurchaseManager(
                    sp.GetRequiredService<IUnitOfWorkFactory>(),
                    sp.GetRequiredService<ICheckManager>(),
                    conditionalPurchaseDeletionOnLabelAppearenceAfterTime));
            services.AddSingleton<IConditionalPurchasesAgent>(sp => new ConditionalPurchasesAgent(
                sp.GetRequiredService<ITasksAgent>(),
                sp.GetRequiredService<IConditionalPurchaseManager>()));
        }

        public static void UseConditionalPurchaseManager(this IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var conditionalPurchaseManager = serviceProvider.GetRequiredService<IConditionalPurchaseManager>();
            var logger = serviceProvider.GetRequiredService<ILogger>();

            conditionalPurchaseManager.OperationCompleted +=
                (sender, conditionalPurchaseManagerResult) =>
                {
                    logger.Information(
                        $"The number of check items modified after {conditionalPurchaseManagerResult.Type} is {conditionalPurchaseManagerResult.Count}.");
                };
        }

        public static void RunConditionalPurchaseAgent(this IApplicationBuilder app, TasksAgentOptions options)
        {
            var serviceProvider = app.ApplicationServices;
            var сonditionalPurchasesAgent =
                serviceProvider.GetRequiredService<IConditionalPurchasesAgent>();

            сonditionalPurchasesAgent.Start(options);
        }
    }
}
