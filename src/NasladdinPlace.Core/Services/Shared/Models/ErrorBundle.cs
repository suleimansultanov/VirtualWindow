using System;

namespace NasladdinPlace.Core.Services.Shared.Models
{
    public sealed class ErrorBundle
    {
        public Exception Exception { get; }
        public string Error { get; }

        public ErrorBundle()
        {
            Exception = new NoException();
            Error = string.Empty;
        }

        public ErrorBundle(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            Exception = exception;
            Error = exception.Message;
        }

        public ErrorBundle(string error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            Exception = new NoException();
            Error = error;
        }

        public bool ContainsError()
        {
            return !string.IsNullOrWhiteSpace(Error);
        }

        public bool ContainsException()
        {
            return !(Exception is NoException);
        }

        public string StackTraceOrError => ContainsException() ? Exception.StackTrace : Error;
    }
}