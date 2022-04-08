using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor;

namespace NasladdinPlace.Core.Utils.ActionExecution.FrequencyLimiter
{
    public static class LimitedFrequencyActionExecutorFactory
    {
        public static ILimitedFrequencyActionExecutor Create(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var safeAsyncActionExecutor = serviceProvider.GetRequiredService<ISafeAsyncActionExecutorWrapperWithExceptionLogging>();
            var taskExecutionStartTimeUpdater = new LimitedFrequencyActionExecutionStartTimeUpdater();
            return new LimitedFrequencyActionExecutor(taskExecutionStartTimeUpdater, safeAsyncActionExecutor);
        }
    }
}