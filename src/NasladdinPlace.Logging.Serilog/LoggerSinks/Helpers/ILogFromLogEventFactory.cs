using Serilog.Events;
using Log = NasladdinPlace.Logging.Models.Log;

namespace NasladdinPlace.Logging.Serilog.LoggerSinks.Helpers
{
    public interface ILogFromLogEventFactory
    {
        Log Create(LogEvent logEvent);
    }
}