using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.Purchases.DataGenerators;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Purchase.Conditional.Manager.Contracts;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.TestUtils.Extensions;

namespace NasladdinPlace.Api.Tests.Scenarios.Purchases
{
    public class ConditionalPurchasesManagerIntegrationTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 1;
        private const int DefaultFirstPosOperationId = 1;
        private const int DefaultSecondPosOperationId = 2;
        private const int DefaultFirstLabeledGoodId = 1;
        private const int DefaultSecondLabeledGoodId = 2;
        private const int DefaultBankTransactionId = 1;

        private IConditionalPurchaseManager _conditionalPurchaseManager;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));

            var serviceProvider = TestServiceProviderFactory.Create();
            _conditionalPurchaseManager = serviceProvider.GetRequiredService<IConditionalPurchaseManager>();

            Mapper.Reset();
            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));
        }

        [Test]
        public void
            DeleteUnverifiedCheckItemsInPurchasesAsync_CheckItemUnverifiedIsGiven_ShouldReturnExpectedCount(
                [ValueSource(typeof(ConditionalPurchasesDataGenerator), nameof(ConditionalPurchasesDataGenerator.GetUnverifiedTestCases))]
                ConditionalPurchasesDataGenerator.GetGeneratorFunc testCaseFunc )
        {
	        var testCase = testCaseFunc();

            var posOperationOfUserAndPosBuilder = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetDateStarted(default(DateTime));

            switch (testCase.Status)
            {
                case PosOperationStatus.Paid:

                    var bankTransactionInfo = new BankTransactionSummary(
                        Context.PaymentCards.First().Id,
                        DefaultBankTransactionId,
                        10M
                    );
                    var operationPaymentInfo =
                        OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionInfo, 5M);

                    posOperationOfUserAndPosBuilder
                        .MarkAsPendingPayment()
                        .MarkAsPaid(operationPaymentInfo);

                    break;

                case PosOperationStatus.Completed:
                    posOperationOfUserAndPosBuilder.MarkAsCompleted();

                    break;
            }

            var posOperation = posOperationOfUserAndPosBuilder
                .SetCheckItems(testCase.CheckItems.ToList())
                .Build();

            posOperation.SetProperty(nameof(PosOperation.DateCompleted), testCase.PosOperationDate);

            Insert(new Collection<PosOperation>
            {
                posOperation
            });

            var firstLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultFirstLabeledGoodId);
            var secondLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultSecondLabeledGoodId);

            var posOperationId = Context.PosOperations.FirstOrDefault()?.Id;
            if (posOperationId.HasValue)
            {
                firstLabeledGood.MarkAsUsedInPosOperation(posOperationId.Value);
                secondLabeledGood.MarkAsUsedInPosOperation(posOperationId.Value);
            }

            secondLabeledGood.MarkAsFoundInPos(DefaultPosId);

            Context.SaveChanges();

            _conditionalPurchaseManager.DeleteUnverifiedCheckItemsInConditionalPurchasesAsync().Wait();

            EnsureCheckItemsByStatusHasExpectedResult(testCase.ExpectedResult, testCase.ExpectedStatus);
        }

        [Test]
        public void
            DeleteUnverifiedCheckItemsInPurchasesAsync_CheckItemUnverifiedAreGiven_ShouldReturnExpectedCount()
        {
            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetDateStarted(default(DateTime))
                .MarkAsCompleted()
                .SetCheckItems(new Collection<CheckItem>
                {
                    ConditionalPurchasesDataGenerator.CreateCheckItem(DefaultPosId,
                        CheckItemStatus.Unverified,
                        DefaultFirstLabeledGoodId,
                        DefaultFirstPosOperationId,
                        10M),
                    ConditionalPurchasesDataGenerator.CreateCheckItem(DefaultPosId,
                        CheckItemStatus.Unverified,
                        DefaultSecondLabeledGoodId,
                        DefaultFirstPosOperationId,
                        25M)
                })
                .Build();

            posOperation.SetProperty(nameof(PosOperation.DateCompleted), DateTime.UtcNow.AddDays(-1));

            Insert(new Collection<PosOperation>
            {
                posOperation
            });

            var firstLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultFirstLabeledGoodId);
            var secondLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultSecondLabeledGoodId);
            
            var posOperationId = Context.PosOperations.FirstOrDefault()?.Id;
            if (posOperationId.HasValue)
            {
                firstLabeledGood.MarkAsUsedInPosOperation(posOperationId.Value);
                secondLabeledGood.MarkAsUsedInPosOperation(posOperationId.Value);
            }

            firstLabeledGood.MarkAsLostInPos(DefaultPosId);
            secondLabeledGood.MarkAsFoundInPos(DefaultPosId);

            Context.SaveChanges();

            _conditionalPurchaseManager.DeleteUnverifiedCheckItemsInConditionalPurchasesAsync().Wait();

            EnsureCheckItemsByStatusHasExpectedResult(0, CheckItemStatus.Unverified);
        }

        [Test]
        public void
            DeleteUnverifiedCheckItemsInPurchasesAsync_TwoDifferentPosOperationWithCheckItemUnverifiedAfterModificationByAdminAreGiven_ShouldReturnExpectedDeleteCheckItemsCount()
        {
            var firstPosOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                .SetCheckItems(
                    new List<CheckItem>
                    {
                        ConditionalPurchasesDataGenerator.CreateCheckItem(
                            DefaultPosId,
                            CheckItemStatus.Unverified,
                            DefaultFirstLabeledGoodId,
                            DefaultFirstPosOperationId,
                            5M)
                    })
                .Build();

            var secondPosOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                .SetCheckItems(
                    new List<CheckItem>
                    {
                        ConditionalPurchasesDataGenerator.CreateCheckItem(
                            DefaultPosId,
                            CheckItemStatus.Unverified,
                            DefaultSecondLabeledGoodId,
                            DefaultSecondPosOperationId,
                            5M, true)
                    })
                .Build();

            firstPosOperation.SetProperty(nameof(PosOperation.DateCompleted), DateTime.UtcNow.AddDays(-1));
            secondPosOperation.SetProperty(nameof(PosOperation.DateCompleted), DateTime.UtcNow.AddDays(-1));

            Insert(new Collection<PosOperation>
            {
                firstPosOperation,
                secondPosOperation
            });

            var firstLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultFirstLabeledGoodId);
            var secondLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultSecondLabeledGoodId);

            firstLabeledGood.MarkAsLostInPos(DefaultPosId);
            secondLabeledGood.MarkAsFoundInPos(DefaultPosId);

            Context.SaveChanges();

            _conditionalPurchaseManager.DeleteUnverifiedCheckItemsInConditionalPurchasesAsync().Wait();

            EnsureCheckItemsByStatusHasExpectedResult(1, CheckItemStatus.Deleted);
        }

        [Test]
        public void
            MarkPurchasedCheckItemsAsUnverifiedIfAppearedAfterPurchaseAsync_CheckItemWithDifferentCheckItemStatusAreGiven_ShouldReturnExpectedCount(
                [ValueSource(typeof(ConditionalPurchasesDataGenerator), nameof(ConditionalPurchasesDataGenerator.GetPaidTestCases))]
                ConditionalPurchasesDataGenerator.GetGeneratorFunc testCaseFunc )
        {
	        var testCase = testCaseFunc();

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetDateStarted(default(DateTime))
                .MarkAsCompleted()
                .SetCheckItems(testCase.CheckItems.ToList())
                .Build();

            posOperation.SetProperty(nameof(PosOperation.DateCompleted), testCase.PosOperationDate);

            Insert(new Collection<PosOperation>
            {
                posOperation
            });

            var firstLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultFirstLabeledGoodId);
            var secondLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultSecondLabeledGoodId);
            
            var posOperationId = Context.PosOperations.FirstOrDefault()?.Id;
            if (posOperationId.HasValue)
            {
                firstLabeledGood.MarkAsUsedInPosOperation(posOperationId.Value);
                secondLabeledGood.MarkAsUsedInPosOperation(posOperationId.Value);
            }

            firstLabeledGood.MarkAsLostInPos(DefaultPosId);
            secondLabeledGood.MarkAsFoundInPos(DefaultPosId);

            Context.SaveChanges();

            _conditionalPurchaseManager.MarkPurchasedCheckItemsAsUnverifiedIfAppearedAfterPurchaseAsync().Wait();

            EnsureCheckItemsByStatusHasExpectedResult(testCase.ExpectedResult, testCase.ExpectedStatus);
        }

        [Test]
        public void
            MarkPurchasedCheckItemsAsUnverifiedIfAppearedAfterPurchaseAsync_TwoDifferentPosOperationWithCheckItemPaidAfterModificationByAdminAreGiven_ShouldReturnExpectedPaidUnverifiedCheckItemsCount()
        {
            var firstPosOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                .SetCheckItems(
                    new List<CheckItem>
                    {
                        ConditionalPurchasesDataGenerator.CreateCheckItem(
                            DefaultPosId,
                            CheckItemStatus.Paid,
                            DefaultFirstLabeledGoodId,
                            DefaultFirstPosOperationId,
                            5M)
                    })
                .Build();

            var secondPosOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                .SetCheckItems(
                    new List<CheckItem>
                    {
                        ConditionalPurchasesDataGenerator.CreateCheckItem(
                            DefaultPosId,
                            CheckItemStatus.Paid,
                            DefaultSecondLabeledGoodId,
                            DefaultSecondPosOperationId,
                            5M, true)
                    })
                .Build();

            firstPosOperation.SetProperty(nameof(PosOperation.DateCompleted), DateTime.UtcNow.AddDays(-1));
            secondPosOperation.SetProperty(nameof(PosOperation.DateCompleted), DateTime.UtcNow.AddDays(-1));

            Insert(new Collection<PosOperation>
            {
                firstPosOperation,
                secondPosOperation
            });

            var firstLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultFirstLabeledGoodId);
            var secondLabeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Id == DefaultSecondLabeledGoodId);

            firstLabeledGood.MarkAsLostInPos(DefaultPosId);
            secondLabeledGood.MarkAsFoundInPos(DefaultPosId);

            Context.SaveChanges();

            _conditionalPurchaseManager.MarkPurchasedCheckItemsAsUnverifiedIfAppearedAfterPurchaseAsync().Wait();

            EnsureCheckItemsByStatusHasExpectedResult(1, CheckItemStatus.PaidUnverified);
        }

        private void EnsureCheckItemsByStatusHasExpectedResult(int expectedResult, CheckItemStatus status)
        {
            Context.CheckItems.AsNoTracking().Should().HaveCountGreaterThan(0);

            var checkItems = Context.CheckItems.AsNoTracking().Where(ci => ci.Status == status);

            checkItems.Should().HaveCount(expectedResult);
        }

        private void Insert<T>(IEnumerable<T> entities) where T : class
        {
            Seeder.Seed(entities);
        }
    }
}
