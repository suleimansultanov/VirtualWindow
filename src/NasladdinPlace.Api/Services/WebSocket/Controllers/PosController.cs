using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos;
using NasladdinPlace.Api.Dtos.PosAntennasOutputPower;
using NasladdinPlace.Api.Dtos.PosDoorsState;
using NasladdinPlace.Api.Dtos.PosSensorsMeasurements;
using NasladdinPlace.Api.Services.WebSocket.Factories.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Pos.Sensors.Checker;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Api.Services.WebSocket.Controllers
{
    public class PosController : WsController
    {
        private readonly ILogger _logger;
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;
        private readonly IPosSensorsMeasurementsManagerFactory _posSensorsMeasurementsManagerFactory;
        private readonly IPosSensorControllerTypeConfigurationProvider _posSensorControllerTypeConfigurationProvider;
        private readonly IPosSensorControllerMeasurementsTracker _posSensorControllerMeasurementsTracker;

        public PosController(
            IUnitOfWorkFactory unitOfWorkFactory,
            ITelegramChannelMessageSender telegramChannelMessageSender,
            IPosSensorsMeasurementsManagerFactory posSensorsMeasurementsManagerFactory,
            IPosSensorControllerTypeConfigurationProvider posSensorControllerTypeConfigurationProvider,
            IPosSensorControllerMeasurementsTracker posSensorControllerMeasurementsTracker,
            ILogger logger) : base(unitOfWorkFactory)
        {
            _logger = logger;
            _telegramChannelMessageSender = telegramChannelMessageSender;
            _posSensorsMeasurementsManagerFactory = posSensorsMeasurementsManagerFactory;
            _posSensorControllerTypeConfigurationProvider = posSensorControllerTypeConfigurationProvider;
            _posSensorControllerMeasurementsTracker = posSensorControllerMeasurementsTracker;
        }

        public async Task UpdateDoorsState(PosDoorsStateDto posDoorsStateDto)
        {
            await ExecuteAsync(async unitOfWork =>
            {
                _logger.LogInfo(
                    $"POS {posDoorsStateDto.PosId.Value} doors state is {posDoorsStateDto.DoorsState.ToString()}.");

                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posDoorsStateDto.PosId.Value);
                posRealTimeInfo.DoorsState = posDoorsStateDto.DoorsState;
                posRealTimeInfo.DoorsStateSyncDateTime = DateTime.UtcNow;
                await unitOfWork.CompleteAsync();
            });
        }

        public Task UpdateSensorsMeasurements(PosSensorsMeasurementsDto posSensorsMeasurementsDto)
        {
            if (posSensorsMeasurementsDto == null)
                throw new ArgumentNullException(nameof(posSensorsMeasurementsDto));

            return UpdateSensorsMeasurementsAux(posSensorsMeasurementsDto);
        }

        private async Task UpdateSensorsMeasurementsAux(PosSensorsMeasurementsDto posSensorsMeasurementsDto)
        {
            if (!posSensorsMeasurementsDto.PosId.HasValue) return;

            var sensorsMeasurementsDto = posSensorsMeasurementsDto.SensorsMeasurements;
            if (!sensorsMeasurementsDto.Any()) return;

            var posId = posSensorsMeasurementsDto.PosId.Value;

            await ExecuteAsync(async unitOfWork =>
            {
                var pointOfSale = await unitOfWork.PointsOfSale.GetByIdAsync(posId);

                if (pointOfSale == null)
                    throw new ArgumentException($"Could not find a pos by id {posId}");

                var controllerType =
                    _posSensorControllerTypeConfigurationProvider
                        .DefineAndCacheTypeOfSensorControllerAsync(pointOfSale);
                var measurementsManager = _posSensorsMeasurementsManagerFactory.Create(controllerType);
                var rearrangedMeasurements = measurementsManager.RearrangeMeasurements(posSensorsMeasurementsDto);
                await _posSensorControllerMeasurementsTracker.TrackAsync(posId, unitOfWork, rearrangedMeasurements);
                var measurementsNotificationsInfo = new MeasurementsNotificationInfo(rearrangedMeasurements, pointOfSale.AbbreviatedName);
                measurementsManager.NotifyAboutAbnormalValues(measurementsNotificationsInfo, pointOfSale);
            });
        }

        public async Task UpdateAntennasOutputPower(PosAntennasOutputPowerDto posAntennasOutputPowerDto)
        {
            if (!posAntennasOutputPowerDto.OutputPower.HasValue || !posAntennasOutputPowerDto.PosId.HasValue)
                return;

            await ExecuteAsync(async unitOfWork =>
            {
                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posAntennasOutputPowerDto.PosId.Value);

                posRealTimeInfo.AntennasOutputPower = posAntennasOutputPowerDto.OutputPower.Value;

                await unitOfWork.CompleteAsync();
            });
        }

        public async Task NotifyAboutFailure(PosFailureInfoDto posFailureInfo)
        {
            if (!posFailureInfo.PosId.HasValue) return;
            
            await ExecuteAsync(async unitOfWork =>
            {
                var posId = posFailureInfo.PosId.Value;
                var pos = await unitOfWork.PointsOfSale.GetByIdAsync(posId);

                if (pos == null)
                    throw new ArgumentException($"Couldn't notify about failure because could not find a pos by id {posId}");

                if (pos.AreNotificationsEnabled)
                    await _telegramChannelMessageSender.SendAsync(
                        $"{Emoji.Sos} Произошел сбой на витрине {pos?.AbbreviatedName}. " +
                        $"Причина: {posFailureInfo.Cause}");
            });
        }
    }
}