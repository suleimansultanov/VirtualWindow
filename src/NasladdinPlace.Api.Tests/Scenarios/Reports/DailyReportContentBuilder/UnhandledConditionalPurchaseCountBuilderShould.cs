using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder.DataGenerators;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
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
    public class UnhandledConditionalPurchaseCountBuilderShould : TestsBase
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
            var dailyStatisticsConfigurationModel = new DailyStatisticsConfigurationModel()
            {
                DailyUnhandledConditionalPurchasesCountLink = "{0}/Checks?OperationDateFrom={1}&OperationDateUntil={2}&HasUnverifiedCheckItems={3}&OperationMode={4}",
                AdminPageBaseUrl = "http://nursultanapi.nasladdin.club"
            };

            var serviceProvider = TestServiceProviderFactory.Create();
            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _contentBuilder = new UnhandledConditionalPurchaseCountBuilder(unitOfWorkFactory, dailyStatisticsConfigurationModel);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenPosOperationIsEmpty()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            AssertExpectedConditionalPurchaseCountContent(utcTimeInterval, expectedUnhandledConditionalPurchasesCount: 0);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenPosOperationWithoutCheckItems()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(DefaultPosId, DefaultUserId));
            AssertExpectedConditionalPurchaseCountContent(utcTimeInterval, expectedUnhandledConditionalPurchasesCount: 0);
        }

        [TestCaseSource(typeof(UnhandledConditionalPurchasesContentBuilderCheckItemsDataGenerator))]
        public void ReturnExpectedContentWhenDifferentPosOperationsAreGiven(Collection<CheckItem> checkItems,
            int expectedUnhandledConditionalPurchasesCount)
        {
            var endUtcDateTime = DateTime.UtcNow.AddDays(-1).AddHours(1);
            var startUtcDateTime = endUtcDateTime.AddDays(-1).AddHours(-2);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(DefaultPosId, DefaultUserId));
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .SetMode(PosMode.Purchase)
                .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                .Build();

            posOperation.MarkAuditRequested();

            var unhandledConditionalPurchaseDateTime = DateTime.UtcNow.AddDays(-2);
            if (expectedUnhandledConditionalPurchasesCount > 0)
                posOperation.SetProperty(nameof(PosOperation.AuditRequestDateTime), unhandledConditionalPurchaseDateTime);
            posOperation.SetProperty(nameof(PosOperation.DatePaid), unhandledConditionalPurchaseDateTime);
            Seeder.Seed(new Collection<PosOperation>
            {
                posOperation
            });
            Seeder.Seed(checkItems);

            AssertExpectedConditionalPurchaseCountContent(utcTimeInterval, expectedUnhandledConditionalPurchasesCount);
        }

        private void AssertExpectedConditionalPurchaseCountContent(DateTimeRange utcDateTimeRange,
            int expectedUnhandledConditionalPurchasesCount)
        {
            var resultContent = (UnhandledConditionalPurchaseCountContent)_contentBuilder.BuildContentWithLinkAsync(utcDateTimeRange).GetAwaiter().GetResult();

            resultContent.Should().NotBeNull();
            resultContent.UnhandledConditionalPurchasesCount.Should().Be(expectedUnhandledConditionalPurchasesCount);
        }
    }
}