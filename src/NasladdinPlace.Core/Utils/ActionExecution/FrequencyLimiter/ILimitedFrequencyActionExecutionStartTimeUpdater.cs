using NasladdinPlace.Core.Utils.ActionExecution.Models;

namespace NasladdinPlace.Core.Utils.ActionExecution.FrequencyLimiter
{
    public interface ILimitedFrequencyActionExecutionStartTimeUpdater
    {
        bool TryUpdateExecutionDateTime(ActionExecutionFrequencyInfo actionExecutionFrequencyInfo);
    }
}