using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Doors.Contracts;
using NasladdinPlace.Core.Services.Pos.State.Contracts;
using NasladdinPlace.Core.Services.Pos.State.Models;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.State
{
    public class PosEquipmentStateManager : IPosEquipmentStateManager
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPosDoorsStateTracker _posDoorsStateTracker;
        private readonly PosStateChartSettings _posStateChartSettings;

        public PosEquipmentStateManager(
            IUnitOfWorkFactory unitOfWorkFactory,
            IPosDoorsStateTracker posDoorsStateTracker,
            IPosStateSettingsProvider posStateSettingsProvider)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (posDoorsStateTracker == null)
                throw new ArgumentNullException(nameof(posDoorsStateTracker));
            if (posStateSettingsProvider == null)
                throw new ArgumentNullException(nameof(posStateSettingsProvider));

            _unitOfWorkFactory = unitOfWorkFactory;
            _posDoorsStateTracker = posDoorsStateTracker;
            _posStateChartSettings = posStateSettingsProvider.GetChartSettings();
        }

        public IImmutableList<PosEquipmentState> GetPosStateWithinPeriod(int posId, DateTimeRange measurementsDateTimeRange)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posTemperaturesWithinPeriod =
                    unitOfWork.PosTemperatures.GetPosTemperaturesWithinPeriod(
                        posId,
                        measurementsDateTimeRange);

                var measuredTemperatures = posTemperaturesWithinPeriod.Any() ?
                    posTemperaturesWithinPeriod.OrderBy(t => t.DateCreated) :
                    CreateEmptyCollectionOfTemperaturesForPos(posId, measurementsDateTimeRange);

                var posStateForThePeriod = measuredTemperatures.Select(pt =>
                {
                    var actualPosDoorState =
                        _posDoorsStateTracker.GetPosDoorsStateActualOnDate(posId, pt.DateCreated);
                    return new PosEquipmentState(pt, actualPosDoorState.State);
                });

                return posStateForThePeriod.ToImmutableList();
            }
        }

        public PosEquipmentState GetPosStateActualOnDate(int posId, DateTime date)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var measurementsDateTimeRange =
                    DateTimeRange.From(date.Add(-_posStateChartSettings.ChartRefreshFrequency), date);
                var posTemperaturesWithinPeriod =
                    unitOfWork.PosTemperatures.GetPosTemperaturesWithinPeriod(posId, measurementsDateTimeRange);

                var actualTemperature = PosTemperature.EmptyOfPosForDate(posId, date);
                if (posTemperaturesWithinPeriod.Any())
                {
                    actualTemperature = posTemperaturesWithinPeriod.OrderByDescending(t => t.DateCreated).First();
                }

                var actualPosDoorState =
                        _posDoorsStateTracker.GetPosDoorsStateActualOnDate(posId, actualTemperature.DateCreated);

                var posEquipmentState = new PosEquipmentState(actualTemperature, actualPosDoorState.State);

                return posEquipmentState;
                
            }
        }

        private IEnumerable<PosTemperature> CreateEmptyCollectionOfTemperaturesForPos(int posId, DateTimeRange measurementsDateTimeRange)
        {
            return new List<PosTemperature>()
            {
                PosTemperature.EmptyOfPosForDate(posId, measurementsDateTimeRange.Start),
                PosTemperature.EmptyOfPosForDate(posId, measurementsDateTimeRange.End),
            };
        }
    }
}