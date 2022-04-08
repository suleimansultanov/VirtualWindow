using System;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models
{
    public class DiagnosticsReport
    {
        public DiagnosticsResult DiagnosticsResult { get; }
        public string DiagnosticsResultDescription { get; }

        public DiagnosticsReport(DiagnosticsResult diagnosticsResult, string diagnosticsResultDescription)
        {
            if (diagnosticsResult == null)
                throw new ArgumentNullException(nameof(diagnosticsResult));
            if (string.IsNullOrWhiteSpace(diagnosticsResultDescription))
                throw new ArgumentNullException(nameof(diagnosticsResultDescription));

            DiagnosticsResult = diagnosticsResult;
            DiagnosticsResultDescription = diagnosticsResultDescription;
        }
    }
}