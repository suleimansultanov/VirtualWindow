using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Application.Services.PosOperations.Contracts;
using NasladdinPlace.Application.Services.PosOperations.Helpers;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Utils.ActionExecution.FrequencyLimiter;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Application.Services.PosOperations.Factory
{
    public static class PosOperationsAppServicesFactory
    {
        public static IPosOperationsAppService Create(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var limitedFrequencyActionExecutor = serviceProvider.GetRequiredService<ILimitedFrequencyActionExecutor>();
            var logger = serviceProvider.GetRequiredService<ILogger>();
            var purchaseManager = serviceProvider.GetRequiredService<IPurchaseManager>();
            var actionExecutionFrequencyInfoFactory = new PosOperationsAppServiceActionExecutionFrequencyInfoFactory();

            return new PosOperationsAppService(
                limitedFrequencyActionExecutor: limitedFrequencyActionExecutor,
                logger: logger,
                actionExecutionFrequencyInfoFactory: actionExecutionFrequencyInfoFactory,
                purchaseManager: purchaseManager
            );
        }
    }
}