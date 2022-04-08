using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.Utilities.Models;
using NUnit.Framework;
using System;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;

namespace NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder
{
    public class PosAbnormalSensorMeasurementContentBuilderShould : TestsBase
    {
        private const int DefaultPosId = 1;

        private IDailyStatisticsContentBuilder _contentBuilder;

        public override void SetUp()
        {
            base.SetUp();

            var serviceProvider = TestServiceProviderFactory.Create();
            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();

            var dailyStatisticsConfigurationModel = new DailyStatisticsConfigurationModel()
            {
                PosAbnormalSensorMeasurementCountLink = "{0}/PosSensors/GetPosAbnormalSensorMeasurement?PosAbnormalSensorMeasurementDateFrom={1}&PosAbnormalSensorMeasurementDateUntil={2}",
                AdminPageBaseUrl = "http://nursultanapi.nasladdin.club"
            };

            _contentBuilder = new PosAbnormalSensorMeasurementContentBuilder(unitOfWorkFactory, dailyStatisticsConfigurationModel);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenPosAbnormalSensorMeasurementTableIsEmpty()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            AssertExpectedPosAbnormalSensorMeasurementContent(utcTimeInterval,
                expectedAbnormalTemperatureCount: 0);
        }

        [Test]
        public void ReturnEmptyContentModelWhenGivenDateRangeWhichDoesNotFallPosAbnormalMeasurementValue()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(new PosAbnormalSensorMeasurementDataSet(DefaultPosId));

            AssertExpectedPosAbnormalSensorMeasurementContent(utcTimeInterval,
                expectedAbnormalTemperatureCount: 0);
        }

        [Test]
        public void ReturnExpectedAbnormalTemperatureAndHumidityCountWhenGivenCorrectlyDateRangeWithValue()
        {
            var endUtcDateTime = DateTime.UtcNow.AddHours(1);
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcDateTimeRange = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(new PosAbnormalSensorMeasurementDataSet(DefaultPosId));

            AssertExpectedPosAbnormalSensorMeasurementContent(utcDateTimeRange,
                expectedAbnormalTemperatureCount: 1);
        }

        private void AssertExpectedPosAbnormalSensorMeasurementContent(DateTimeRange utcDateTimeRange,
            int expectedAbnormalTemperatureCount)
        {
            var resultContent =
                (PosAbnormalSensorMeasurementContent)_contentBuilder
                    .BuildContentWithLinkAsync(utcDateTimeRange).Result;

            resultContent.Should().NotBeNull();
            resultContent.AbnormalTemperatureCount.Should().Be(expectedAbnormalTemperatureCount);
        }
    }
}