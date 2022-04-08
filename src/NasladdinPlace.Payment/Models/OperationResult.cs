namespace NasladdinPlace.Payment.Models
{
    public class OperationResult
    {
        public static OperationResult Success()
        {
            return new OperationResult(true, string.Empty);
        }

        public static OperationResult Failure(string error)
        {
            return new OperationResult(false, error);
        }
        
        public bool IsSuccessful { get; }
        public string Error { get; }

        private OperationResult(bool isSuccessful, string error)
        {
            IsSuccessful = isSuccessful;
            Error = error;
        }
    }
}