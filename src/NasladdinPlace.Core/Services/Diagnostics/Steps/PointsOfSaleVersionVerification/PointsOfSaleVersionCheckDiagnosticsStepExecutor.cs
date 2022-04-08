using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor;
using NasladdinPlace.Core.Services.Pos.Version.Contracts;
using NasladdinPlace.Core.Services.Pos.Version.Models;
using NasladdinPlace.Core.Services.Printers.Common;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PointsOfSaleVersionVerification
{
    public class PointsOfSaleVersionCheckDiagnosticsStepExecutor : IDiagnosticsStepExecutor
    {
        private readonly IPointsOfSaleVersionUpdateChecker _pointsOfSaleVersionUpdateChecker;
        private readonly IMessagePrinter<PointsOfSaleVersionUpdateInfo> _pointsOfSaleVersionUpdateInfoPrinter;

        public PointsOfSaleVersionCheckDiagnosticsStepExecutor(
            IPointsOfSaleVersionUpdateChecker pointsOfSaleVersionUpdateChecker,
            IMessagePrinter<PointsOfSaleVersionUpdateInfo> pointsOfSaleVersionUpdateInfoPrinter)
        {
            if (pointsOfSaleVersionUpdateChecker == null)
                throw new ArgumentNullException(nameof(pointsOfSaleVersionUpdateChecker));
            if (pointsOfSaleVersionUpdateInfoPrinter == null)
                throw new ArgumentNullException(nameof(pointsOfSaleVersionUpdateInfoPrinter));

            _pointsOfSaleVersionUpdateChecker = pointsOfSaleVersionUpdateChecker;
            _pointsOfSaleVersionUpdateInfoPrinter = pointsOfSaleVersionUpdateInfoPrinter;
        }

        public async Task<Result> ExecuteAsync(DiagnosticsContext context)
        {
            var pointsOfSalesVersionUpdateInfo =
                await _pointsOfSaleVersionUpdateChecker.GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync();

            if (!pointsOfSalesVersionUpdateInfo.IsUpdateRequired)
                return Result.Success();

            var pointsOfSaleVersionUpdateInfoMessage =
                _pointsOfSaleVersionUpdateInfoPrinter.Print(pointsOfSalesVersionUpdateInfo);

            return Result.Failure(pointsOfSaleVersionUpdateInfoMessage);
        }
    }
}