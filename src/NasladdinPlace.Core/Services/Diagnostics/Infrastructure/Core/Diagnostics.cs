using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Reporter;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Runner.Factory;
using NasladdinPlace.Core.Services.Shared.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Core
{
    public class Diagnostics : IDiagnostics
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDiagnosticsRunnerFactory _diagnosticsRunnerFactory;
        private readonly IDiagnosticsReporter _diagnosticsReporter;

        public event EventHandler<DiagnosticsReport> DiagnosticsCompleted;
        
        public Diagnostics(
            IServiceProvider serviceProvider,
            IDiagnosticsRunnerFactory diagnosticsRunnerFactory,
            IDiagnosticsReporter diagnosticsReporter)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (diagnosticsRunnerFactory == null)
                throw new ArgumentNullException(nameof(diagnosticsRunnerFactory));
            if (diagnosticsReporter == null)
                throw new ArgumentNullException(nameof(diagnosticsReporter));
            
            _serviceProvider = serviceProvider;
            _diagnosticsRunnerFactory = diagnosticsRunnerFactory;
            _diagnosticsReporter = diagnosticsReporter;
        }

        public async Task RunAsync()
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                try
                {
                    await RunDiagnosticsAsync(serviceScope);
                }
                catch (Exception ex)
                {
                    await ReportDiagnosticsCompletedWithExceptionAsync(ex);
                }
            }
        }

        private async Task RunDiagnosticsAsync(IServiceScope serviceScope)
        {
            var diagnosticsRunner = _diagnosticsRunnerFactory.Create(serviceScope);

            diagnosticsRunner.DiagnosticsCompleted += HandleDiagnosticsCompletion;

            await diagnosticsRunner.RunAsync();

            diagnosticsRunner.DiagnosticsCompleted -= HandleDiagnosticsCompletion;
        }

        private async void HandleDiagnosticsCompletion(object sender, DiagnosticsResult diagnosticsResult)
        {
            var diagnosticsResultDescription = await _diagnosticsReporter.PrintReportAsync(diagnosticsResult);
            var diagnosticsReport = new DiagnosticsReport(diagnosticsResult, diagnosticsResultDescription);
            NotifyDiagnosticsCompleted(diagnosticsReport);
        }

        private void NotifyDiagnosticsCompleted(DiagnosticsReport diagnosticsReport)
        {
            DiagnosticsCompleted?.Invoke(this, diagnosticsReport);
        }

        private async Task ReportDiagnosticsCompletedWithExceptionAsync(Exception ex)
        {
            var diagnosticsResult = DiagnosticsResult.Failure(new ErrorBundle(ex));
            var diagnosticsResultDescription = await _diagnosticsReporter.PrintReportAsync(diagnosticsResult);
            var diagnosticsReport = new DiagnosticsReport(diagnosticsResult, diagnosticsResultDescription);
            NotifyDiagnosticsCompleted(diagnosticsReport);
        }
    }
}