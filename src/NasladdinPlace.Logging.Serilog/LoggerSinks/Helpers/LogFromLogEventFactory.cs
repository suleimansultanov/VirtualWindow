using System;
using System.Text;
using NasladdinPlace.Logging.Models;
using NasladdinPlace.Logging.Serilog.Constants;
using Serilog.Events;

namespace NasladdinPlace.Logging.Serilog.LoggerSinks.Helpers
{
    public class LogFromLogEventFactory : ILogFromLogEventFactory
    {
        private readonly IFormatProvider _formatProvider;

        public LogFromLogEventFactory(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }
        
        public Log Create(LogEvent logEvent)
        {
            if (logEvent == null)
                throw new ArgumentNullException(nameof(logEvent));
            
            var logMessage = MakeMessageFromLogEvent(logEvent);
            var logLevelAsString = logEvent.Level.ToString();
            
            return new Log(level: logLevelAsString, timestamp: logEvent.Timestamp, content: logMessage);
        }

        private string MakeMessageFromLogEvent(LogEvent logEvent)
        {
            var logMessageStringBuilder = new StringBuilder();
            
            var properties = logEvent.Properties;
            
            if (properties.TryGetValue(SerilogPropertyNames.RequestId, out var requestIdPropertyValue))
            {
                logMessageStringBuilder
                    .Append("[RequestId=")
                    .Append(FormatPropertyValue(requestIdPropertyValue))
                    .Append("]");
            }
            
            if (properties.TryGetValue(SerilogPropertyNames.UserId, out var userIdPropertyValue))
            {
                logMessageStringBuilder
                    .Append("[UserId=")
                    .Append(FormatPropertyValue(userIdPropertyValue))
                    .Append("]");
            }

            logMessageStringBuilder
                .Append(" ")
                .Append(logEvent.RenderMessage(_formatProvider));

            return logMessageStringBuilder.ToString();
        }

        private static string FormatPropertyValue(LogEventPropertyValue propertyValue)
        {
            return propertyValue.ToString()
                .Replace(oldValue: "\"", newValue: string.Empty)
                .Replace(oldValue: "\"", newValue: string.Empty);
        }
    }
}