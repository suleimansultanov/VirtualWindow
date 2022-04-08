using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models
{
    public class DiagnosticsContext
    {
        public ApplicationUser User { get; set; }
        public PosOperation PosOperation { get; set; }
        
        public IList<DiagnosticsStepResult> DiagnosticsStepResults { get; }

        public DiagnosticsContext()
        {
            DiagnosticsStepResults = new List<DiagnosticsStepResult>();
        }

        public void AddDiagnosticsStepResult(DiagnosticsStepResult diagnosticsStepResult)
        {
            DiagnosticsStepResults.Add(diagnosticsStepResult);
        }
    }
}