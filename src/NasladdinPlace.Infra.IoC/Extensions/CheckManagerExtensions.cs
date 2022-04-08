using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Check.Factory;
using NasladdinPlace.Core.Services.Check.Helpers;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Calculator;
using NasladdinPlace.Core.Services.Check.Refund.Calculator.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion;
using NasladdinPlace.Core.Services.Purchase.Completion.Contracts;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class CheckManagerExtensions
    {
        public static void AddCheckManager(this IServiceCollection services)
        {
            services.AddSingleton<ICheckRefundCalculator, CheckRefundCalculator>();
            services.AddSingleton<ICheckManagerBonusPointsHelper, CheckManagerBonusPointsHelper>();
            services.AddSingleton<IPurchaseCompletionManager, PurchaseCompletionManager>();

            services.AddSingleton(CheckManagerFactory.Create);
        }
    }
}
