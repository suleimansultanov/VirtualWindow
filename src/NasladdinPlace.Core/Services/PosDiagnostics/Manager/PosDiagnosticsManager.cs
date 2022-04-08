using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.PosDiagnostics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.PosDiagnostics.Manager
{
    public class PosDiagnosticsManager : IPosDiagnosticsManager
    {
        public event EventHandler<List<PosDiagnosticsState>> OnDiagnosticsCompleted;

        private readonly IPosInteractor _posInteractor;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly int _inventoryTimeoutInSeconds;
        private readonly bool _includeDoorsStateCheck;
        private readonly int _doorStateTimeoutInSeconds;

        public PosDiagnosticsManager(
            IPosInteractor posInteractor,
            IUnitOfWorkFactory unitOfWorkFactory,
            int inventoryTimeoutInSeconds,
            bool includeDoorsStateCheck,
            int doorStateTimeoutInSeconds)
        {
            _posInteractor = posInteractor;
            _unitOfWorkFactory = unitOfWorkFactory;
            _inventoryTimeoutInSeconds = inventoryTimeoutInSeconds;
            _includeDoorsStateCheck = includeDoorsStateCheck;
            _doorStateTimeoutInSeconds = doorStateTimeoutInSeconds;
        }

        public async Task RunPosDiagnosticsAsync()
        {
            var posDiagnosticsStates = new List<PosDiagnosticsState>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var connectedPoses = unitOfWork.PosRealTimeInfos.GetAll()
                    .Where(p => p.ConnectionStatus == PosConnectionStatus.Connected &&
                                p.PosActivityStatus == PosActivityStatus.Active).ToList();

                posDiagnosticsStates.AddRange(connectedPoses.Select(c => new PosDiagnosticsState(c.Id)).ToList());

                await RequestContentSync(unitOfWork, connectedPoses, posDiagnosticsStates);

                if (_includeDoorsStateCheck)
                    await RequestDoorsStateSync(unitOfWork, connectedPoses, posDiagnosticsStates);

                PerformOnDiagnosticsCompleted(posDiagnosticsStates);
            }
        }

        private async Task RequestContentSync(IUnitOfWork unitOfWork, IEnumerable<PosRealTimeInfo> connectedPoses,
            IEnumerable<PosDiagnosticsState> posDiagnosticStates)
        {
            foreach (var posRealTimeInfo in connectedPoses)
                await _posInteractor.RequestAccountingBalancesAsync(posRealTimeInfo.Id);

            Thread.Sleep(TimeSpan.FromSeconds(_inventoryTimeoutInSeconds));

            foreach (var posDiagnosticState in posDiagnosticStates)
            {
                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posDiagnosticState.PosId);
                if (posRealTimeInfo.ContentSyncDateTime > posDiagnosticState.ContentSyncRequestDateTime)
                    posDiagnosticState.MarkAsAcceptableSyncTimeDelay();
            }
        }

        private async Task RequestDoorsStateSync(IUnitOfWork unitOfWork, IEnumerable<PosRealTimeInfo> connectedPoses, IEnumerable<PosDiagnosticsState> posDiagnosticStates)
        {
            foreach (var posRealTimeInfo in connectedPoses)
                await _posInteractor.RequestDoorsStateAsync(posRealTimeInfo.Id);

            Thread.Sleep(TimeSpan.FromSeconds(_doorStateTimeoutInSeconds));

            foreach (var posDiagnosticState in posDiagnosticStates)
            {
                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posDiagnosticState.PosId);
                if (posRealTimeInfo.DoorsStateSyncDateTime < posDiagnosticState.DoorsStateSyncRequestDateTime)
                    posDiagnosticState.MarkAsUnacceptableSyncTimeDelay();
            }
        }

        private void PerformOnDiagnosticsCompleted(List<PosDiagnosticsState> posDiagnosticStates)
        {
            if (!posDiagnosticStates.Any())
                return;

            OnDiagnosticsCompleted?.Invoke(this, posDiagnosticStates);
        }
    }
}