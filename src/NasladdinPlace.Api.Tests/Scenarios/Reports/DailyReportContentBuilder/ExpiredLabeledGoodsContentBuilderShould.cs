using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder.DataGenerators;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Interfaces;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.Utilities.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Reports.DailyReports.DailyStatistics.Builders.Models.ConfigurationModels;

namespace NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder
{
    public class ExpiredLabeledGoodsContentBuilderShould : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;
        private const int DefaultPosOperationId = 1;

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

            _contentBuilder = new ExpiredLabeledGoodsBuilder(unitOfWorkFactory, dailyStatisticsConfigurationModel);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenPosOperationIsEmpty()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            AssertExpectedConditionalPurchaseCountContent(utcTimeInterval, expectedSumOfExpiredLabeledGoods: 0);
        }

        [Test]
        public void ReturnDefaultEmptyContentWhenPosOperationWithoutCheckItems()
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(DefaultPosId, DefaultUserId));
            AssertExpectedConditionalPurchaseCountContent(utcTimeInterval, expectedSumOfExpiredLabeledGoods: 0);
        }

        [TestCaseSource(typeof(ExpiredLabeledGoodsDataGenerator))]
        public void ReturnExpectedContentWhenDifferentPosModeIsGiven(Collection<LabeledGood> labeledGoods,
            decimal expectedSumOfExpiredLabeledGoods, PosMode mode)
        {
            var endUtcDateTime = DateTime.UtcNow;
            var startUtcDateTime = endUtcDateTime.AddDays(-1);

            var utcTimeInterval = DateTimeRange.From(startUtcDateTime, endUtcDateTime);

            Seeder.Seed(new UsersDataSet());

            foreach (var label in labeledGoods)
            {
                if (mode != PosMode.Purchase)
                    label.SetProperty(nameof(LabeledGood.ExpirationDate), startUtcDateTime.AddDays(-1));
            }

            Seeder.Seed(labeledGoods);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetMode(mode)
                .SetCheckItems(new List<CheckItem>
                {
                    CreateCheckItem(Context.LabeledGoods.First(lg => lg.Label == ExpiredLabeledGoodsDataGenerator.FirstLabel)
                        .Id),
                    CreateCheckItem(Context.LabeledGoods.First(lg => lg.Label == ExpiredLabeledGoodsDataGenerator.SecondLabel)
                        .Id)
                })
                .Build();
            posOperation.SetProperty(nameof(PosOperation.DateCompleted), endUtcDateTime.AddHours(-10));

            Seeder.Seed(new List<PosOperation>
            {
                posOperation
            });

            AssertExpectedConditionalPurchaseCountContent(utcTimeInterval, expectedSumOfExpiredLabeledGoods);
        }

        private void AssertExpectedConditionalPurchaseCountContent(DateTimeRange utcDateTimeRange,
            decimal expectedSumOfExpiredLabeledGoods)
        {
            var resultContent = (ExpiredLabeledGoodsContent) _contentBuilder
                .BuildContentWithLinkAsync(utcDateTimeRange)
                .GetAwaiter()
                .GetResult();

            resultContent.Should().NotBeNull();
            resultContent.SumOfExpiredLabeledGoods.Should().Be(expectedSumOfExpiredLabeledGoods);
        }

        private CheckItem CreateCheckItem(int labeledGoodId)
        {
            return CheckItem.NewBuilder(
                    DefaultPosId,
                    DefaultPosOperationId,
                    ExpiredLabeledGoodsDataGenerator.DefaultGoodId,
                    labeledGoodId,
                    ExpiredLabeledGoodsDataGenerator.DefaultCurrencyId)
                .SetPrice(ExpiredLabeledGoodsDataGenerator.DefaultLabeledGoodPrice)
                .Build();
        }
    }
}