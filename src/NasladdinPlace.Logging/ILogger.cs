namespace NasladdinPlace.Logging
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(string message);
        void LogFormattedInfo(string format, params object[] parameters);
    }
}