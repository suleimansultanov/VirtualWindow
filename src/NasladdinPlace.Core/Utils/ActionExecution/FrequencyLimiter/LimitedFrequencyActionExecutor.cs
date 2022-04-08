using System;
using NasladdinPlace.Core.Utils.ActionExecution.Models;
using NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor;

namespace NasladdinPlace.Core.Utils.ActionExecution.FrequencyLimiter
{
    public class LimitedFrequencyActionExecutor : ILimitedFrequencyActionExecutor
    {
        private readonly ILimitedFrequencyActionExecutionStartTimeUpdater _actionExecutionStartTimeUpdater;
        private readonly ISafeAsyncActionExecutorWrapperWithExceptionLogging _safeAsyncActionExecutor;

        public LimitedFrequencyActionExecutor(
            ILimitedFrequencyActionExecutionStartTimeUpdater actionExecutionStartTimeUpdater,
            ISafeAsyncActionExecutorWrapperWithExceptionLogging safeAsyncActionExecutor)
        {
            if (actionExecutionStartTimeUpdater == null)
                throw new ArgumentNullException(nameof(actionExecutionStartTimeUpdater));
            if (safeAsyncActionExecutor == null)
                throw new ArgumentNullException(nameof(safeAsyncActionExecutor));
            
            _actionExecutionStartTimeUpdater = actionExecutionStartTimeUpdater;
            _safeAsyncActionExecutor = safeAsyncActionExecutor;
        }
        
        public bool TryExecute(Action action, ActionExecutionFrequencyInfo actionExecutionFrequencyInfo)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (actionExecutionFrequencyInfo == null)
                throw new ArgumentNullException(nameof(actionExecutionFrequencyInfo));

            if (_actionExecutionStartTimeUpdater.TryUpdateExecutionDateTime(actionExecutionFrequencyInfo))
            {
                action();
            }

            return true;
        }

        public bool TryExecuteAsync(Action action, ActionExecutionFrequencyInfo actionExecutionFrequencyInfo)
        {
            return TryExecute(() => { _safeAsyncActionExecutor.ExecuteAsync(action); }, actionExecutionFrequencyInfo);
        }
    }
}