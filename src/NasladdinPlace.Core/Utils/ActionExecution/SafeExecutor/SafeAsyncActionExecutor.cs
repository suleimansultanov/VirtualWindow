using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor
{
    public class SafeAsyncActionExecutor : ISafeAsyncActionExecutor
    {
        public Task ExecuteAsync(Action action, Action<Exception> executionExceptionHandler)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (executionExceptionHandler == null)
                throw new ArgumentNullException(nameof(executionExceptionHandler));
            
            return Task.Run(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (AggregateException ex)
                {
                    ex.Handle((x) =>
                    {
                        executionExceptionHandler.Invoke(x);
                        return true;
                    });
                }
                catch (Exception ex)
                {
                    executionExceptionHandler.Invoke(ex);
                }
            });
        }

        public Task ExecuteAsync(Func<Task> func, Action<Exception> executionExceptionHandler)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            if (executionExceptionHandler == null)
                throw new ArgumentNullException(nameof(executionExceptionHandler));
            
            return Task.Run(async () =>
            {
                try
                {
                    await func();
                }
                catch (AggregateException ex)
                {
                    ex.Handle((x) =>
                    {
                        executionExceptionHandler.Invoke(x);
                        return true;
                    });
                }
                catch (Exception ex)
                {
                    executionExceptionHandler.Invoke(ex);
                }
            });
        }

        public Task ExecuteAsync(Action action)
        {
            return ExecuteAsync(action, executionExceptionHandler: _ => {});
        }
    }
}