using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Executor;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Contracts;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Models;
using NasladdinPlace.Core.Services.Printers.Common;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.PosScreenResolutionVerification
{
    public class PosScreenResolutionCheckDiagnosticsStepExecutor : IDiagnosticsStepExecutor
    {
        private readonly IPosScreenResolutionChecker _posScreenResolutionChecker;
        private readonly IMessagePrinter<IEnumerable<PosScreenResolutionInfo>> _pointsOfSaleScreenResolutionPrinter;

        public PosScreenResolutionCheckDiagnosticsStepExecutor(
            IPosScreenResolutionChecker posScreenResolutionChecker,
            IMessagePrinter<IEnumerable<PosScreenResolutionInfo>> pointsOfSaleScreenResolutionPrinter)
        {
            if (posScreenResolutionChecker == null)
                throw new ArgumentNullException(nameof(posScreenResolutionChecker));
            if (pointsOfSaleScreenResolutionPrinter == null)
                throw new ArgumentNullException(nameof(pointsOfSaleScreenResolutionPrinter));

            _posScreenResolutionChecker = posScreenResolutionChecker;
            _pointsOfSaleScreenResolutionPrinter = pointsOfSaleScreenResolutionPrinter;
        }

        public async Task<Result> ExecuteAsync(DiagnosticsContext context)
        {
            var posScreenResolutionInfos =
                await _posScreenResolutionChecker.GetPointsOfSaleWithIncorrectScreenResolutionAsync();

            if (!posScreenResolutionInfos.Any())
                return Result.Success();

            var pointsOfSaleWithWrongScreenResolutionMessage =
                _pointsOfSaleScreenResolutionPrinter.Print(posScreenResolutionInfos);

            return Result.Failure(pointsOfSaleWithWrongScreenResolutionMessage);
        }
    }
}