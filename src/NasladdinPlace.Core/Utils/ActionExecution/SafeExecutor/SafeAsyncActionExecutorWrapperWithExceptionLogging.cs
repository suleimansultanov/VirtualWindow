using System;
using System.Threading.Tasks;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor
{
    public class SafeAsyncActionExecutorWrapperWithExceptionLogging : ISafeAsyncActionExecutorWrapperWithExceptionLogging
    {
        private readonly ILogger _logger;
        private readonly ISafeAsyncActionExecutor _safeAsyncActionExecutor;

        public SafeAsyncActionExecutorWrapperWithExceptionLogging(
            ILogger logger,
            ISafeAsyncActionExecutor safeAsyncActionExecutor)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (safeAsyncActionExecutor == null)
                throw new ArgumentNullException(nameof(safeAsyncActionExecutor));
            
            _logger = logger;
            _safeAsyncActionExecutor = safeAsyncActionExecutor;
        }

        public Task ExecuteAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return _safeAsyncActionExecutor.ExecuteAsync(
                action,
                executionExceptionHandler: ex =>
                {
                    _logger.LogError($"An error has occured during execution of a task. Exception: {ex}");
                });
        }
    }
}