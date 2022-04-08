using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder.DataGenerators;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.Utilities.Models;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;

namespace NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder
{
    public class PaidPurchasesContentBuilderShould : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;

        private IDailyStatisticsContentBuilder _contentBuilder;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            var serviceProvider = TestServiceProviderFactory.Create();
            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            var dailyStatisticsConfigurationModel = new DailyStatisticsConfigurationModel()
            {
                BasePurchasesLink = "{0}/GoodsMoving?OperationDateFrom={1}&OperationDateUntil={2}&OperationMode={2}",
                AdminPageBaseUrl = "http://nursultanapi.nasladdin.club"
            };

            _contentBuilder = new PaidPurchasesContentBuilder(unitOfWorkFactory, dailyStatisticsConfigurationModel);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenPosOperationTableIsEmpty()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            AssertExpectedPaidPurchasesContent(utcTimeInterval, expectedPosOperationsCount: 0, expectedTotalPrice: 0M);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenPosOperationWithoutCheckItems()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());
            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                .Build();

            Seeder.Seed(new Collection<PosOperation>
            {
                posOperation
            });

            AssertExpectedPaidPurchasesContent(utcTimeInterval, expectedPosOperationsCount: 0, expectedTotalPrice: 0M);
        }

        [TestCaseSource(typeof(PaidPurchasesContentBuilderCheckItemsDataGenerator))]
        public void ReturnExpectedContentWhenDifferentPosOperationsAreGiven(Collection<CheckItem> checkItems, int expectedPosOperation, decimal expectedTotalPrice)
        {
            var endUtcDateTime = DateTime.UtcNow.AddHours(1);
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(DefaultPosId, DefaultUserId));
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                .Build();

            Seeder.Seed(new Collection<PosOperation>
            {
                posOperation
            });

            Seeder.Seed(checkItems);

            AssertExpectedPaidPurchasesContent(utcTimeInterval, expectedPosOperation, expectedTotalPrice);
        }

        private void AssertExpectedPaidPurchasesContent(DateTimeRange utcDateTimeRange,
            int expectedPosOperationsCount, decimal expectedTotalPrice)
        {
            var resultContent =
                (PaidPurchasesContent)_contentBuilder.BuildContentWithLinkAsync(utcDateTimeRange).Result;

            resultContent.Should().NotBeNull();
            resultContent.PosOperationsCount.Should().Be(expectedPosOperationsCount);
            resultContent.TotalPrice.Should().Be(expectedTotalPrice);
        }
    }
}