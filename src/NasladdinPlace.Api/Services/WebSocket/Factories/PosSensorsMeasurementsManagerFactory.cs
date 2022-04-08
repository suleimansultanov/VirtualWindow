using System;
using NasladdinPlace.Api.Services.WebSocket.Factories.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Api.Services.WebSocket.Factories
{
    public class PosSensorsMeasurementsManagerFactory : IPosSensorsMeasurementsManagerFactory
    {
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;
        private readonly PosSensorMeasurementsSettingsModel _posSensorMeasurementsSettingsModel;
        private readonly ILogger _logger;

        public PosSensorsMeasurementsManagerFactory(
            ITelegramChannelMessageSender telegramChannelMessageSender,
            PosSensorMeasurementsSettingsModel posSensorMeasurementsSettingsModel,
            ILogger logger)
        {
            if(telegramChannelMessageSender == null)
                throw new ArgumentNullException(nameof(telegramChannelMessageSender));
            if(posSensorMeasurementsSettingsModel == null)
                throw new ArgumentNullException(nameof(posSensorMeasurementsSettingsModel));
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            _posSensorMeasurementsSettingsModel = posSensorMeasurementsSettingsModel;
            _logger = logger;
            _telegramChannelMessageSender = telegramChannelMessageSender;
        }

        public IPosSensorsMeasurementsManager Create(SensorControllerType sensorControllerType)
        {
            switch (sensorControllerType)
            {
                case SensorControllerType.Esp:
                    return new Esp8266MeasurementsManager(_telegramChannelMessageSender, _posSensorMeasurementsSettingsModel, _logger);
                case SensorControllerType.Legacy:
                    return new LegacySensorsMeasurementsManager();
                default:
                case SensorControllerType.New:
                    return new Hy6002MeasurementsManager();
            }
        }
    }
}