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
using NasladdinPlace.Core.Services.Pos.Version;
using NasladdinPlace.Core.Services.Pos.Version.Contracts;
using NasladdinPlace.Core.Services.Pos.Version.Models;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;

namespace NasladdinPlace.Api.Tests.Scenarios.PointsOfSale.Versioning
{
    public class PointsOfSaleVersionUpdateCheckerShould : TestsBase
    {
        private const string UndefinedMinRequiredPosVersion = 
            PointsOfSaleVersionUpdateChecker.UndefinedMinRequiredPosVersion;
        private const string MinRequiredPosVersion = "1.0.0.123";
        
        private IServiceProvider _serviceProvider;
        private IPointsOfSaleVersionUpdateChecker _pointsOfSaleVersionUpdateChecker;
        private ConcurrentBag<PosRealTimeInfo> _posRealTimeInfos;
        
        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            _serviceProvider = TestServiceProviderFactory.Create();
            
            _pointsOfSaleVersionUpdateChecker = _serviceProvider.GetRequiredService<IPointsOfSaleVersionUpdateChecker>();
            _posRealTimeInfos = new ConcurrentBag<PosRealTimeInfo>();
        }

        [TestCase("1.0.0.0")]
        [TestCase("0.0.0.0")]
        [TestCase("0.9.0.0")]
        [TestCase("1.0.0.1229")]
        public void ReturnSinglePosVersionWhenPointsOfSaleMinRequiredVersionIsGreaterThanConnectedPosVersion(string version)
        {
            var firstPosId = GetFirstPosId();
            
            InsertMinRequiredPointsOfSaleVersion(MinRequiredPosVersion);
            SetPosVersion(posId: firstPosId, version: version, connectionStatus: PosConnectionStatus.Connected);
            var versionUpdateInfo = 
                _pointsOfSaleVersionUpdateChecker.GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync().Result;
            versionUpdateInfo.IsUpdateRequired.Should().BeTrue();
            versionUpdateInfo.PointsOfSaleVersionInfo.Should().HaveCount(1);
            EnsurePosVersionInfoCorrect(versionUpdateInfo.PointsOfSaleVersionInfo.First(), firstPosId, version);
            versionUpdateInfo.RequiredMinVersion.Should().Be(MinRequiredPosVersion);
        }

        [TestCase("1.1.0.0")]
        [TestCase(MinRequiredPosVersion)]
        [TestCase("1.9.0.0")]
        [TestCase("1.0.0.5229")]
        public void ReturnNoPosVersionWhenPointsOfSaleMinRequiredVersionIsLessOrEqualToConnectedPosVersion(string version)
        {
            InsertMinRequiredPointsOfSaleVersion(MinRequiredPosVersion);
            SetPosVersion(posId: GetFirstPosId(), version: version, connectionStatus: PosConnectionStatus.Connected);
            var versionUpdateInfo = 
                _pointsOfSaleVersionUpdateChecker.GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync().Result;
            versionUpdateInfo.IsUpdateRequired.Should().BeFalse();
            versionUpdateInfo.PointsOfSaleVersionInfo.Should().HaveCount(0);
            versionUpdateInfo.RequiredMinVersion.Should().Be(MinRequiredPosVersion);
        }

        [TestCase("1.0.0.0")]
        [TestCase("0.0.0.0")]
        [TestCase("0.9.0.0")]
        [TestCase("1.0.0.1229")]
        public void ReturnNoPosVersionWhenPointsOfSaleMinRequiredVersionDoesNotExistAndConnectedPosHasVersion(string version)
        {
            SetPosVersion(posId: GetFirstPosId(), version: version, connectionStatus: PosConnectionStatus.Connected);
            var versionUpdateInfo = 
                _pointsOfSaleVersionUpdateChecker.GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync().Result;
            versionUpdateInfo.IsUpdateRequired.Should().BeFalse();
            versionUpdateInfo.PointsOfSaleVersionInfo.Should().HaveCount(0);
            versionUpdateInfo.RequiredMinVersion.Should().Be(UndefinedMinRequiredPosVersion);
        }

        [Test]
        public void ReturnNoPosVersionWhenPointsOfSaleMinRequiredVersionExistsAndNoConnectedPosExists()
        {
            InsertMinRequiredPointsOfSaleVersion(MinRequiredPosVersion);
            var versionUpdateInfo = 
                _pointsOfSaleVersionUpdateChecker.GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync().Result;
            versionUpdateInfo.IsUpdateRequired.Should().BeFalse();
            versionUpdateInfo.PointsOfSaleVersionInfo.Should().HaveCount(0);
            versionUpdateInfo.RequiredMinVersion.Should().Be(MinRequiredPosVersion);
        }

        [TestCase("1.0.0.0", "1.1.0.0")]
        [TestCase("0.0.0.0", MinRequiredPosVersion)]
        [TestCase("0.9.0.0", "1.9.0.0")]
        [TestCase("1.0.0.1229", "1.0.0.5229")]
        public void ReturnPosWithLesserVersionWhenPointsOfSaleMinRequiredVersionIsGreaterThanOneConnectedPosVersionAndLessOrEqualToAnother(
            string lessThanMinRequiredVersion, string greaterThanMinRequiredVersion)
        {
            var firstPosId = GetFirstPosId();
            
            InsertMinRequiredPointsOfSaleVersion(MinRequiredPosVersion);
            SetPosVersion(
                posId: firstPosId,
                version: lessThanMinRequiredVersion,
                connectionStatus: PosConnectionStatus.Connected
            );
            SetPosVersion(
                posId: GetSecondPosId(),
                version: greaterThanMinRequiredVersion,
                connectionStatus: PosConnectionStatus.Connected
            );
            var versionUpdateInfo = 
                _pointsOfSaleVersionUpdateChecker.GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync().Result;
            versionUpdateInfo.IsUpdateRequired.Should().BeTrue();
            versionUpdateInfo.PointsOfSaleVersionInfo.Should().HaveCount(1);
            EnsurePosVersionInfoCorrect(
                versionUpdateInfo.PointsOfSaleVersionInfo.First(),
                firstPosId,
                lessThanMinRequiredVersion
            );
            versionUpdateInfo.RequiredMinVersion.Should().Be(MinRequiredPosVersion);
        }

        [TestCase("1.0.0.0")]
        [TestCase("0.0.0.0")]
        [TestCase("0.9.0.0")]
        [TestCase("1.0.0.1229")]
        public void
            ReturnConnectedPosVersionWhenPointsOfSaleMinRequiredVersionIsGreaterThanOneConnectedPosVersionAndOneDisconnectedPos(string version)
        {
            var secondPosId = GetSecondPosId();
            
            InsertMinRequiredPointsOfSaleVersion(MinRequiredPosVersion);
            SetPosVersion(
                posId: GetFirstPosId(),
                version: version,
                connectionStatus: PosConnectionStatus.Disconnected
            );
            SetPosVersion(
                posId: secondPosId,
                version: version,
                connectionStatus: PosConnectionStatus.Connected
            );
            var versionUpdateInfo = 
                _pointsOfSaleVersionUpdateChecker.GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync().Result;
            versionUpdateInfo.IsUpdateRequired.Should().BeTrue();
            versionUpdateInfo.PointsOfSaleVersionInfo.Should().HaveCount(1);
            EnsurePosVersionInfoCorrect(versionUpdateInfo.PointsOfSaleVersionInfo.First(), secondPosId, version);
            versionUpdateInfo.RequiredMinVersion.Should().Be(MinRequiredPosVersion);
        }
        
        [TestCase("1.0.0.0", "0.0.0.1")]
        [TestCase("0.0.0.0", "0.8.1.0")]
        [TestCase("0.9.0.0", "0.0.2.124")]
        [TestCase("1.0.0.1229", "1.0.0.1229")]
        public void
            ReturnTwoPosVersionsWhenPointsOfSaleMinRequiredVersionIsGreaterThanConnectedFirstAndSecondPosVersions(
                string firstPosVersion, string secondPosVersion)
        {
            var firstPosId = GetFirstPosId();
            var secondPosId = GetSecondPosId();
            
            InsertMinRequiredPointsOfSaleVersion(MinRequiredPosVersion);
            SetPosVersion(
                posId: firstPosId,
                version: firstPosVersion,
                connectionStatus: PosConnectionStatus.Connected
            );
            SetPosVersion(
                posId: secondPosId,
                version: secondPosVersion,
                connectionStatus: PosConnectionStatus.Connected
            );
            var versionUpdateInfo = 
                _pointsOfSaleVersionUpdateChecker.GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync().Result;
            versionUpdateInfo.IsUpdateRequired.Should().BeTrue();
            versionUpdateInfo.PointsOfSaleVersionInfo.Should().HaveCount(2);
            
            var firstPosVersionInfo = versionUpdateInfo.PointsOfSaleVersionInfo.Single(v => v.PosInfo.Id == firstPosId);
            var secondPosVersionInfo = versionUpdateInfo.PointsOfSaleVersionInfo.Single(v => v.PosInfo.Id == secondPosId);
            
            EnsurePosVersionInfoCorrect(firstPosVersionInfo, firstPosId, firstPosVersion);
            EnsurePosVersionInfoCorrect(secondPosVersionInfo, secondPosId, secondPosVersion);
            
            versionUpdateInfo.RequiredMinVersion.Should().Be(MinRequiredPosVersion);
        }
        
        [Test]
        public void ReturnSinglePosVersionWhenPointsOfSaleMinRequiredVersionExistsAndConnectedPosVersionNotExists()
        {
            var firstPosId = GetFirstPosId();
            
            InsertMinRequiredPointsOfSaleVersion(MinRequiredPosVersion);
            SetPosVersion(posId: firstPosId, version: null, connectionStatus: PosConnectionStatus.Connected);
            var versionUpdateInfo = 
                _pointsOfSaleVersionUpdateChecker.GetVersionInfoOfPointsOfSalesThatRequiredVersionUpdateAsync().Result;
            versionUpdateInfo.IsUpdateRequired.Should().BeTrue();
            versionUpdateInfo.PointsOfSaleVersionInfo.Should().HaveCount(1);
            EnsurePosVersionInfoCorrect(versionUpdateInfo.PointsOfSaleVersionInfo.First(), firstPosId, null);
            versionUpdateInfo.RequiredMinVersion.Should().Be(MinRequiredPosVersion);
        }

        private void InsertMinRequiredPointsOfSaleVersion(string version)
        {
            var pointsOfSaleMinRequiredVersionConfigurationKey = new ConfigurationKey(
                id: ConfigurationKeyIdentifier.PointsOfSaleRequiredMinVersion,
                name: nameof(ConfigurationKeyIdentifier.PointsOfSaleRequiredMinVersion),
                valueDataType: ConfigurationValueDataType.String
            );
            Context.ConfigurationKeys.Add(pointsOfSaleMinRequiredVersionConfigurationKey);

            var pointsOfSaleMinRequiredVersionConfigurationValue = new ConfigurationValue(
                ConfigurationKeyIdentifier.PointsOfSale,
                version
            );

            pointsOfSaleMinRequiredVersionConfigurationKey.TrySetValue(
                pointsOfSaleMinRequiredVersionConfigurationValue,
                new ConfigurationValueValidatorsFactory()
            );
            
            Context.SaveChanges();
        }

        private void SetPosVersion(int posId, string version, PosConnectionStatus connectionStatus)
        {
            var posRealTimeInfo = new PosRealTimeInfo(posId)
            {
                Version = version, 
                ConnectionStatus = connectionStatus
            };

            var mockPosRealTimeInfoDataStore = _serviceProvider.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>();
            mockPosRealTimeInfoDataStore.Setup(s => s.GetOrAddById(posId)).Returns(posRealTimeInfo);
            
            _posRealTimeInfos.Add(posRealTimeInfo);
            
            mockPosRealTimeInfoDataStore.Setup(s => s.GetAll()).Returns(_posRealTimeInfos);
        }

        private int GetFirstPosId()
        {
            return Context.PointsOfSale.OrderBy(pos => pos.Id).First().Id;
        }

        private int GetSecondPosId()
        {
            return Context.PointsOfSale.OrderBy(pos => pos.Id).Skip(1).First().Id;
        }

        private static void EnsurePosVersionInfoCorrect(
            PosVersionInfo posVersionInfo, int expectedPosId, string expectedVersion)
        {
            posVersionInfo.CurrentVersion.Should().Be(expectedVersion);
            posVersionInfo.PosInfo.Id.Should().Be(expectedPosId);
            posVersionInfo.PosInfo.Name.Should().NotBeNullOrEmpty();
        }
    }
}