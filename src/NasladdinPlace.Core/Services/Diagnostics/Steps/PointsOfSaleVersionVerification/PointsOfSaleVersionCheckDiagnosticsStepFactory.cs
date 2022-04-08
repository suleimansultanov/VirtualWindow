using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Configuration.Manager.Contracts;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Preparation;
using NasladdinPlace.Core.Services.Pos.Version;
using NasladdinPlace.Core.Services.Pos.Version.Printer;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PointsOfSaleVersionVerification
{
    public class PointsOfSaleVersionCheckDiagnosticsStepFactory : BaseDiagnosticsStepFactory
    {
        public PointsOfSaleVersionCheckDiagnosticsStepFactory(IServiceScope serviceScope) : base(serviceScope)
        {
        }

        public override DiagnosticsStep Create()
        {         
            var pointsOfSaleVersionCheckStepInfo = new PointsOfSaleVersionCheckStepInfo();
            var pointsOfSaleVersionCheckStepPreparer = new NoDiagnosticsStepPreparation();

            var pointsOfSaleVersionUpdateChecker = new PointsOfSaleVersionUpdateChecker(
                GetRequiredService<IConfigurationManager>(),
                GetRequiredService<IUnitOfWorkFactory>()
            );     

            var pointsOfSaleVersionUpdateMessagePrinter = new PointsOfSaleVersionUpdateInfoEnglishPrinter();
            var pointsOfSaleVersionCheckStepExecutor = new PointsOfSaleVersionCheckDiagnosticsStepExecutor(
                pointsOfSaleVersionUpdateChecker,
                pointsOfSaleVersionUpdateMessagePrinter
            );
            
            var pointsOfSaleVersionCheckStepChecker = new NoDiagnosticsStepChecker();
            var pointsOfSaleVersionCheckStepCleaner = new NoDiagnosticsStepCleaner();

            return new DiagnosticsStep(
                pointsOfSaleVersionCheckStepInfo,
                pointsOfSaleVersionCheckStepPreparer,
                pointsOfSaleVersionCheckStepExecutor,
                pointsOfSaleVersionCheckStepChecker,
                pointsOfSaleVersionCheckStepCleaner
            )
            {
                ShouldContinueOnFailure = true
            };
        }
    }
}