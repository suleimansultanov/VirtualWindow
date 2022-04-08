using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Dtos.PosAntennasOutputPower;
using NasladdinPlace.Api.Dtos.PosSensorsMeasurements;
using NasladdinPlace.Api.Services.WebSocket.Controllers;
using NasladdinPlace.Api.Services.WebSocket.Factories.Contracts;
using NasladdinPlace.Api.Services.WebSocket.Managers.PosSensorManager.Contracts;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Pos.Sensors.Checker;
using NasladdinPlace.Core.Services.Pos.Sensors.Models;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.Api.Tests.Controllers
{
    public class PosControllerTests : TestsBase
    {
        private const int NonExistingPosId = 5;
        private const int DefaultPosId = 1;
        private readonly int[] TestPosIds = { 1, 2, 3 };
        private IUnitOfWorkFactory _unitOfWorkFactory;
        private PosController _posController;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            var serviceProvider = TestServiceProviderFactory.Create();

            _unitOfWorkFactory = serviceProvider.GetService<IUnitOfWorkFactory>();

            var mockTelegramMessageSender = new Mock<ITelegramChannelMessageSender>();
            var posSensorMeasurementsTrackerFactory = serviceProvider.GetRequiredService<IPosSensorsMeasurementsManagerFactory>();
            var posSensorControllerTypeConfigurationProvider = serviceProvider.GetRequiredService<IPosSensorControllerTypeConfigurationProvider>();
            var posSensorControllerMeasurementsTracker = serviceProvider.GetRequiredService<IPosSensorControllerMeasurementsTracker>();

            var mockPosRealTimeInfo = serviceProvider.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>();

            foreach (var posId in TestPosIds)
            {
                mockPosRealTimeInfo.Setup(p => p.GetOrAddById(posId))
                    .Returns(new PosRealTimeInfo(posId));
            }

            _posController = new PosController(
                _unitOfWorkFactory,
                mockTelegramMessageSender.Object,
                posSensorMeasurementsTrackerFactory,
                posSensorControllerTypeConfigurationProvider,
                posSensorControllerMeasurementsTracker,
                serviceProvider.GetService<Logging.ILogger>()
            );
        }

        [Test]
        public void UpdateAntennasOutputPower_AntennasOutputPowerDtoIsValid_ShouldReturnNotNullAndAntennasOutputPowerFieldChanged()
        {
            _posController.UpdateAntennasOutputPower(new PosAntennasOutputPowerDto
            {
                PosId = DefaultPosId,
                OutputPower = PosAntennasOutputPower.Dbm20
            }).GetAwaiter().GetResult();

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var result = unitOfWork.PosRealTimeInfos.GetById(DefaultPosId);

                result.Should().NotBeNull();
                result.AntennasOutputPower.Should().BeEquivalentTo(PosAntennasOutputPower.Dbm20);
            }
        }

        [Test]
        public void UpdateAntennasOutputPower_PosIdIsNotSpecified_ShouldReturnNotNullAndAntennasOutputPowerFieldNotChange()
        {
            _posController.UpdateAntennasOutputPower(new PosAntennasOutputPowerDto
            {
                OutputPower = PosAntennasOutputPower.Dbm20
            }).GetAwaiter().GetResult();

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var result = unitOfWork.PosRealTimeInfos.GetById(DefaultPosId);

                result.Should().NotBeNull();
                result.AntennasOutputPower.Should().BeEquivalentTo(PosAntennasOutputPower.Dbm0);
            }
        }

        [Test]
        public void UpdateAntennasOutputPower_AntennasOutputPowerIsNotSpecified_ShouldReturnNotNullAndAntennasOutputPowerFieldNotChange()
        {
            _posController.UpdateAntennasOutputPower(new PosAntennasOutputPowerDto
            {
                PosId = DefaultPosId
            }).GetAwaiter().GetResult();

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var result = unitOfWork.PosRealTimeInfos.GetById(DefaultPosId);

                result.Should().NotBeNull();
                result.AntennasOutputPower.Should().BeEquivalentTo(PosAntennasOutputPower.Dbm0);
            }
        }

        [TestCase(SensorControllerType.Legacy)]
        [TestCase(SensorControllerType.New)]
        [TestCase(SensorControllerType.Esp)]
        public void UpdateSensorsMeasurements_SensorsByAllChannels_ShouldGetInsidePosTemperatureFromEvaporatorAndHumidityFromHumidityInsidePos(SensorControllerType controllerType)
        {
            var posSensorsMeasurementsDto = GetPosSensorsMeasurementsDto(controllerType);

            _posController.UpdateSensorsMeasurements(posSensorsMeasurementsDto).GetAwaiter().GetResult();

            EnsureGetCorrectTemperatureAndHumidity(
                posSensorsMeasurementsDto,
                SensorPosition.Evaporator,
                SensorPosition.HumidityInsidePos,
                controllerType);
        }

        [TestCase(SensorControllerType.Legacy)]
        [TestCase(SensorControllerType.New)]
        [TestCase(SensorControllerType.Esp)]
        public void UpdateSensorsMeasurements_SensorsWithoutEvaporatorAndHumidityInsidePosChannels_ShouldGetInsidePosTempratureAndHumidityFromInsidePos(SensorControllerType controllerType)
        {
            var posSensorsMeasurementsDto = GetPosSensorsMeasurementsDto(controllerType);

            var evaparator =
                posSensorsMeasurementsDto.SensorsMeasurements.FirstOrDefault(x =>
                    x.SensorId == (int)SensorPosition.Evaporator);
            var humidity =
                posSensorsMeasurementsDto.SensorsMeasurements.FirstOrDefault(x =>
                    x.SensorId == (int)SensorPosition.HumidityInsidePos);

            posSensorsMeasurementsDto.SensorsMeasurements.Remove(evaparator);
            posSensorsMeasurementsDto.SensorsMeasurements.Remove(humidity);

            _posController.UpdateSensorsMeasurements(posSensorsMeasurementsDto).GetAwaiter().GetResult();

            EnsureGetCorrectTemperatureAndHumidity(
                posSensorsMeasurementsDto,
                SensorPosition.InsidePos,
                SensorPosition.InsidePos,
                controllerType);
        }

        [TestCase(SensorControllerType.Legacy)]
        [TestCase(SensorControllerType.New)]
        [TestCase(SensorControllerType.Esp)]
        public void UpdateSensorsMeasurements_SensorsWithoutHumidityInsidePosChannel_ShouldGetInsidePosTemperatureFromEvaporatorAndHumidityFromInsidePos(SensorControllerType controllerType)
        {
            var posSensorsMeasurementsDto = GetPosSensorsMeasurementsDto(controllerType);

            var humidity =
                posSensorsMeasurementsDto.SensorsMeasurements.FirstOrDefault(x =>
                    x.SensorId == (int)SensorPosition.HumidityInsidePos);

            posSensorsMeasurementsDto.SensorsMeasurements.Remove(humidity);

            _posController.UpdateSensorsMeasurements(posSensorsMeasurementsDto).GetAwaiter().GetResult();

            EnsureGetCorrectTemperatureAndHumidity(
                posSensorsMeasurementsDto,
                SensorPosition.Evaporator,
                SensorPosition.InsidePos,
                controllerType);
        }

        [TestCase(SensorControllerType.Legacy)]
        [TestCase(SensorControllerType.New)]
        [TestCase(SensorControllerType.Esp)]
        public void UpdateSensorsMeasurements_SensorsWithoutEvaporatorChannel_ShouldGetInsidePosTemperatureFromInsidePosAndHumidityFromHumidityInsidePos(SensorControllerType controllerType)
        {
            var posSensorsMeasurementsDto = GetPosSensorsMeasurementsDto(controllerType);

            var evaparator =
                posSensorsMeasurementsDto.SensorsMeasurements.FirstOrDefault(x =>
                    x.SensorId == (int)SensorPosition.Evaporator);

            posSensorsMeasurementsDto.SensorsMeasurements.Remove(evaparator);

            _posController.UpdateSensorsMeasurements(posSensorsMeasurementsDto).GetAwaiter().GetResult();

            EnsureGetCorrectTemperatureAndHumidity(
                posSensorsMeasurementsDto,
                SensorPosition.InsidePos,
                SensorPosition.HumidityInsidePos,
                controllerType);
        }

        [TestCase(SensorControllerType.Legacy)]
        [TestCase(SensorControllerType.New)]
        [TestCase(SensorControllerType.Esp)]
        public void UpdateSensorsMeasurements_NotExistingPosId_ShouldGetArgumentNullException(SensorControllerType controllerType)
        {
            var posSensorsMeasurementsDto = GetPosSensorsMeasurementsDto(controllerType);

            posSensorsMeasurementsDto.PosId = NonExistingPosId;

            Action updateMeasurementsAction = () => _posController.UpdateSensorsMeasurements(posSensorsMeasurementsDto).GetAwaiter().GetResult();

            updateMeasurementsAction.Should().Throw<ArgumentException>();
        }

        private PosSensorsMeasurementsDto GetPosSensorsMeasurementsDto(SensorControllerType type)
        {
            var measurements = JsonConvert.DeserializeObject<PosSensorsMeasurementsDto>(PosSensorsMeasurementsDtoJsonDictionary[type]);
            measurements.PosId = Context.PointsOfSale.FirstOrDefault(pos => pos.SensorControllerType == type)?.Id;

            return measurements;
        }

        private void EnsureGetCorrectTemperatureAndHumidity(PosSensorsMeasurementsDto posSensorsMeasurementsDto,
            SensorPosition temperatureSensorPosition,
            SensorPosition humiditySensorPosition,
            SensorControllerType type)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                if (posSensorsMeasurementsDto.PosId == null) return;
                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById((int)posSensorsMeasurementsDto.PosId);

                posRealTimeInfo.Should().NotBeNull();

                var expectedTemperature = ExpectedTemperatureMeasurementsDictionary[type].FirstOrDefault(sm => sm.SensorPosition == temperatureSensorPosition)?.Temperature;
                var expectedHumidity = ExpectedTemperatureMeasurementsDictionary[type].FirstOrDefault(sm => sm.SensorPosition == humiditySensorPosition)?.Humidity;
                var expectedAmperage = ExpectedTemperatureMeasurementsDictionary[type].FirstOrDefault(sm => sm.SensorPosition == SensorPosition.InsidePos)?.Amperage;
                var expectedFrontPanelPosition = ExpectedTemperatureMeasurementsDictionary[type].FirstOrDefault(sm => sm.SensorPosition == SensorPosition.InsidePos)?.FrontPanelPosition;

                var insidePos =
                    posRealTimeInfo.SensorsMeasurements.FirstOrDefault(x => x.SensorPosition == SensorPosition.InsidePos);

                insidePos?.Temperature.Should().Be(expectedTemperature);
                insidePos?.Humidity.Should().Be(expectedHumidity);
                insidePos?.Amperage.Should().Be(expectedAmperage);
                insidePos?.FrontPanelPosition.Should().Be(expectedFrontPanelPosition);
            }
        }

        private Dictionary<SensorControllerType, string> PosSensorsMeasurementsDtoJsonDictionary => new Dictionary<SensorControllerType, string>()
        {
            {
                SensorControllerType.Legacy,
                @"{'sensorsMeasurements':
                [{'sensorId':0,'temperature':100,'humidity':65.2},
                {'sensorId':1,'temperature':25.3,'humidity':10.8},
                {'sensorId':2,'temperature':28.6875,'humidity':0.0},
                {'sensorId':3,'temperature':-20,'humidity':0.0},
                {'sensorId':4,'temperature':-0.0625,'humidity':0.0},
                {'sensorId':5,'temperature':-0.0625,'humidity':1.0},
                {'sensorId':6,'temperature':-0.0625,'humidity':2.0},
                {'sensorId':7,'temperature':-0.0625,'humidity':3.0}]}"
            },
            {
                SensorControllerType.New,
                @"{'sensorsMeasurements':
                [{'sensorId':0,'temperature':100,'humidity':65.2},
                {'sensorId':1,'temperature':25.3,'humidity':10.8},
                {'sensorId':2,'temperature':28.6875,'humidity':0.0},
                {'sensorId':3,'temperature':-20,'humidity':0.0},
                {'sensorId':4,'temperature':-0.0625,'humidity':0.0},
                {'sensorId':5,'temperature':-0.0625,'humidity':1.0},
                {'sensorId':6,'temperature':-0.0625,'humidity':2.0},
                {'sensorId':7,'temperature':-0.0625,'humidity':3.0}]}"
            },
            {
                SensorControllerType.Esp,
                @"{'sensorsMeasurements':
                [{'sensorId':0,'temperature':100,'humidity':65.2,'amperage':1.2,'frontPanelPosition':1},
                {'sensorId':1,'temperature':25.3,'humidity':10.8,'amperage':1.5,'frontPanelPosition':1},
                {'sensorId':2,'temperature':28.6875,'humidity':0.0,'amperage':1.7,'frontPanelPosition':1}]}"
            }
        };

        private Dictionary<SensorControllerType, IEnumerable<SensorMeasurements>> ExpectedTemperatureMeasurementsDictionary => new Dictionary<SensorControllerType, IEnumerable<SensorMeasurements>>()
        {
            {
                SensorControllerType.Legacy,
                new []
                {
                    new SensorMeasurements(SensorPosition.InsidePos, 100, 65.2, 0.0, FrontPanelPosition.NotFound),
                    new SensorMeasurements(SensorPosition.Evaporator, 100, 65.2, 0.0, FrontPanelPosition.NotFound),
                    new SensorMeasurements(SensorPosition.HumidityInsidePos, 100, 65.2, 0.0, FrontPanelPosition.NotFound)
                }
            },
            {
                SensorControllerType.New,
                new []
                {
                    new SensorMeasurements(SensorPosition.InsidePos, 100, 65.2, 0.0, FrontPanelPosition.NotFound),
                    new SensorMeasurements(SensorPosition.Evaporator, -20, 0.0, 0.0, FrontPanelPosition.NotFound),
                    new SensorMeasurements(SensorPosition.HumidityInsidePos, -0.0625, 0.0, 0.0, FrontPanelPosition.NotFound)
                }
            },
            {
                SensorControllerType.Esp,
                new []
                {
                    new SensorMeasurements(SensorPosition.InsidePos, 100, 65.2, 1.2, FrontPanelPosition.Opened),
                    new SensorMeasurements(SensorPosition.Evaporator, 100, 65.2, 1.2, FrontPanelPosition.Opened),
                    new SensorMeasurements(SensorPosition.HumidityInsidePos, 100, 65.2, 1.2, FrontPanelPosition.Opened)
                }
            }
        };
    }
}
