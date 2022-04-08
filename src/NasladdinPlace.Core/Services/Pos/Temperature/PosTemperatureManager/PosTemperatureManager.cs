using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;

namespace NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureManager
{
    public class PosTemperatureManager : IPosTemperatureManager
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly PosTemperatureMeasurementsSettings _temperatureMeasurementsSettings;
        private readonly PosStateHistoricalDataSettings _historicalDataSettings;

        public PosTemperatureManager(
            IUnitOfWorkFactory unitOfWorkFactory,
            IPosStateSettingsProvider posStateSettingsProvider)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (posStateSettingsProvider == null)
                throw new ArgumentNullException(nameof(posStateSettingsProvider));
            
            _unitOfWorkFactory = unitOfWorkFactory;
            _temperatureMeasurementsSettings = posStateSettingsProvider.GetTemperatureMeasurementsSettings();
            _historicalDataSettings = posStateSettingsProvider.GetHistoricalDataSettings();
        }

        public double? GetPosTemperature(int posId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var recentPosTemperatures = unitOfWork.PosTemperatures.GetLatestByPosWithinPeriod(
                    posId, _temperatureMeasurementsSettings.AverageTemperaturePeriodForComputation
                );
                
                if (!recentPosTemperatures.Any())
                    return null;

                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);
                var posAverageTemperature = unitOfWork.PosTemperatures.GetAverageByPosId(posId, _temperatureMeasurementsSettings.AverageTemperaturePeriodForComputation);
                var posCurrentTemperature = posRealTimeInfo.TemperatureInsidePos;

                return IsTemperatureWithinBounds(posCurrentTemperature) 
                    ? posCurrentTemperature 
                    : posAverageTemperature.Temperature;
            }
        }

        public async Task<IList<PosTemperature>> GetPointsOfSaleAbnormalTemperaturesAsync()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var averageTemperatures = unitOfWork.PosTemperatures.GetAverageOfPointsOfSale(_temperatureMeasurementsSettings.AverageTemperaturePeriodForComputation);

                var abnormalAverageTemperatures = averageTemperatures
                    .Where(pti => !IsTemperatureWithinBounds(pti.Temperature))
                    .ToImmutableList();

                var abnormalTemperaturesForReport = new List<PosTemperature>();
                foreach (var abnormalAverageTemperature in abnormalAverageTemperatures)
                {
                    var posId = abnormalAverageTemperature.PosId;
                    var realTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);
                    var currentPosTemperature = unitOfWork.PosTemperatures.GetLatestByPos(posId).Temperature;

                    if (IsTemperatureWithinBounds(currentPosTemperature) ||
                        await IsNotificationMutedByAdminAsync(unitOfWork, realTimeInfo)) continue;

                    var posAbnormalSensorMeasurement = new PosAbnormalSensorMeasurement(
                        posId: posId,
                        type: PosSensorType.Temperature,
                        measurementValue: abnormalAverageTemperature.Temperature,
                        measurementUnit: SensorMeasurementUnit.Celsius,
                        sensorPosition: SensorPosition.InsidePos
                    );

                    unitOfWork.PosAbnormalSensorMeasurements.Add(posAbnormalSensorMeasurement);

                    abnormalTemperaturesForReport.Add(abnormalAverageTemperature);
                }

                await unitOfWork.CompleteAsync();

                return abnormalTemperaturesForReport;
            }            
        }

        public IList<int> GetPosIdsToNotifyAboutNoTemperatureUpdates()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var pointsOfSaleIdsToNotify = unitOfWork.PosRealTimeInfos.GetConnected().
                    Where(p =>
                    {
                        var posTemperaturesForAveraging = unitOfWork.PosTemperatures.GetLatestByPosWithinPeriod(
                            p.Id, _temperatureMeasurementsSettings.AverageTemperaturePeriodForComputation);
                        var recentlyUpdatedTemperatures = unitOfWork.PosTemperatures.GetLatestByPosWithinPeriod(
                            p.Id, _temperatureMeasurementsSettings.TemperatureUpdateMaxDelay);

                        return posTemperaturesForAveraging.Any() && !recentlyUpdatedTemperatures.Any();
                    })
                    .Select(p => p.Id);

                return pointsOfSaleIdsToNotify.ToList();
            }
        }

        public async Task DeletePosTemperaturesHistoricalDataAsync()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var obsoleteHistoricalDoorsState =
                    unitOfWork.PosTemperatures.GetAllPosTemperaturesOlderThanPeriod(_historicalDataSettings.PosStateDataLifeTimePeriod);
                unitOfWork.PosTemperatures.RemoveRange(obsoleteHistoricalDoorsState);
                await unitOfWork.CompleteAsync();
            }
        }

        private async Task<bool> IsNotificationMutedByAdminAsync(IUnitOfWork unitOfWork, PosRealTimeInfo realTimeInfo)
        {   
            var lastAdminPosOperation = await unitOfWork.PosOperations.GetLastAdminPosOperationAsync(realTimeInfo.Id);
            
            if (lastAdminPosOperation == null)
                return false;

            var operationDate = lastAdminPosOperation.DateCompleted ?? lastAdminPosOperation.DateStarted;
            var nowMinusOperationDate = DateTime.UtcNow.Subtract(operationDate);
            return nowMinusOperationDate <= _temperatureMeasurementsSettings.AbnormalTemperatureAlertMutingPeriodAfterAdminPosOperation;
        }

        private bool IsTemperatureWithinBounds(double temperature)
        {
            return temperature >= _temperatureMeasurementsSettings.LowerNormalTemperature &&
                   temperature <= _temperatureMeasurementsSettings.UpperNormalTemperature;
        }
    }
}
