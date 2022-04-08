using System;

namespace NasladdinPlace.Api.Client.Rest.RequestExecutor.Models
{
    public class RequestResponse<T> : BaseRequestResponse
    {
        public T Result { get; private set; }

        public static RequestResponse<T> Success(T result)
        {
            return new RequestResponse<T>
            {
                Result = result
            };
        }

        public static RequestResponse<T> Undefined()
        {
            return new RequestResponse<T>
            {
                Status = ResultStatus.Undefined
            };
        }

        public static RequestResponse<T> Failure(Exception exception)
        {
            return new RequestResponse<T>
            {
                Error = exception.Message,
                Exception = exception,
                Status = ResultStatus.Failure
            };
        }

        public static RequestResponse<T> Unauthorized()
        {
            return new RequestResponse<T>
            {
                Status = ResultStatus.Unauthorized
            };
        }
    }
}
