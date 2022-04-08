using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NasladdinPlace.Core.Utils.ActionExecution.FrequencyLimiter;
using NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor;

namespace NasladdinPlace.Infra.IoC.Extensions
{
    public static class ActionExecutionUtilitiesExtensions
    {
        public static void AddActionExecutionUtilities(this IServiceCollection services)
        {
            services.AddSingleton<ISafeAsyncActionExecutor, SafeAsyncActionExecutor>();
            services.TryAddSingleton(SafeAsyncActionExecutorWithExceptionsLoggingWrapperFactory.Create);
            services.TryAddSingleton(LimitedFrequencyActionExecutorFactory.Create);
        }
    }
}