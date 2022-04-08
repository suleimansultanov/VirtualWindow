using NasladdinPlace.Logging.Serilog.LoggerSinks.Helpers;
using NasladdinPlace.Logging.Writers;
using Serilog;
using Serilog.Configuration;

namespace NasladdinPlace.Logging.Serilog.LoggerSinks
{
    public static class CustomSinkExtensions
    {
        public static LoggerConfiguration CustomSink(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            ILogWriter logWriter,
            ILogFromLogEventFactory logFromLogEventFactory)
        {
            return loggerSinkConfiguration.Sink(new CustomSink(logWriter, logFromLogEventFactory));
        }
    }
}
