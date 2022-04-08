using System.Text;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Reporter;

namespace NasladdinPlace.Core.Services.Diagnostics.TacticalDiagnostics
{
    public class TacticalDiagnosticsFailuresReporter : IDiagnosticsReporter
    {
        public Task<string> PrintReportAsync(DiagnosticsResult diagnosticsResult)
        {
            if (diagnosticsResult.Succeeded)
            {
                return Task.FromResult($"{Emoji.White_Check_Mark} Тактическая диагностика проведена успешно.");
            }
            
            var tacticalDiagnosticsReport = new StringBuilder();
            
            tacticalDiagnosticsReport.AppendLine($"{Emoji.Negative_Squared_Cross_Mark} Тактическая диагностика проведена c ошибками: ");

            var errorsOrderNumber = 1;

            if (diagnosticsResult.HasError)
            {
                tacticalDiagnosticsReport.AppendLine($"{errorsOrderNumber}. Общая ошибка: {diagnosticsResult.ErrorBundle.Error}.");
                ++errorsOrderNumber;
            }
            
            foreach (var diagnosticsStepResult in diagnosticsResult.DiagnosticsStepResults)
            {
                if (!diagnosticsStepResult.HasError) continue;

                var stepName = diagnosticsStepResult.DiagnosticsStepInfo.Name;
                var error = diagnosticsStepResult.ErrorBundle.Error;
                tacticalDiagnosticsReport.AppendLine($"{errorsOrderNumber}. {stepName}: {error}");
                ++errorsOrderNumber;
            }
            
            return Task.FromResult(tacticalDiagnosticsReport.ToString());
        }
    }
}