using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor
{
    public interface ISafeAsyncActionExecutorWrapperWithExceptionLogging
    {
        Task ExecuteAsync(Action action);
    }
}