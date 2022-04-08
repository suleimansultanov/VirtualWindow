using System;
using System.Threading.Tasks;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Doors.Contracts;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;

namespace NasladdinPlace.Core.Services.Pos.Doors
{
    public class PosDoorsStateTracker : IPosDoorsStateTracker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly PosStateHistoricalDataSettings _historicalDataSettings;

        public PosDoorsStateTracker(
            IUnitOfWorkFactory unitOfWorkFactory,
            IPosStateSettingsProvider posStateSettingsProvider)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (posStateSettingsProvider == null)
                throw new ArgumentNullException(nameof(posStateSettingsProvider));

            _unitOfWorkFactory = unitOfWorkFactory;
            _historicalDataSettings = posStateSettingsProvider.GetHistoricalDataSettings();
        }

        public async Task NotifyPosDoorsOpenedAsync(int posId, PosDoorPosition doorPosition, int posOperationId)
        {
            var state = doorPosition == PosDoorPosition.Right
                ? PosDoorsState.RightDoorOpened(posId, posOperationId)
                : PosDoorsState.LeftDoorOpened(posId, posOperationId);
            await SetStateAsync(state);
        }

        public async Task NotifyPosDoorsClosedAsync(int posId)
        {
            var state = PosDoorsState.Closed(posId);
            await SetStateAsync(state);
        }

        private async Task SetStateAsync(PosDoorsState state)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                if (IsAllowedToSetState(unitOfWork, state))
                {
                    unitOfWork.PosDoorsStates.Add(state);
                    await unitOfWork.CompleteAsync();
                }
            }
        }

        public async Task DeletePosDoorsStateHistoricalDataAsync()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var obsoleteHistoricalDoorsState =
                    unitOfWork.PosDoorsStates.GetAllDoorsStateOlderThanPeriod(_historicalDataSettings
                        .PosStateDataLifeTimePeriod);
                unitOfWork.PosDoorsStates.RemoveRange(obsoleteHistoricalDoorsState);
                await unitOfWork.CompleteAsync();
            }
        }

        public PosDoorsState GetPosDoorsStateActualOnDate(int posId, DateTime dateCreated)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posDoorState = PosDoorsState.Closed(posId);
                var posDoorsStatesOlderThanDate = unitOfWork.PosDoorsStates.GetPosDoorsStatesOlderThanDate(posId, dateCreated);

                if (posDoorsStatesOlderThanDate.Any())
                {
                    posDoorState = posDoorsStatesOlderThanDate.OrderByDescending(t => t.DateCreated).First();
                }

                return posDoorState;
            }
        }

        private bool IsAllowedToSetState(IUnitOfWork unitOfWork, PosDoorsState state)
        {
            var latestStateOfPos = unitOfWork.PosDoorsStates.GetLatestByPosId(state.PosId);
            return latestStateOfPos == null || latestStateOfPos.State != state.State;
        }
    }
}