using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Services.Spreadsheet.Converters;
using NasladdinPlace.Api.Services.Spreadsheet.Factories.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.Spreadsheet.DataProviders.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;

namespace NasladdinPlace.Api.Tests.Scenarios.Spreadsheet.DataProviders
{
    public class PurchaseReportDataProviderShould : TestsBase
    {
        private const int DefaultSize = 25;
        private const int DefaultExportPeriod = 100;
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;
        private const int DefaultPaymentCardId = 1;
        private const int DefaultTransactionId = 1;
        private const int DefaultFirstLabeledGoodId = 1;
        private const int DefaultSecondLabeledGoodId = 1;   
        private const int DefaultFirstPosOperationId = 1;
        private const int DefaultSecondPosOperationId = 2;
        private const int DefaultGoodId = 1;
        private const int DefaultCurrencyId = 1;

        private IReportDataBatchProvider _reportDataBatchProvider;
        private IReportFieldConverter _booleanConverter;

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
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();
            var reportDataProviderFactory = serviceProvider.GetRequiredService<IReportDataBatchProviderFactory>();
            _reportDataBatchProvider = reportDataProviderFactory.Create(ReportType.DailyPurchaseStatistics, TimeSpan.FromDays(DefaultExportPeriod));
            _booleanConverter = new BooleanReportFieldConverter();
        }

        [Test]
        public void ReturnEmptyListWhenPosOperationIsNotGiven()
        {
            GetReportRecords().Should().BeEmpty();
        }

        [TestCase(CheckItemStatus.Unpaid, 3, 2, 3, 2, false)]
        [TestCase(CheckItemStatus.Paid, 5, 0, 5, 0, false)]
        [TestCase(CheckItemStatus.PaidUnverified, 2, 3, 2, 3, true)]
        public void ReturnRecordWhenPaidPosOperationWithCheckItemAndDifferentPaymentAreGiven(CheckItemStatus initialCheckItemStatus,
            decimal initialAmountMoney, decimal initialBonus,
            decimal expectedActualPrice, decimal expectedBonuses, bool expectedIsConditionalPurchase)
        {
            CreatePosOperationPaidViaMixPaymentOrViaMoney(new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultFirstPosOperationId,
                        DefaultGoodId,
                        DefaultFirstLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            }, initialAmountMoney, initialBonus, checkItemStatus:initialCheckItemStatus);

            AssertPurchaseReportRecordsEqual(new List<ExpectedPurchaseReportRecord>
            {
                new ExpectedPurchaseReportRecord(expectedActualPrice, expectedBonuses, 0M, 5M, 5M, 1, expectedIsConditionalPurchase)
            }, GetReportRecords());
        }

        [Test]
        public void ReturnRecordWhenFullPayedViaBonusesPosOperationWithCheckItemWithDiscountIsGiven()
        {
            var checkItem = CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultFirstPosOperationId,
                        DefaultGoodId,
                        DefaultFirstLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build();

            checkItem.AddDiscount(20M);

            AddBonusPointsForUser(DefaultUserId, 4M);

            var operationPaymentInfo =
                OperationPaymentInfo.ForPaymentViaBonuses(DefaultUserId, 4M);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetCheckItems(new List<CheckItem>
                {
                    checkItem
                })
                .Build();

            Seeder.Seed(new List<PosOperation>
            {
                posOperation
            });

            AddBonusPointsToPosOperationAndMarkAsPaid(operationPaymentInfo, DefaultFirstPosOperationId);

            AssertPurchaseReportRecordsEqual(new List<ExpectedPurchaseReportRecord>
            {
                new ExpectedPurchaseReportRecord(0M, 4M, 1M, 5M, 5M, 1, false)
            }, GetReportRecords());
        }

        [TestCase(CheckItemStatus.Unpaid, 4, 0, 20, 4, 0, 1, 5, false)]
        [TestCase(CheckItemStatus.Paid, 2, 2, 20, 2, 2, 1, 5, false)]
        [TestCase(CheckItemStatus.PaidUnverified, 3, 3, 40, 0, 3, 2, 5, true)]
        public void ReturnRecordWhenPaidPosOperationWithCheckItemWithDiscountIsGiven(
            CheckItemStatus initialCheckItemStatus, decimal initialAmountMoney, decimal initialBonus, decimal initialDiscount,
            decimal expectedActualPrice, decimal expectedBonuses,
            decimal expectedDiscount, decimal expectedPrice, bool expectedIsConditionalPurchase)
        {
            var checkItem = CheckItem.NewBuilder(
                    DefaultPosId,
                    DefaultFirstPosOperationId,
                    DefaultGoodId,
                    DefaultFirstLabeledGoodId,
                    DefaultCurrencyId)
                .SetPrice(5M)
                .SetStatus(CheckItemStatus.Unpaid)
                .Build();
  
            checkItem.AddDiscount(initialDiscount);

            CreatePosOperationPaidViaMixPaymentOrViaMoney(new List<CheckItem>
            {
                checkItem
            }, initialAmountMoney, initialBonus, checkItemStatus: initialCheckItemStatus);

            AssertPurchaseReportRecordsEqual(new List<ExpectedPurchaseReportRecord>
            {
                new ExpectedPurchaseReportRecord(expectedActualPrice, expectedBonuses, expectedDiscount, 5M, expectedPrice, 1, expectedIsConditionalPurchase)
            }, GetReportRecords());
        }

        [TestCase(CheckItemStatus.Deleted)]
        [TestCase(CheckItemStatus.Refunded)]
        [TestCase(CheckItemStatus.Unverified)]
        public void ReturnEmptyListWhenPaidOrUnpaidCheckItemIsNotGiven(CheckItemStatus initialCheckItemStatus)
        {
            CreatePosOperationPaidViaMixPaymentOrViaMoney(new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultFirstPosOperationId,
                        DefaultGoodId,
                        DefaultFirstLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(initialCheckItemStatus)
                    .Build()
            }, 5M, 0M, checkItemStatus:initialCheckItemStatus);

            GetReportRecords().Should().BeEmpty();
        }

        [Test]
        public void ReturnEmptyListWhenPosOperationPaidEarlierThanReportDefaultPeriod()
        {
            CreatePosOperationPaidEarlierThanReportExportPeriod(new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultFirstPosOperationId,
                        DefaultGoodId,
                        DefaultFirstLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Paid)
                    .Build()
            }, 5M, 0M);

            GetReportRecords().Should().BeEmpty();
        }

        [Test]
        public void ReturnRecordWhenPosOperationWithPaidOrUnpaidCheckItemsAndBonusAreGiven()
        {
            CreatePosOperationPaidViaMixPaymentOrViaMoney(new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultFirstPosOperationId,
                        DefaultGoodId,
                        DefaultFirstLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build(),

                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultSecondPosOperationId,
                        DefaultGoodId,
                        DefaultSecondLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.PaidUnverified)
                    .Build()
            }, 8M, 2M);

            AssertPurchaseReportRecordsEqual(new List<ExpectedPurchaseReportRecord>
            {
                new ExpectedPurchaseReportRecord(3M, 2M, 0M, 5M, 5M, 1, true),
                new ExpectedPurchaseReportRecord(5M, 0M, 0M, 5M, 5M, 1, false)
            }, GetReportRecords());
        }

        [Test]
        public void ReturnRecordWhenPosOperationOneOfCheckItemsWithDiscountAndBonusAreGiven()
        {
            var firstCheckItem = CheckItem.NewBuilder(
                    DefaultPosId,
                    DefaultFirstPosOperationId,
                    DefaultGoodId,
                    DefaultFirstLabeledGoodId,
                    DefaultCurrencyId)
                .SetPrice(5M)
                .SetStatus(CheckItemStatus.Unpaid)
                .Build();

            firstCheckItem.AddDiscount(20M);

            CreatePosOperationPaidViaMixPaymentOrViaMoney(new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultSecondPosOperationId,
                        DefaultGoodId,
                        DefaultSecondLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.PaidUnverified)
                    .Build(), firstCheckItem
            }, 6M, 3M, checkItemStatus:CheckItemStatus.PaidUnverified);

            AssertPurchaseReportRecordsEqual(new List<ExpectedPurchaseReportRecord>
            {
                new ExpectedPurchaseReportRecord(2M, 3M, 0M, 5M, 5M, 1, true),
                new ExpectedPurchaseReportRecord(4M, 0M, 1M, 5M, 5M, 1, false)
            }, GetReportRecords());
        }

        [Test]
        public void ReturnRecordsWhenPosOperationsWithDifferentCheckItemsIsGiven()
        {
            CreatePosOperationPaidViaMixPaymentOrViaMoney(new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultFirstPosOperationId,
                        DefaultGoodId,
                        DefaultFirstLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            }, 5M, 0M);

            CreatePosOperationPaidViaMixPaymentOrViaMoney(new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultSecondPosOperationId,
                        DefaultGoodId,
                        DefaultSecondLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            }, 3M, 2M, DefaultSecondPosOperationId, CheckItemStatus.PaidUnverified);

            AssertPurchaseReportRecordsEqual(new List<ExpectedPurchaseReportRecord>
            {
                new ExpectedPurchaseReportRecord(3M, 2M, 0M, 5M, 5M, 1, true),
                new ExpectedPurchaseReportRecord(5M, 0M, 0M, 5M, 5M, 1, false)            
            }, GetReportRecords());
        }

        private void CreatePosOperationPaidViaMixPaymentOrViaMoney(
            IEnumerable<CheckItem> checkItems,
            decimal amountMoney, 
            decimal bonusPointsAmount,
            int posOperationId = DefaultFirstPosOperationId,
            CheckItemStatus checkItemStatus = CheckItemStatus.Paid)
        {
            var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, amountMoney);

            AddBonusPointsForUser(DefaultUserId, bonusPointsAmount);

            var operationPaymentInfo =
                OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, bonusPointsAmount);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetCheckItems(checkItems.ToList())
                .Build();

            Seeder.Seed(new List<PosOperation>
            {
                posOperation
            });

            AddBonusPointsToPosOperationAndMarkAsPaid(operationPaymentInfo, posOperationId);

            var checkItem = Context.CheckItems.OrderBy( i => i.Id ).First(cki => cki.PosOperationId == posOperationId);
            if (checkItemStatus != checkItem.Status)
            {
                checkItem.SetProperty(nameof(checkItem.Status),checkItemStatus);
                Context.SaveChanges();
            }
        }

        private void AddBonusPointsToPosOperationAndMarkAsPaid(OperationPaymentInfo operationPaymentInfo, int posOperationId)
        {
            var savedPosOperation =
                Context.PosOperations.First(po => po.Id == posOperationId);
            savedPosOperation.WriteOffBonusPoints();
            savedPosOperation.MarkAs(PosOperationStatus.PendingPayment);
            savedPosOperation.MarkAsPaid(operationPaymentInfo);

            Context.SaveChanges();
        }

        private void CreatePosOperationPaidEarlierThanReportExportPeriod(IEnumerable<CheckItem> checkItems, decimal amountMoney, decimal amountBonuses)
        {
            var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, amountMoney);

            var operationPaymentInfo =
                OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, amountBonuses);
           
            var oldPosOperationDate = DateTime.UtcNow.AddDays(-(DefaultExportPeriod + 1));
            
            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(operationPaymentInfo)
                .SetCheckItems(checkItems.ToList())
                .Build();

            posOperation.SetProperty(nameof(PosOperation.DatePaid), oldPosOperationDate);
            posOperation.SetProperty(nameof(PosOperation.DateCompleted), oldPosOperationDate);
            posOperation.SetProperty(nameof(PosOperation.DateStarted), oldPosOperationDate);

            Seeder.Seed(new List<PosOperation>
            {
                posOperation
            });
        }

        private IList<PurchaseReportRecord> GetReportRecords()
        {
            var allRecords = new List<PurchaseReportRecord>();
            foreach (var uploadingInfo in _reportDataBatchProvider.Provide(DefaultSize))
            {
                allRecords.AddRange(uploadingInfo.Records.Cast<PurchaseReportRecord>());
            }

            return allRecords;
        }

        private void AssertReportRecordEquals(PurchaseReportRecord actualPurchaseReportRecord, ExpectedPurchaseReportRecord expectedPurchaseReportRecord)
        {
            var expectedPos = Context.PointsOfSale.SingleOrDefault(p => p.Id == DefaultPosId);
            var expectedUser = Context.Users.SingleOrDefault(u => u.Id == DefaultUserId);
            var expectedGood = Context.Goods.SingleOrDefault(g => g.Id == DefaultGoodId);

            actualPurchaseReportRecord.UserId.Should().Be(expectedUser.Id);
            actualPurchaseReportRecord.UserName.Should().Be(expectedUser.UserName);
            actualPurchaseReportRecord.PosId.Should().Be(expectedPos.Id);
            actualPurchaseReportRecord.PosName.Should().Be(expectedPos.Name);
            actualPurchaseReportRecord.ActualPrice.Should().Be(expectedPurchaseReportRecord.ExpectedActualPrice);
            actualPurchaseReportRecord.Bonuses.Should().Be(expectedPurchaseReportRecord.ExpectedBonuses);
            actualPurchaseReportRecord.Discount.Should().Be(expectedPurchaseReportRecord.ExpectedDiscount);
            actualPurchaseReportRecord.Price.Should().Be(expectedPurchaseReportRecord.ExpectedPrice);
            actualPurchaseReportRecord.PricePerItem.Should().Be(expectedPurchaseReportRecord.ExpectedPricePerItem);
            actualPurchaseReportRecord.GoodId.Should().Be(expectedGood.Id);
            actualPurchaseReportRecord.GoodName.Should().Be(expectedGood.Name);
            actualPurchaseReportRecord.GoodCount.Should().Be(expectedPurchaseReportRecord.ExpectedGoodCount);
            actualPurchaseReportRecord.IsConditionalPurchase.Should()
                .Be(_booleanConverter.Convert(expectedPurchaseReportRecord.ExpectedIsConditionalPurchase));
        }

        private void AssertPurchaseReportRecordsEqual(IEnumerable<ExpectedPurchaseReportRecord> expected,
            IEnumerable<PurchaseReportRecord> actual)
        {
            var actualList = actual.ToList();
            var expectedList = expected.ToList();

            actualList.Should().NotBeNull();
            actualList.Count.Should().Be(expectedList.Count);

            for (var index = 0; index < expectedList.Count; index++)
            {
                AssertReportRecordEquals(actualList[index], expectedList[index]);
            }
        }

        private void AddBonusPointsForUser(int userId, decimal bonus)
        {
            var user = Context.Users.Single(u => u.Id == userId);
            user.AddBonusPoints(bonus, BonusType.Payment);
            Context.SaveChanges();
        }
    } 
}
