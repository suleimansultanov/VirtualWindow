using System;
using System.Text;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using Serilog;

namespace NasladdinPlace.Api.Services.Diagnostics
{
    public class TacticalDiagnosticsResultLogger 
    {
        private readonly ILogger _logger;

        public TacticalDiagnosticsResultLogger(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            
            _logger = logger;
        }

        public void Log(DiagnosticsResult diagnosticsResult) 
        {
            if (diagnosticsResult == null)
                throw new ArgumentNullException(nameof(diagnosticsResult));

            if (diagnosticsResult.Succeeded) return;

            var diagnosticsFailureMessage = new StringBuilder();

            diagnosticsFailureMessage.AppendLine("Tactical diagnostics has failed because: ");

            var errorsCounter = 1;
            if (diagnosticsResult.HasError)
            {
                diagnosticsFailureMessage.AppendLine($"{errorsCounter}. {diagnosticsResult.ErrorBundle.StackTraceOrError}.");
                errorsCounter++;
            }
            
            foreach (var stepResult in diagnosticsResult.DiagnosticsStepResults)
            {
                if (stepResult.Succeeded) continue;

                var stepName = stepResult.DiagnosticsStepInfo.Name;
                var errorBundle = stepResult.ErrorBundle;
                diagnosticsFailureMessage.AppendLine($"{errorsCounter}. {stepName}. {errorBundle.StackTraceOrError}");
            }
            
            _logger.Error(diagnosticsFailureMessage.ToString());
        }
    }
}