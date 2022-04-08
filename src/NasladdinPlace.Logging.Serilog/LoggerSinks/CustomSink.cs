using System;
using NasladdinPlace.Logging.Serilog.LoggerSinks.Helpers;
using NasladdinPlace.Logging.Writers;
using Serilog.Core;
using Serilog.Events;

namespace NasladdinPlace.Logging.Serilog.LoggerSinks
{
    public class CustomSink : ILogEventSink
    {
        private readonly ILogWriter _logWriter;
        private readonly ILogFromLogEventFactory _logFromLogEventFactory;

        public CustomSink(
            ILogWriter logWriter,
            ILogFromLogEventFactory logFromLogEventFactory)
        {
            if (logWriter == null)
                throw new ArgumentNullException(nameof(logWriter));
            if (logFromLogEventFactory == null)
                throw new ArgumentNullException(nameof(logFromLogEventFactory));
            
            _logWriter = logWriter;
            _logFromLogEventFactory = logFromLogEventFactory;
        }

        public void Emit(LogEvent logEvent)
        {
            var log = _logFromLogEventFactory.Create(logEvent);
            _logWriter.Write(log);
        }
    }
}
