using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor
{
    public static class SafeAsyncActionExecutorWithExceptionsLoggingWrapperFactory
    {
        public static ISafeAsyncActionExecutorWrapperWithExceptionLogging Create(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            
            var logger = serviceProvider.GetRequiredService<ILogger>();
            var taskSafeExecutor = serviceProvider.GetRequiredService<ISafeAsyncActionExecutor>();
            return new SafeAsyncActionExecutorWrapperWithExceptionLogging(logger, taskSafeExecutor);
        }
    }
}