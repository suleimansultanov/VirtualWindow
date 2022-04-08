using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor
{
    public interface ISafeAsyncActionExecutor
    {
        Task ExecuteAsync(Action action, Action<Exception> executionExceptionHandler);
        Task ExecuteAsync(Func<Task> func, Action<Exception> executionExceptionHandler);
        Task ExecuteAsync(Action action);
    }
}