using System.Collections.Generic;
using NasladdinPlace.Core.Services.PosDiagnostics.Models;

namespace NasladdinPlace.Core.Services.PosDiagnostics.Report
{
    public interface IPosDiagnosticsReportBuilder
    {
        string BuildReport(List<PosDiagnosticsState> posDiagnosticStates);
    }
}
