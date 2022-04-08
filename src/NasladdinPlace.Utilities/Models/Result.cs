namespace NasladdinPlace.Utilities.Models
{
    public class Result
    {
        public static Result Success()
        {
            return new Result(true, string.Empty);
        }

        public static Result Failure(string error)
        {
            return new Result(false, error);
        }

        public static Result Failure()
        {
            return new Result(false, string.Empty);
        }
        
        public bool Succeeded { get; }
        public string Error { get; }

        private Result(bool succeeded, string error)
        {
            Succeeded = succeeded;
            Error = error;
        }
    }
}