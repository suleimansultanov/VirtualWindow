using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder.DataGenerators;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.Utilities.Models;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder
{
    public class UnpaidPurchasesContentBuilderShould : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;

        private IDailyStatisticsContentBuilder _contentBuilder;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            var dailyStatisticsConfigurationModel = new DailyStatisticsConfigurationModel
            {
                UnpaidPurchaseExpirationHours = 25,
                AdminPageBaseUrl = "http://nursultanapi.nasladdin.club",
                TotalUnpaidCheckItemsLink = "{0}/Checks?OperationDateFrom={1}&OperationDateUntil={2}&HasUnverifiedCheckItems={3}&OperationStatus={4}"
            };
            var serviceProvider = TestServiceProviderFactory.Create();
            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _contentBuilder = new UnpaidPurchasesContentBuilder(unitOfWorkFactory, dailyStatisticsConfigurationModel);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenPosOperationTableIsEmpty()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);


            AssertExpectedUnpaidPurchasesContent(utcTimeInterval, expectedPosOperationsCount: 0, expectedTotalPrice: 0M);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenPosOperationWithoutCheckItems()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(DefaultPosId, DefaultUserId));

            AssertExpectedUnpaidPurchasesContent(utcTimeInterval, expectedPosOperationsCount: 0, expectedTotalPrice: 0M);
        }

        [TestCaseSource(typeof(UnpaidPurchasesContentBuilderCheckItemsDataGenerator))]
        public void ReturnExpectedContentWhenDifferentPosOperationsAreGiven(Collection<CheckItem> checkItems,
            int expectedPosOperation, decimal expectedTotalPrice)
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            var firstPosOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetMode(PosMode.Purchase)
                .Build();

            var secondPosOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsCompleted()
                .SetMode(PosMode.Purchase)
                .Build();

            var unpaidPurchaseExpireDateTime = DateTime.UtcNow.AddDays(-2).AddHours(-1);
            firstPosOperation.SetProperty(nameof(PosOperation.DateStarted), unpaidPurchaseExpireDateTime);
            secondPosOperation.SetProperty(nameof(PosOperation.DateStarted), unpaidPurchaseExpireDateTime);
            secondPosOperation.SetProperty(nameof(PosOperation.DateCompleted), unpaidPurchaseExpireDateTime);

            Seeder.Seed(new Collection<PosOperation>
            {
                firstPosOperation,
                secondPosOperation
            });
            Seeder.Seed(checkItems);

            AssertExpectedUnpaidPurchasesContent(utcTimeInterval, expectedPosOperation, expectedTotalPrice);
        }

        private void AssertExpectedUnpaidPurchasesContent(DateTimeRange utcDateTimeRange,
            int expectedPosOperationsCount, decimal expectedTotalPrice)
        {
            var resultContent =
                (UnpaidPurchasesContent)_contentBuilder.BuildContentWithLinkAsync(utcDateTimeRange).Result;

            resultContent.Should().NotBeNull();
            resultContent.PosOperationsCount.Should().Be(expectedPosOperationsCount);
            resultContent.TotalPrice.Should().Be(expectedTotalPrice);
        }
    }
}