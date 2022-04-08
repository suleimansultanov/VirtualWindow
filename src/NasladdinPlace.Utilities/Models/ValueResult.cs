namespace NasladdinPlace.Utilities.Models
{
    public class ValueResult<T> where T: class
    {
        public static ValueResult<T> Success(T value)
        {
            return new ValueResult<T>(true, value, string.Empty);
        }

        public static ValueResult<T> Failure(string error)
        {
            return new ValueResult<T>(false, null, error);
        }

        public static ValueResult<T> Failure()
        {
            return Failure(string.Empty);
        }
        
        public bool Succeeded { get; }
        public T Value { get; }
        public string Error { get; }

        private ValueResult(bool succeeded, T value, string error)
        {
            Succeeded = succeeded;
            Value = value;
            Error = error;
        }
    }
}