using System;
using FluentAssertions;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.Utilities.Models;
using NUnit.Framework;

namespace NasladdinPlace.DAL.Tests.Repositories.PosRepository
{
    public class PosTemperatureRepositoryIntegrationTests : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultMeasurementPeriodInMinutes = 35;
        private const double DefaultTemperatureValue = 4D;
        private const double OldTemperatureValue = 2D;

        private IPosTemperatureRepository _posTemperaturesRepository;

        public override void SetUp()
        {
            base.SetUp();
            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            
            _posTemperaturesRepository = new DAL.Repositories.PosTemperatureRepository(Context);
        }

        [Test]
        public void GetAverageByPosId_PosAndArrayOfMeasuredTemperaturesAreGiven_ShouldReturnDefaultAverageTemperature()
        {
            AddPosTemperatures();
            var averageTemperatureOfPos = _posTemperaturesRepository.GetAverageByPosId(DefaultPosId, TimeSpan.FromMinutes(DefaultMeasurementPeriodInMinutes));

            averageTemperatureOfPos.Temperature.Should().Be(DefaultTemperatureValue);
        }

        [Test]
        public void GetLatestByPos_PosAndArrayOfMeasuredTemperaturesAreGiven_ShouldReturnDefaultTemperature()
        {
            AddPosTemperatures();
            var averageTemperatureOfPos = _posTemperaturesRepository.GetLatestByPos(DefaultPosId);

            averageTemperatureOfPos.Temperature.Should().Be(DefaultTemperatureValue);
        }

        [Test]
        public void GetAverageByPosId_EmptyPosTemperaturesAreGiven_ShouldReturnNanAverageTemperature()
        {
            var averageTemperatureOfPos = _posTemperaturesRepository.GetAverageByPosId(DefaultPosId, TimeSpan.FromMinutes(DefaultMeasurementPeriodInMinutes));

            averageTemperatureOfPos.Temperature.Should().Be(double.NaN);
        }

        [Test]
        public void GetLatestByPos_EmptyPosTemperaturesAreGiven_ShouldReturnNanTemperature()
        {
            var averageTemperatureOfPos = _posTemperaturesRepository.GetLatestByPos(DefaultPosId);

            averageTemperatureOfPos.Temperature.Should().Be(double.NaN);
        }

        private void AddPosTemperatures()
        {
            var averageTemperatureMeasurementDefaultDateTimeFrom = GetAverageTemperatureMeasurementDefaultDateTimeFrom();
            var periodForAverageTemperaturesMeasurements = DateTimeRange.From(averageTemperatureMeasurementDefaultDateTimeFrom, DateTime.UtcNow);
            var periodForObsoleteMeasurements = DateTimeRange.From(averageTemperatureMeasurementDefaultDateTimeFrom.Add(-TimeSpan.FromMinutes(DefaultMeasurementPeriodInMinutes)), averageTemperatureMeasurementDefaultDateTimeFrom);
            CreateStubPosTemperatures(DefaultPosId, periodForAverageTemperaturesMeasurements, DefaultTemperatureValue);
            CreateStubPosTemperatures(DefaultPosId, periodForObsoleteMeasurements, OldTemperatureValue);
        }

        private void CreateStubPosTemperatures(int posId, DateTimeRange range, double temperatureValue)
        {
            var posTemperatureDateCreated = range.Start;
            do
            {
                _posTemperaturesRepository.Add(new PosTemperature(posId, temperatureValue, posTemperatureDateCreated));
                posTemperatureDateCreated = posTemperatureDateCreated.Add(TimeSpan.FromMinutes(1));

            } while (posTemperatureDateCreated < range.End);

            Context.SaveChanges();
        }

        private DateTime GetAverageTemperatureMeasurementDefaultDateTimeFrom()
        {
            return DateTime.UtcNow.Add(-TimeSpan.FromMinutes(DefaultMeasurementPeriodInMinutes));
        }
    }
}