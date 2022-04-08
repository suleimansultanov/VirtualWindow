using System;

namespace NasladdinPlace.Api.Client.Rest.RequestExecutor.Models
{
    public class BaseRequestResponse
    {
        public ResultStatus Status { get; protected set; }
        public Exception Exception { get; protected set; }
        public string Error { get; protected set; }

        protected BaseRequestResponse()
        {
            Status = ResultStatus.Success;
            Exception = new Exception();
            Error = string.Empty;
        }

        public bool IsRequestSuccessful => Status == ResultStatus.Success;
    }
}