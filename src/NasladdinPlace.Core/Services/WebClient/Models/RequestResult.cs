using System;

namespace NasladdinPlace.Core.Services.WebClient.Models
{
    public class RequestResult<T>
    {
        public string Error { get; private set; }

        public T Result { get; private set; }

        public bool Succeeded { get; private set; }

        public static RequestResult<T> Success(T result)
        {
            return new RequestResult<T>
            {
                Succeeded = true,
                Result = result,
                Error = string.Empty
            };
        }

        public static RequestResult<T> Failure(string error)
        {
            return new RequestResult<T>
            {
                Succeeded = false,
                Error = error
            };
        }

        public static RequestResult<T> Failure(Exception exception)
        {
            return Failure(exception.Message);
        }
    }
}
