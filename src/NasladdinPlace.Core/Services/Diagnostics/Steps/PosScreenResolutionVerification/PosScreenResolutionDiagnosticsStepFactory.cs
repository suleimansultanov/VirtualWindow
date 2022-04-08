using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Manager.Contracts;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation;
using NasladdinPlace.Core.Services.Pos.ScreenResolution;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Printer;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PosScreenResolutionVerification
{
    public class PosScreenResolutionDiagnosticsStepFactory : BaseDiagnosticsStepFactory
    {
        public PosScreenResolutionDiagnosticsStepFactory(IServiceScope serviceScope) : base(serviceScope)
        {
        }

        public override DiagnosticsStep Create()
        {
            var pointsOfSaleScreenResolutionCheckStepInfo = new PosScreenResolutionDiagnosticsStepInfo();
            var pointsOfSaleScreenResolutionCheckStepPreparer = new NoDiagnosticsStepPreparation();

            var pointsOfSaleScreenResolutionChecker =
                new PosScreenResolutionChecker(GetRequiredService<IUnitOfWorkFactory>(), GetRequiredService<IConfigurationManager>());
            var posScreenResolutionInfoEnglishPrinter = new IncorrectPosScreenResolutionInfoEnglishPrinter();

            var pointsOfSaleScreenResolutionCheckStepExecutor =
                new PosScreenResolutionCheckDiagnosticsStepExecutor(pointsOfSaleScreenResolutionChecker,
                    posScreenResolutionInfoEnglishPrinter);

            var pointsOfSaleScreenResolutionCheckStepChecker = new NoDiagnosticsStepChecker();
            var pointsOfSaleScreenResolutionCheckStepCleaner = new NoDiagnosticsStepCleaner();

            return new DiagnosticsStep(
                pointsOfSaleScreenResolutionCheckStepInfo,
                pointsOfSaleScreenResolutionCheckStepPreparer,
                pointsOfSaleScreenResolutionCheckStepExecutor,
                pointsOfSaleScreenResolutionCheckStepChecker,
                pointsOfSaleScreenResolutionCheckStepCleaner
            )
            {
                ShouldContinueOnFailure = true
            };
        }
    }
}