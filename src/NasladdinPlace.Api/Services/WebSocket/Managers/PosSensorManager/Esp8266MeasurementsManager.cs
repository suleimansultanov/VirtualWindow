using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using NasladdinPlace.Api.Dtos.PosSensorsMeasurements;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager
{
    public class Esp8266MeasurementsManager : BaseSensorsMeasurementsManager, IPosSensorsMeasurementsManager
    {
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;
        private readonly PosSensorMeasurementsSettingsModel _posSensorMeasurementsSettingsModel;
        private readonly ILogger _logger;

        public Esp8266MeasurementsManager(
            ITelegramChannelMessageSender telegramChannelMessageSender,
            PosSensorMeasurementsSettingsModel posSensorMeasurementsSettingsModel,
            ILogger logger)
        {
            if (telegramChannelMessageSender == null)
                throw new ArgumentNullException(nameof(telegramChannelMessageSender));
            if (posSensorMeasurementsSettingsModel == null)
                throw new ArgumentNullException(nameof(posSensorMeasurementsSettingsModel));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _telegramChannelMessageSender = telegramChannelMessageSender;
            _posSensorMeasurementsSettingsModel = posSensorMeasurementsSettingsModel;
            _logger = logger;

            SubscribeOnAbnormalMeasurementsReceived();
        }

        public IEnumerable<SensorMeasurements> RearrangeMeasurements(PosSensorsMeasurementsDto posSensorsMeasurementsDto)
        {
            var measurements = posSensorsMeasurementsDto.SensorsMeasurements
                .Where(sm =>
                    sm.SensorId.HasValue && sm.Temperature.HasValue && sm.Humidity.HasValue && sm.Amperage.HasValue &&
                    sm.FrontPanelPosition.HasValue)
                .Select(sm => FromSensorPositionAsInt(
                    sm.SensorId.Value,
                    sm.Temperature.Value,
                    sm.Humidity.Value,
                    sm.Amperage.Value,
                    sm.FrontPanelPosition.Value
                ));
            return CopyValuesFromInsidePosSensorToEvaporatorAndHumidityInsidePos(measurements);
        }

        private IEnumerable<SensorMeasurements> CopyValuesFromInsidePosSensorToEvaporatorAndHumidityInsidePos(IEnumerable<SensorMeasurements> sensorsMeasurements)
        {
            if (sensorsMeasurements == null)
                throw new ArgumentNullException(nameof(sensorsMeasurements));

            var immutableSensorMeasurements = sensorsMeasurements.ToImmutableList();
            var insidePosMeasurements = immutableSensorMeasurements.FirstOrDefault(sm => sm.SensorPosition == SensorPosition.InsidePos);

            if (insidePosMeasurements == null)
                return immutableSensorMeasurements;

            var rearrangedSensorMeasurements = new List<SensorMeasurements>();

            rearrangedSensorMeasurements.AddRange(immutableSensorMeasurements);
            rearrangedSensorMeasurements.AddRange(new[]
            {
                new SensorMeasurements(SensorPosition.Evaporator, insidePosMeasurements.Temperature, insidePosMeasurements.Humidity),
                new SensorMeasurements(SensorPosition.HumidityInsidePos, insidePosMeasurements.Temperature, insidePosMeasurements.Humidity),
            });

            return rearrangedSensorMeasurements.ToImmutableList();
        }

        private void SubscribeOnAbnormalMeasurementsReceived()
        {
            OnAbnormalMeasurementsReceived += async (sender, info) =>
            {
                try
                {
                    var amperage = info.Amperage;
                    var frontPanelPosition = info.FrontPanelPosition;

                    if (amperage <= _posSensorMeasurementsSettingsModel.LowerNormalAmperage ||
                        amperage >= _posSensorMeasurementsSettingsModel.UpperNormalAmperage)
                    {
                        await _telegramChannelMessageSender.SendAsync(
                            $"{Emoji.Lightning} Сбой в сети 12В на витрине {info.PosName}. Текущее значение {amperage.ToString(CultureInfo.InvariantCulture)}A");
                    }

                    if (double.IsNaN(amperage))
                    {
                        await _telegramChannelMessageSender.SendAsync(
                            $"{Emoji.Lightning} Сбой в сети 12В на витрине {info.PosName}. Не удалось получить значение тока");
                    }

                    if (frontPanelPosition == _posSensorMeasurementsSettingsModel.FrontPanelPositionAbnormalPosition)
                    {
                        await _telegramChannelMessageSender.SendAsync(
                            $"{Emoji.Unlock} Открыта декоративная панель на витрине {info.PosName}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while reporting about abnormal data from ESP8266: {ex}");
                }
            };
        }
    }
}