using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.Spreadsheet.DataProviders.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;

namespace NasladdinPlace.Api.Tests.Scenarios.Spreadsheet.DataProviders
{
    public class PointsOfSaleContentReportDataProviderShould : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;
        private const int DefaultGoodId = 1;
        private const string DefaultLabel = "74 72 61 63 65 00 02 05 51 37 bb e3 b2 6f e2 80 11 00 20 00 77 93 26 f7 08 aac";
        private const int DefaultCurrencyId = 1;
        private const int DefaultSize = 25;
        private const int DefaultExportPeriod = 100;

        private IReportDataBatchProvider _reportDataBatchProvider;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            Seeder.Seed(new PointsOfSaleDataSet());

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();
            var reportDataProviderFactory = serviceProvider.GetRequiredService<IReportDataBatchProviderFactory>();
            _reportDataBatchProvider = reportDataProviderFactory.Create(ReportType.PointsOfSaleContent, TimeSpan.FromDays(DefaultExportPeriod));
        }

        [Test]
        public void ReturnRecordWhenTieLabeledGoodIsGiven()
        {
            Seeder.Seed(new List<LabeledGood>
            {
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .TieToGood(
                        DefaultGoodId,
                        new LabeledGoodPrice(5M, DefaultCurrencyId),
                        new ExpirationPeriod()
                    ).Build()
            });
            var reportRecords = GetReportRecords();
            reportRecords.Should().NotBeEmpty();
            AssertReportRecordEquals(reportRecords.FirstOrDefault(), new ExpectedPosGoodReportRecord(5M, 5M));
        }

        [Test]
        public void ReturnEmptyListWhenUntieLabeledGoodIsGiven()
        {
            Seeder.Seed(new List<LabeledGood>
            {
                LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                    .Build()
            });

            GetReportRecords().Should().BeEmpty();
        }

        private IList<PosGoodReportRecord> GetReportRecords()
        {
            var allRecords = new List<PosGoodReportRecord>();
            foreach (var uploadingInfo in _reportDataBatchProvider.Provide(DefaultSize))
            {
                allRecords.AddRange(uploadingInfo.Records.Cast<PosGoodReportRecord>());
            }

            return allRecords;
        }

        private void AssertReportRecordEquals(PosGoodReportRecord actualPosGoodReportRecord, ExpectedPosGoodReportRecord expectedPosGoodReportRecord)
        {
            var expectedPos = Context.PointsOfSale.SingleOrDefault(p => p.Id == DefaultPosId);
            var expectedGood = Context.Goods.SingleOrDefault(g => g.Id == DefaultGoodId);
            var expectedCategory = Context.GoodCategories.SingleOrDefault(g => g.Id == GoodCategory.Default.Id);

            actualPosGoodReportRecord.PosId.Should().Be(expectedPos.Id);
            actualPosGoodReportRecord.PosName.Should().Be(expectedPos.Name);
            actualPosGoodReportRecord.Price.Should().Be(expectedPosGoodReportRecord.ExpectedPrice);
            actualPosGoodReportRecord.PricePerItem.Should().Be(expectedPosGoodReportRecord.ExpectedPricePerItem);
            actualPosGoodReportRecord.GoodId.Should().Be(expectedGood.Id);
            actualPosGoodReportRecord.GoodName.Should().Be(expectedGood.Name);
            actualPosGoodReportRecord.GoodCategoryId.Should().Be(expectedCategory.Id);
            actualPosGoodReportRecord.GoodCategory.Should().Be(expectedCategory.Name);
        }
    }
}