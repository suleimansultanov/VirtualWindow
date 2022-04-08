using System;
using System.Collections.Generic;
using System.Text;
using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Contracts;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureManager;
using NasladdinPlace.Core.Services.PosDiagnostics.Models;
using NasladdinPlace.Core.Utils;

namespace NasladdinPlace.Core.Services.PosDiagnostics.Report
{
    public class PosDiagnosticsReportBuilder : IPosDiagnosticsReportBuilder
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPosTemperatureManager _posTemperatureManager;
        private readonly IPosComponentsActivityManager _posComponentsActivityManager;

        public PosDiagnosticsReportBuilder(IUnitOfWorkFactory unitOfWorkFactory, IPosTemperatureManager posTemperatureManager, IPosComponentsActivityManager posComponentsActivityManager)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _posTemperatureManager = posTemperatureManager;
            _posComponentsActivityManager = posComponentsActivityManager;
        }

        public string BuildReport(List<PosDiagnosticsState> posDiagnosticStates)
        {
            var report = new StringBuilder($"Витрина | {"t ℃",-10} | Состояние{Environment.NewLine}");
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                foreach (var posDiagnosticState in posDiagnosticStates)
                {
                    var posDetails = unitOfWork.PointsOfSale.GetByIdAsync(posDiagnosticState.PosId).Result;
                    var posTemperature = _posTemperatureManager.GetPosTemperature(posDiagnosticState.PosId);
                    var strPosTemperature = posTemperature.HasValue ? posTemperature.Value.ToString("0.00") : " -";
                    var lastResponse =
                        _posComponentsActivityManager.PointsOfSale.GetLastResponse(posDiagnosticState.PosId);
                    var posState = posDiagnosticState.IsAcceptableSyncTimeDelay ? 
                        "Работает" : 
                        $"Недоступна с {DateTimeUtils.GetLastResponseTimeMessage(lastResponse)}";

                    report.AppendLine($"{posDetails.AbbreviatedName, -15}{strPosTemperature, -8}{posState}");
                }
            }

            return report.ToString();
        }

    }
}
