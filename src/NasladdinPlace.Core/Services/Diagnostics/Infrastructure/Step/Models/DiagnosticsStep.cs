using System;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation;
using NasladdinPlace.Core.Services.Shared.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models
{
    public class DiagnosticsStep
    {
        public Guid Id { get; }
        public DiagnosticsStepInfo Info { get; }
        public IDiagnosticsStepPreparer Preparer { get; }
        public IDiagnosticsStepExecutor Executor { get; }
        public IDiagnosticsStepSuccessChecker SuccessChecker { get; }
        public IDiagnosticsStepCleaner Cleaner { get; }
        public bool ShouldContinueOnFailure { get; set; }

        public DiagnosticsStep(
            DiagnosticsStepInfo diagnosticsStepInfo,
            IDiagnosticsStepPreparer diagnosticsStepPreparer,
            IDiagnosticsStepExecutor diagnosticsStepExecutor,
            IDiagnosticsStepSuccessChecker diagnosticsStepSuccessChecker,
            IDiagnosticsStepCleaner diagnosticsStepCleaner)
        {
            if (diagnosticsStepInfo == null)
                throw new ArgumentNullException(nameof(diagnosticsStepInfo));
            if (diagnosticsStepPreparer == null)
                throw new ArgumentNullException(nameof(diagnosticsStepPreparer));
            if (diagnosticsStepExecutor == null)
                throw new ArgumentNullException(nameof(diagnosticsStepExecutor));
            if (diagnosticsStepSuccessChecker == null)
                throw new ArgumentNullException(nameof(diagnosticsStepSuccessChecker));
            if (diagnosticsStepCleaner == null)
                throw new ArgumentNullException(nameof(diagnosticsStepCleaner));

            Id = Guid.NewGuid();
            Info = diagnosticsStepInfo;
            Preparer = diagnosticsStepPreparer;
            Executor = diagnosticsStepExecutor;
            SuccessChecker = diagnosticsStepSuccessChecker;
            Cleaner = diagnosticsStepCleaner;
        }

        public DiagnosticsStepResult CreateResult(Result result)
        {
            return result.Succeeded 
                ? DiagnosticsStepResult.Success(this) 
                : DiagnosticsStepResult.Failure(this, new ErrorBundle(result.Error));
        }
    }
}