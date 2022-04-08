using System;
using NasladdinPlace.Core.Utils.ActionExecution.Models;

namespace NasladdinPlace.Core.Utils.ActionExecution.FrequencyLimiter
{
    public interface ILimitedFrequencyActionExecutor
    {
        bool TryExecute(Action action, ActionExecutionFrequencyInfo actionExecutionFrequencyInfo);
        bool TryExecuteAsync(Action action, ActionExecutionFrequencyInfo actionExecutionFrequencyInfo);
    }
}