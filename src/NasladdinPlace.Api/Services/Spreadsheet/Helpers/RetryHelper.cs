using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.Spreadsheet.Helpers
{
    public static class RetryHelper
    {
        public static Task RetryOnExceptionAsync(TimeSpan delayBetweenAttempts, Func<Task> action,
            Predicate<Exception> retryOnExceptionPredicate, int permittedRetryCount)
        {
            if (permittedRetryCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(permittedRetryCount));

            return RetryOnExceptionAuxAsync(delayBetweenAttempts, action, retryOnExceptionPredicate, permittedRetryCount);
        }

        private static async Task RetryOnExceptionAuxAsync<TException>(TimeSpan delayBetweenAttempts, Func<Task> action,
            Predicate<TException> retryOnExceptionPredicate,
            int permittedRetryCount) where TException : Exception
        {
            var attempts = 0;
            do
            {
                try
                {
                    await action();
                    break;
                }
                catch (TException ex)
                {
                    attempts++;
                    var canRetry = attempts < permittedRetryCount &&
                                   (retryOnExceptionPredicate == null || retryOnExceptionPredicate(ex));

                    if (!canRetry)
                        throw;

                    await Task.Delay(delayBetweenAttempts);
                }
            } while (true);
        }
    }
}