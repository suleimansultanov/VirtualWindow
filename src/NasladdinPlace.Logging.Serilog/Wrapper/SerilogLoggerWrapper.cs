using Newtonsoft.Json;
using System;
using System.Linq;

namespace NasladdinPlace.Logging.Serilog.Wrapper
{
    public class SerilogLoggerWrapper : ILogger
    {
        private readonly global::Serilog.ILogger _serilogLogger;

        public SerilogLoggerWrapper(global::Serilog.ILogger serilogLogger)
        {
            if (serilogLogger == null)
                throw new ArgumentNullException(nameof(serilogLogger));
            
            _serilogLogger = serilogLogger;
        }
        
        public void LogInfo(string message)
        {
            _serilogLogger.Information(message);
        }

        public void LogError(string message)
        {
            _serilogLogger.Error(message);
        }

        public void LogFormattedInfo(string format, params object[] parameters)
        {
            var serializedParameters = parameters.Select(p => JsonConvert.SerializeObject(parameters)).ToArray();
            var message = string.Format(format, serializedParameters);

            _serilogLogger.Information(message);
        }
    }
}