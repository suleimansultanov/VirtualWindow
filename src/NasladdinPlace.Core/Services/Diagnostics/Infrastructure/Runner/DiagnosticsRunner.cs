using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;
using NasladdinPlace.Core.Services.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Runner
{
    public class DiagnosticsRunner : IDiagnosticsRunner
    {
        public event EventHandler<EventArgs> DiagnosticsStarted;
        public event EventHandler<DiagnosticsResult> DiagnosticsCompleted;
        public event EventHandler<DiagnosticsStepInfo> DiagnosticsStepStarted;
        public event EventHandler<DiagnosticsStepResult> DiagnosticsStepCompleted;
        
        private readonly IList<DiagnosticsStep> _diagnosticsSteps;
        
        public DiagnosticsRunner(params DiagnosticsStep[] diagnosticsSteps)
        {
            if (diagnosticsSteps == null)
                throw new ArgumentNullException(nameof(diagnosticsSteps));

            _diagnosticsSteps = diagnosticsSteps.ToImmutableList();
        }

        public async Task RunAsync()
        {
            var context = new DiagnosticsContext();
            
            NotifyDiagnosticsStarted();

            try
            {
                await RunDiagnosticsStepsAsync(context);
                await CleanUpResourcesAsync(context);
                NotifyDiagnosticsCompleted(DiagnosticsResult.FromSteps(context.DiagnosticsStepResults));
            }
            catch (Exception ex)
            {
                NotifyDiagnosticsCompleted(DiagnosticsResult.Failure(context.DiagnosticsStepResults, new ErrorBundle(ex)));
            }
        }

        private async Task RunDiagnosticsStepsAsync(DiagnosticsContext context)
        {   
            foreach (var diagnosticsStep in _diagnosticsSteps)
            {
                var diagnosticsStepResult = await RunDiagnosticsStepAsync(context, diagnosticsStep);

                if (!diagnosticsStepResult.Succeeded && !diagnosticsStep.ShouldContinueOnFailure) break;
            }
        }
        
        private async Task<DiagnosticsStepResult> RunDiagnosticsStepAsync(DiagnosticsContext context, DiagnosticsStep diagnosticsStep)
        {
            try
            {
                return await RunDiagnosticsStepAuxAsync(context, diagnosticsStep);
            }
            catch (Exception ex)
            {
                return DiagnosticsStepResult.Failure(diagnosticsStep, new ErrorBundle(ex));
            }
        }
        
        private async Task<DiagnosticsStepResult> RunDiagnosticsStepAuxAsync(DiagnosticsContext context, DiagnosticsStep diagnosticsStep)
        {
            NotifyDiagnosticsStepStarted(diagnosticsStep.Info);
                
            var executionResult = await diagnosticsStep.Executor.ExecuteAsync(context);

            var diagnosticsStepResult = diagnosticsStep.CreateResult(executionResult);

            if (executionResult.Succeeded)
            {
                var executionVerificationResult = 
                    await diagnosticsStep.SuccessChecker.AssertStepExecutedSuccessfullyAsync(context);
                diagnosticsStepResult = diagnosticsStep.CreateResult(executionVerificationResult);
            }

            context.AddDiagnosticsStepResult(diagnosticsStepResult);
            
            NotifyDiagnosticsStepCompleted(diagnosticsStepResult);

            return diagnosticsStepResult;
        }

        private static async Task CleanUpResourcesAsync(DiagnosticsContext context)
        {
            var executedDiagnosticsSteps = context.DiagnosticsStepResults
                .Select(dsr => dsr.DiagnosticsStep)
                .ToImmutableList()
                .Reverse();
            
            foreach (var executedDiagnosticsStep in executedDiagnosticsSteps)
            {
                await executedDiagnosticsStep.Cleaner.CleanUpAsync(context);
            }
        }

        private void NotifyDiagnosticsStarted()
        {
            DiagnosticsStarted?.Invoke(this, EventArgs.Empty);
        }

        private void NotifyDiagnosticsStepStarted(DiagnosticsStepInfo diagnosticsStepInfo)
        {
            DiagnosticsStepStarted?.Invoke(this, diagnosticsStepInfo);
        }

        private void NotifyDiagnosticsStepCompleted(DiagnosticsStepResult diagnosticsStepResult)
        {
            DiagnosticsStepCompleted?.Invoke(this, diagnosticsStepResult);
        }

        private void NotifyDiagnosticsCompleted(DiagnosticsResult diagnosticsResult)
        {
            DiagnosticsCompleted?.Invoke(this, diagnosticsResult);
        }
    }
}