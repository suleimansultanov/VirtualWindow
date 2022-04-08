using System;
using System.Collections.Concurrent;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Services.Configuration.Validators.Factory;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Contracts;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;

namespace NasladdinPlace.Api.Tests.Scenarios.PointsOfSale.ScreenResolution
{
    public class PosScreenResolutionCheckerTests : TestsBase
    {
        private const int DefaultHeight = 700;
        private const int DefaultWidth = 1400;

        private IServiceProvider _serviceProvider;
        private IPosScreenResolutionChecker _posScreenResolutionChecker;
        private ConcurrentBag<PosRealTimeInfo> _posRealTimeInfos;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            _serviceProvider = TestServiceProviderFactory.Create();

            _posScreenResolutionChecker = _serviceProvider.GetRequiredService<IPosScreenResolutionChecker>();
            _posRealTimeInfos = new ConcurrentBag<PosRealTimeInfo>();

            InsertPosScreenResolutionCheckerDelayLastSentInMinutes();
        }

        [TestCase(1200, 600, 1)]
        [TestCase(1400, 700, 0)]
        [TestCase(1920, 1080, 1)]
        public void
            GetPointsOfSaleWithIncorrectScreenResolutionAsync_PointsOfSaleWithDifferentHeightAndWidthInDatabaseAndConnectedIsGiven_ShouldReturnExpectedPosScreenResolutionInfoCount(
                int width, int height, int expectedCount)
        {
            var firstPosId = GetFirstPosId();
            var posScreenResolution = new Core.Models.ScreenResolution(width, height);

            SetRequiredPosScreenResolution(
                id: firstPosId,
                screenResolution: new Core.Models.ScreenResolution(DefaultWidth, DefaultHeight));
            SetPosScreenResolution(
                posId: firstPosId,
                screenResolution: posScreenResolution,
                connectionStatus: PosConnectionStatus.Connected,
                screenResolutionSyncDateTime: DateTime.UtcNow);

            var pointsOfSaleScreenResolutionInfo =
                _posScreenResolutionChecker.GetPointsOfSaleWithIncorrectScreenResolutionAsync().Result;
            pointsOfSaleScreenResolutionInfo.Should().HaveCount(expectedCount);
        }

        [Test]
        public void GetPointsOfSaleWithIncorrectScreenResolutionAsync_PointsOfSaleExistsAndNoConnectedPosExists_ShouldReturnNoPosScreenResolutionInfo()
        {
            var pointsOfSaleScreenResolutionInfo =
                _posScreenResolutionChecker.GetPointsOfSaleWithIncorrectScreenResolutionAsync().Result;
            pointsOfSaleScreenResolutionInfo.Should().HaveCount(0);
        }

        [TestCase(1200, 600, 1)]
        [TestCase(1400, 700, 0)]
        [TestCase(1920, 1080, 1)]
        public void
            GetPointsOfSaleWithIncorrectScreenResolutionAsync_OneConnectedPosAndOneDisconnectedPosWithCorrectDefaultResolutionAreGiven_ShouldReturnExpectedPosScreenResolutionInfoCount(
                int width, int height, int expectedCount)
        {
            var firstPosId = GetFirstPosId();
            var secondPosId = GetSecondPosId();

            var posScreenResolution = new Core.Models.ScreenResolution(width, height);

            SetRequiredPosScreenResolution(
                id: firstPosId,
                screenResolution: new Core.Models.ScreenResolution(DefaultWidth, DefaultHeight));
            SetRequiredPosScreenResolution(
                id: secondPosId,
                screenResolution: new Core.Models.ScreenResolution(DefaultWidth, DefaultHeight));

            SetPosScreenResolution(
                posId: firstPosId,
                screenResolution: posScreenResolution,
                connectionStatus: PosConnectionStatus.Connected,
                screenResolutionSyncDateTime: DateTime.UtcNow);
            SetPosScreenResolution(
                posId: secondPosId,
                screenResolution: posScreenResolution,
                connectionStatus: PosConnectionStatus.Disconnected,
                screenResolutionSyncDateTime: DateTime.UtcNow);

            var pointsOfSaleScreenResolutionInfo =
                _posScreenResolutionChecker.GetPointsOfSaleWithIncorrectScreenResolutionAsync().Result;
            pointsOfSaleScreenResolutionInfo.Should().HaveCount(expectedCount);
        }

        [TestCase(PosConnectionStatus.Connected, PosConnectionStatus.Connected, 2)]
        [TestCase(PosConnectionStatus.Connected, PosConnectionStatus.Disconnected, 1)]
        [TestCase(PosConnectionStatus.Disconnected, PosConnectionStatus.Disconnected, 0)]
        public void
            GetPointsOfSaleWithIncorrectScreenResolutionAsync_DifferentConnectionStatusAndIncorrectScreenResolutionAreGiven_ShouldReturnExpectedPosScreenResolutionInfoCount(
                PosConnectionStatus firstPosConnectionStatus, PosConnectionStatus secondPosConnectionStatus,
                int expectedCount)
        {
            var firstPosId = GetFirstPosId();
            var secondPosId = GetSecondPosId();

            var posScreenResolution = new Core.Models.ScreenResolution(DefaultHeight, DefaultWidth);

            SetRequiredPosScreenResolution(
                id: firstPosId,
                screenResolution: new Core.Models.ScreenResolution(DefaultWidth, DefaultHeight));
            SetRequiredPosScreenResolution(
                id: secondPosId,
                screenResolution: new Core.Models.ScreenResolution(DefaultWidth, DefaultHeight));

            SetPosScreenResolution(
                posId: firstPosId,
                screenResolution: posScreenResolution,
                connectionStatus: firstPosConnectionStatus,
                screenResolutionSyncDateTime: DateTime.UtcNow);
            SetPosScreenResolution(
                posId: secondPosId,
                screenResolution: posScreenResolution,
                connectionStatus: secondPosConnectionStatus,
                screenResolutionSyncDateTime: DateTime.UtcNow);

            var pointsOfSaleScreenResolutionInfo =
                _posScreenResolutionChecker.GetPointsOfSaleWithIncorrectScreenResolutionAsync().Result;
            pointsOfSaleScreenResolutionInfo.Should().HaveCount(expectedCount);
        }

        [Test]
        public void
            GetPointsOfSaleWithIncorrectScreenResolutionAsync_PointsOfSaleConnectedButLastUpdateMoreThanDelay_ShouldReturnPosScreenResolutionCountNotZero()
        {
            var firstPosId = GetFirstPosId();

            var posScreenResolution = new Core.Models.ScreenResolution(DefaultHeight, DefaultWidth);
            var screenResolutionSyncDateTimeInPast = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));

            SetRequiredPosScreenResolution(
                id: firstPosId,
                screenResolution: posScreenResolution);

            SetPosScreenResolution(
                posId: firstPosId,
                screenResolution: posScreenResolution,
                connectionStatus: PosConnectionStatus.Connected,
                screenResolutionSyncDateTime: screenResolutionSyncDateTimeInPast);

            var pointsOfSaleScreenResolutionInfo =
                _posScreenResolutionChecker.GetPointsOfSaleWithIncorrectScreenResolutionAsync().Result;

            pointsOfSaleScreenResolutionInfo.Should().NotBeEmpty();
        }

        private void SetPosScreenResolution(int posId, Core.Models.ScreenResolution screenResolution,
            PosConnectionStatus connectionStatus, DateTime screenResolutionSyncDateTime)
        {
            var posRealTimeInfo = new PosRealTimeInfo(posId)
            {
                ConnectionStatus = connectionStatus,
            };

            posRealTimeInfo.UpdatableScreenResolution.Update(screenResolution);
            posRealTimeInfo.UpdatableScreenResolution.SetProperty(nameof(posRealTimeInfo.UpdatableScreenResolution.DateUpdated),
                screenResolutionSyncDateTime);

            var mockPosRealTimeInfoDataStore = _serviceProvider.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>();
            mockPosRealTimeInfoDataStore.Setup(s => s.GetOrAddById(posId)).Returns(posRealTimeInfo);

            _posRealTimeInfos.Add(posRealTimeInfo);

            mockPosRealTimeInfoDataStore.Setup(s => s.GetAll()).Returns(_posRealTimeInfos);
        }

        private void SetRequiredPosScreenResolution(int id, Core.Models.ScreenResolution screenResolution)
        {
            var pos = Context.PointsOfSale.FirstOrDefault(p => p.Id == id);
            pos.ScreenResolutionOrNull = screenResolution;
            Context.SaveChanges();
        }

        private int GetFirstPosId()
        {
            return Context.PointsOfSale.OrderBy(pos => pos.Id).First().Id;
        }

        private int GetSecondPosId()
        {
            return Context.PointsOfSale.OrderBy(pos => pos.Id).Skip(1).First().Id;
        }

        private void InsertPosScreenResolutionCheckerDelayLastSentInMinutes()
        {
            var configurationKey = new ConfigurationKey(
                id: ConfigurationKeyIdentifier.PosScreenResolutionCheckerResolutionMaxUpdateDelayInMinutes,
                name: nameof(ConfigurationKeyIdentifier.PosScreenResolutionCheckerResolutionMaxUpdateDelayInMinutes),
                valueDataType: ConfigurationValueDataType.TimeSpan
            );
            Context.ConfigurationKeys.Add(configurationKey);

            var configurationValue = new ConfigurationValue(
                ConfigurationKeyIdentifier.PosScreenResolutionCheckerResolutionMaxUpdateDelayInMinutes,
                "00:10:00"
            );

            configurationKey.TrySetValue(
                configurationValue,
                new ConfigurationValueValidatorsFactory()
            );

            Context.SaveChanges();
        }
    }
}