using System;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models
{
    public class DiagnosticsStepResult : IDiagnosticsStepResultView
    {
        public static DiagnosticsStepResult Success(DiagnosticsStep diagnosticsStep)
        {
            return new DiagnosticsStepResult(true, diagnosticsStep, new ErrorBundle());
        }

        public static DiagnosticsStepResult Failure(DiagnosticsStep diagnosticsStep, ErrorBundle errorBundle)
        {
            if (errorBundle == null)
                throw new ArgumentNullException(nameof(errorBundle));
            
            return new DiagnosticsStepResult(false, diagnosticsStep, errorBundle);
        }
        
        public bool Succeeded { get; }
        public ErrorBundle ErrorBundle { get; }
        public DiagnosticsStep DiagnosticsStep { get; }

        private DiagnosticsStepResult(
            bool succeeded, 
            DiagnosticsStep diagnosticsStep, 
            ErrorBundle errorBundle)
        {
            if (diagnosticsStep == null)
                throw new ArgumentNullException(nameof(diagnosticsStep));

            Succeeded = succeeded;
            DiagnosticsStep = diagnosticsStep;
            ErrorBundle = errorBundle;
        }

        public Guid DiagnosticsStepId => DiagnosticsStep.Id;
        
        public DiagnosticsStepInfo DiagnosticsStepInfo => DiagnosticsStep.Info;

        public bool HasError => ErrorBundle.ContainsError();
    }
}