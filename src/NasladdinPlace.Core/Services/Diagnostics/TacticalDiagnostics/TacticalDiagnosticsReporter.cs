using System.Text;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Reporter;

namespace NasladdinPlace.Core.Services.Diagnostics.TacticalDiagnostics
{
    public class TacticalDiagnosticsReporter : IDiagnosticsReporter
    {
        public Task<string> PrintReportAsync(DiagnosticsResult diagnosticsResult)
        {
            var reportStringBuilder = new StringBuilder();
            
            reportStringBuilder.AppendLine($"{Emoji.White_Check_Mark} Результаты диагностики:");

            if (diagnosticsResult.HasError)
            {
                reportStringBuilder.AppendLine(
                    "Не удалось провести диагностику из-за возникшей ошибки. " +
                    $"{diagnosticsResult.ErrorBundle.Error}");
                return Task.FromResult(reportStringBuilder.ToString());
            }
            
            var diagnosticStepResults = diagnosticsResult.DiagnosticsStepResults;

            var stepCounter = 1;
            foreach (var diagnosticsStepResult in diagnosticStepResults)
            {
                var diagnosticsStepName = diagnosticsStepResult.DiagnosticsStepInfo.Name;
                var diagnosticsStepResultMessage = diagnosticsStepResult.Succeeded 
                    ? Emoji.Thumbsup 
                    : Emoji.Thumbsdown + " " + diagnosticsStepResult.ErrorBundle.Error;
                reportStringBuilder.AppendLine($"{stepCounter}. {diagnosticsStepName} - {diagnosticsStepResultMessage}");
                ++stepCounter;
            }
            
            return Task.FromResult(reportStringBuilder.ToString());
        }
    }
}