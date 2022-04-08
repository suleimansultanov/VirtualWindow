using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Doors.Contracts;
using NasladdinPlace.Core.Services.Pos.Temperature.Models.Settings;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureManager;
using NasladdinPlace.Core.Services.Pos.Temperature.PosTemperatureSettings;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.Utilities.DateTimeConverter;
using NUnit.Framework;

namespace NasladdinPlace.Api.Tests.Scenarios.PointsOfSale.PosState
{
    public class PosStateHistoricalDataDeletingTests : TestsBase
    {
        private const int DefaultPosId = 1;
        private const double DefaultPosTemperature = 3;

        private IServiceProvider _serviceProvider;
        private IPosDoorsStateTracker _posDoorsStateManager;
        private IPosTemperatureManager _posTemperatureManager;
        private PosStateHistoricalDataSettings _historicalDataSettings;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            _serviceProvider = TestServiceProviderFactory.Create();

            _posDoorsStateManager = _serviceProvider.GetRequiredService<IPosDoorsStateTracker>();
            _posTemperatureManager = _serviceProvider.GetRequiredService<IPosTemperatureManager>();
            _historicalDataSettings = _serviceProvider.GetRequiredService<IPosStateSettingsProvider>().GetHistoricalDataSettings();
        }

        [Test]
        public void CheckObsoletePosDoorsStateDataIsDeleted_ActualAndObsoleteDoorsStateDataGiven_ShouldReturnOnlyActualDataAfterDeleting()
        {
            CreatePosDoorState(DateTime.UtcNow);

            var obsoleteDateCreated = CreateObsoleteDateCreated();

            CreatePosDoorState(obsoleteDateCreated);

            _posDoorsStateManager.DeletePosDoorsStateHistoricalDataAsync().GetAwaiter().GetResult();

            var expectedPosDoorsStates = Context.PosDoorsStates.Select(ps => new ExpectedPosStateInfo()
                {
                    Id = ps.Id,
                    DateCreated = ps.DateCreated
                }
            );

            EnsureObsoletePosStateDataDeletedAndActualDataNotDeleted(expectedPosDoorsStates.ToList());
        }

        [Test]
        public void CheckObsoletePosTemperatureDataIsDeleted_ActualAndObsoleteTemperatureGiven_ShouldReturnOnlyActualDataAfterDeleting()
        {
            CreatePosTemperature(DateTime.UtcNow);

            var obsoleteDateCreated = CreateObsoleteDateCreated();

            CreatePosTemperature(obsoleteDateCreated);

            _posTemperatureManager.DeletePosTemperaturesHistoricalDataAsync().GetAwaiter().GetResult();

            var expectedPosTemperatures = Context.PosTemperatures.Select(pt => new ExpectedPosStateInfo()
                {
                    Id = pt.Id,
                    DateCreated = pt.DateCreated
                }
            );

            EnsureObsoletePosStateDataDeletedAndActualDataNotDeleted(expectedPosTemperatures.ToList());
        }

        private void EnsureObsoletePosStateDataDeletedAndActualDataNotDeleted(List<ExpectedPosStateInfo> posStateData)
        {
            posStateData.Should().NotBeNull();
            posStateData.Should().NotBeEmpty();
            posStateData.Count.Should().Be(1);
            var obsoleteDataDateTime = CreateActualDataLifeTimeBorderDate();
            posStateData.First().DateCreated.Should().BeAfter(obsoleteDataDateTime);
        }

        private DateTime CreateObsoleteDateCreated()
        {
            return CreateActualDataLifeTimeBorderDate().Add(-TimeSpan.FromMinutes(1));
        }

        private DateTime CreateActualDataLifeTimeBorderDate()
        {
            return UtcMoscowDateTimeConverter.ConvertToUtcDateTime(DateTime.UtcNow.Date.Add(-_historicalDataSettings.PosStateDataLifeTimePeriod));
        }

        private void CreatePosTemperature(DateTime dateCreated)
        {
            var posTemperature = new PosTemperature(DefaultPosId, DefaultPosTemperature);
            posTemperature.SetProperty(nameof(posTemperature.DateCreated), dateCreated);
            Context.PosTemperatures.Add(posTemperature);
            Context.SaveChanges();
        }

        private void CreatePosDoorState(DateTime dateCreated)
        {
            var posDoorsState = PosDoorsState.Closed(DefaultPosId);
            posDoorsState.SetProperty(nameof(posDoorsState.DateCreated), dateCreated);
            Context.PosDoorsStates.Add(posDoorsState);
            Context.SaveChanges();
        }
    }
}