namespace NasladdinPlace.CloudPaymentsCore
{
    public class Response<TResult> where TResult: class
    {
        public static Response<TResult> Success(TResult result)
        {
            return new Response<TResult>(result, ResponseStatus.Success, string.Empty);
        }

        public static Response<TResult> Failure(string error = "")
        {
            return new Response<TResult>(null, ResponseStatus.Failure, error);
        }
        
        public TResult Result { get; }
        public ResponseStatus Status { get; }
        public string Error { get; }

        protected Response(TResult result, ResponseStatus status, string error)
        {
            Result = result;
            Status = status;
            Error = error;
        }

        public bool IsSuccess => Status == ResponseStatus.Success;
    }
}