using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models
{
    public class DiagnosticsResult
    {
        public static DiagnosticsResult FromSteps(IEnumerable<IDiagnosticsStepResultView> diagnosticsStepResultViews)
        {
            if (diagnosticsStepResultViews == null)
                throw new ArgumentNullException(nameof(diagnosticsStepResultViews));

            var immutableDiagnosticsStepResultViews = diagnosticsStepResultViews.ToImmutableList();
            
            var areAllStepsSucceeded = immutableDiagnosticsStepResultViews.All(dsr => dsr.Succeeded);
            
            return new DiagnosticsResult(areAllStepsSucceeded, immutableDiagnosticsStepResultViews, new ErrorBundle());
        }

        public static DiagnosticsResult Failure(
            IEnumerable<IDiagnosticsStepResultView> diagnosticsStepResultViews,
            ErrorBundle errorBundle)
        {
            if (errorBundle == null)
                throw new ArgumentNullException(nameof(errorBundle));
            
            return new DiagnosticsResult(false, diagnosticsStepResultViews, errorBundle);
        }

        public static DiagnosticsResult Failure(ErrorBundle errorBundle)
        {
            return Failure(Enumerable.Empty<IDiagnosticsStepResultView>(), errorBundle);
        }
        
        public bool Succeeded { get; }
        public IEnumerable<IDiagnosticsStepResultView> DiagnosticsStepResults { get; }
        public ErrorBundle ErrorBundle { get; }
        
        private DiagnosticsResult(bool succeeded, IEnumerable<IDiagnosticsStepResultView> diagnosticsStepResults, ErrorBundle errorBundle)
        {
            if (diagnosticsStepResults == null)
                throw new ArgumentNullException(nameof(diagnosticsStepResults));

            Succeeded = succeeded;
            DiagnosticsStepResults = diagnosticsStepResults.ToImmutableList();
            ErrorBundle = errorBundle;
        }

        public bool HasError => ErrorBundle.ContainsError();
    }
}