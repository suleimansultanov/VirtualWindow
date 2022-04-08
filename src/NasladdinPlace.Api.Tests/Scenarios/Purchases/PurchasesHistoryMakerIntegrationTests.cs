using AutoMapper;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.PurchasesHistoryMaker;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Api.Tests.Scenarios.Purchases.Models;
using NasladdinPlace.TestUtils.Extensions;

namespace NasladdinPlace.Api.Tests.Scenarios.Purchases
{
    public class PurchasesHistoryMakerIntegrationTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 1;

        private IPurchasesHistoryMaker _purchasesHistoryMaker;

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

            Mapper.Reset();
            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();
            _purchasesHistoryMaker = serviceProvider.GetRequiredService<IPurchasesHistoryMaker>();
        }

        [Test]
        public void MakePurchasesHistoryOfUserAsync_PosOperationWithoutCheckItemsIsGiven_ShouldReturnEmptyPurchasesHistory()
        {
            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build()
            });
            EnsurePurchasesHistoryIsEmpty();
        }

        [TestCase(CheckItemStatus.Refunded)]
        [TestCase(CheckItemStatus.Deleted)]
        public void MakePurchasesHistoryOfUserAsync_PosOperationWithRefundedOrDeletedCheckItemIsGiven_ShouldReturnEmptyPurchasesHistory(CheckItemStatus inputCheckItemStatus)
        {
            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build()
            });

            Insert(new Collection<CheckItem>
            {
                CreateCheckItem(DefaultPosId, inputCheckItemStatus, 1, 1, 10M)
            });

            EnsurePurchasesHistoryIsEmpty();
        }

        [Test]
        public void MakePurchasesHistoryOfUserAsync_CheckItemInPosOperationIsGiven_ShouldReturnExpectedCountChecksInHistory()
        {
            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(new DateTime(2010, 3, 25))
                    .Build()
            });

            Insert(new Collection<CheckItem>
            {
                CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, 1, 1, 10M)
            });
            EnsurePurchasesHistoryHasExpectedResult(1, new Collection<ExpectedCheck>()
            {
                new ExpectedCheck(
                    id:1, 
                    quantity: 1,
                    totalPrice: 10M,
                    expectedTotalPriceWithDiscount:10M,
                    expectedCheckItemsCount: 1,
                    expectedDateTime: 25.March(2010), expectedBonus: 0M)
            });
        }

        [TestCase(CheckItemStatus.Refunded, CheckItemStatus.Unpaid, 5, 5, 1, 5, 2)]
        [TestCase(CheckItemStatus.Refunded, CheckItemStatus.Unpaid, 5, 10, 1, 10, 2)]
        [TestCase(CheckItemStatus.Deleted, CheckItemStatus.Unpaid, 5, 5, 1, 5, 1)]
        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Unpaid, 5, 5, 2, 10, 1)]
        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Unpaid, 5, 10, 2, 15, 2)]
        public void
            MakePurchasesHistoryOfUserAsync_DifferentCheckItemsActionAndPriceInPosOperationAreGiven_ShouldReturnExpectedChecksCountInHistory(
                CheckItemStatus firstCheckItemStatus, 
                CheckItemStatus secondCheckItemStatus, 
                decimal firstCheckItemPrice, 
                decimal secondCheckItemPrice,
                int expectedQuantity, 
                decimal expectedTotalPrice, 
                int expectedItemsCount)
        {
            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(new DateTime(2010, 3, 25))
                    .Build()
            });

            Insert(new Collection<CheckItem>()
            {
                CreateCheckItem(DefaultPosId, firstCheckItemStatus, 1, 1, firstCheckItemPrice),
                CreateCheckItem(DefaultPosId, secondCheckItemStatus, 2, 1, secondCheckItemPrice)
            });
            EnsurePurchasesHistoryHasExpectedResult(1, new Collection<ExpectedCheck>()
            {
                new ExpectedCheck(
                    id: 1,
                    quantity: expectedQuantity,
                    totalPrice: expectedTotalPrice,
                    expectedTotalPriceWithDiscount: expectedTotalPrice,
                    expectedCheckItemsCount: expectedItemsCount,
                    expectedDateTime: 25.March(2010), expectedBonus: 0M)
            });
        }

        [Test]
        public void MakePurchasesHistoryOfUserAsync_NoCheckItemsInPosOperationsAreGiven_ShouldReturnEmptyPurchasesHistory()
        {
            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build()
            });
            EnsurePurchasesHistoryIsEmpty();
        }

        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Unpaid, 2)]
        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Refunded, 1)]
        public void MakePurchasesHistoryOfUserAsync_DifferentCheckItemsInPosOperationsAreGiven_ShouldReturnExpectedCountChecksInHistory(
            CheckItemStatus firstCheckItemStatus,
            CheckItemStatus secondCheckItemStatus,
            int expectedChecksCount)
        {
            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(new DateTime(2010, 3, 25))
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(new DateTime(2011, 4, 15))
                    .Build()
            });

            Insert(new Collection<CheckItem>
            {
                CreateCheckItem(DefaultPosId, firstCheckItemStatus, 1, 1, 10M),
                CreateCheckItem(DefaultPosId, secondCheckItemStatus, 2, 2, 10M)
            });
            EnsurePurchasesHistoryHasExpectedResult(expectedChecksCount, new Collection<ExpectedCheck>
            {
                new ExpectedCheck(
                    id: Context.PosOperations.OrderBy(po => po.DateStarted).First().Id,
                    quantity: 1,
                    totalPrice: 10M,
                    expectedTotalPriceWithDiscount: 10M,
                    expectedCheckItemsCount: 1,
                    expectedDateTime: 25.March(2010), expectedBonus: 0M),
                new ExpectedCheck(
                    id: Context.PosOperations.OrderByDescending(po => po.DateStarted).First().Id,
                    quantity: 1,
                    totalPrice: 10M,
                    expectedTotalPriceWithDiscount: 10M,
                    expectedCheckItemsCount: 1,
                    expectedDateTime: 15.April(2011), expectedBonus: 0M)
            });
        }

        [Test]
        public void MakePurchasesHistoryOfUserAsync_PosOperationsAreTakenWithRefundAndDeleteCheckItems_ShouldReturnEmptyPurchasesHistory()
        {
            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(new DateTime(2010,3,25))
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(new DateTime(2011, 4, 26))
                    .Build()
            });        
            Insert(new Collection<CheckItem>
            {
                CreateCheckItem(DefaultPosId, CheckItemStatus.Refunded, 1, 1, 10M),
                CreateCheckItem(DefaultPosId, CheckItemStatus.Deleted, 2, 2, 10M)
            });
            EnsurePurchasesHistoryIsEmpty();
        }

        [TestCase(CheckItemStatus.Refunded, CheckItemStatus.Unpaid, 1, 5, 4, 2)]
        [TestCase(CheckItemStatus.Deleted, CheckItemStatus.Unpaid, 1, 5, 4, 1)]
        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Deleted, 1, 5, 5, 1)]
        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Unpaid, 2, 10, 9, 2)]
        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Paid, 2, 10, 9, 2)]
        [TestCase(CheckItemStatus.PaidUnverified, CheckItemStatus.Paid, 2, 10, 9, 2)]
        public void MakePurchasesHistoryOfUserAsync_DifferentCheckItemsInPosOperationAreGivenAndOneOfWhichHaveDiscount_ShouldReturnEmptyPurchasesHistory(
            CheckItemStatus firstCheckItemStatus,
            CheckItemStatus secondCheckItemStatus,
            int expectedQuantity,
            decimal expectedTotalPrice,
            decimal expectedPriceWithDiscount,
            int expectedItemsCount)
        {
            var firstCheckItem = CreateCheckItem(DefaultPosId, firstCheckItemStatus, 1, 1, 5M);

            var secondCheckItem = CreateCheckItem(DefaultPosId, secondCheckItemStatus, 2, 1, 5M);
            secondCheckItem.AddDiscount(20M);

            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(new DateTime(2010,3,25))
                    .SetCheckItems(new Collection<CheckItem>
                    {
                        firstCheckItem,
                        secondCheckItem
                    })
                    .Build()
            });
            EnsurePurchasesHistoryHasExpectedResult(1, new Collection<ExpectedCheck>
            {
                new ExpectedCheck(
                    id: Context.PosOperations.OrderBy(po => po.DateStarted).First().Id,
                    quantity: expectedQuantity,
                    totalPrice: expectedTotalPrice,
                    expectedTotalPriceWithDiscount: expectedPriceWithDiscount,
                    expectedCheckItemsCount: expectedItemsCount,
                    expectedDateTime: 25.March(2010), expectedBonus: 0M)
            });
        }
        
        [TestCase(CheckItemStatus.Refunded, CheckItemStatus.Unpaid, 1, 5, 4, 2)]
        [TestCase(CheckItemStatus.Deleted, CheckItemStatus.Unpaid, 1, 5, 4, 1)]
        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Deleted, 1, 5, 5, 1)]
        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Unpaid, 2, 10, 9, 2)]
        [TestCase(CheckItemStatus.Unpaid, CheckItemStatus.Paid, 2, 10, 9, 2)]
        [TestCase(CheckItemStatus.PaidUnverified, CheckItemStatus.Paid, 2, 10, 9, 2)]
        public void MakePurchasesHistoryOfUserAsync_DifferentCheckItemsInPosOperationWithBonusAreGivenAndOneOfWhichHaveDiscount_ShouldReturnEmptyPurchasesHistory(
            CheckItemStatus firstCheckItemStatus,
            CheckItemStatus secondCheckItemStatus,
            int expectedQuantity,
            decimal expectedTotalPrice, 
            decimal expectedPriceWithDiscount, 
            int expectedItemsCount)
        {
            var firstCheckItem = CreateCheckItem(DefaultPosId, firstCheckItemStatus, 1, 1, 5M);

            var secondCheckItem = CreateCheckItem(DefaultPosId, secondCheckItemStatus, 2, 1, 5M);
            secondCheckItem.AddDiscount(20M);

            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(new DateTime(2010,3,25))
                    .SetCheckItems(new Collection<CheckItem>
                    {
                        firstCheckItem,
                        secondCheckItem
                    })
                    .Build()
            });
            EnsurePurchasesHistoryHasExpectedResult(1, new Collection<ExpectedCheck>
            {
                new ExpectedCheck(
                    id: Context.PosOperations.OrderBy(po => po.DateStarted).First().Id,
                    quantity: expectedQuantity,
                    totalPrice: expectedTotalPrice,
                    expectedTotalPriceWithDiscount: expectedPriceWithDiscount,
                    expectedCheckItemsCount: expectedItemsCount,
                    expectedDateTime: 25.March(2010), expectedBonus: 0M)
            });
        }

        [Test]
        public void MakePurchasesHistoryOfUserAsync_CheckItemWithSeveralCorrectionsInPosOperationIsGivenAndHaveNoAuditRecordByLastCorrection_ShouldReturnCorrectPurchasesHistory()
        {
            Insert(new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                    .SetDateStarted(new DateTime(2010,3,25))
                    .SetCheckItems(new Collection<CheckItem>
                    {
                        CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, 1, 1, 5M)
                    })
                    .Build()
            });

            var checkItem = Context.CheckItems.FirstOrDefault();
            checkItem.MarkAsPaid(DefaultUserId);
            checkItem.MarkAsModifiedByAdmin();

            Context.SaveChanges();

            checkItem.SetProperty("Status", CheckItemStatus.Unpaid);

            Context.SaveChanges();

            EnsurePurchasesHistoryHasExpectedResult(1, new Collection<ExpectedCheck>
            {
                new ExpectedCheck(
                    id: Context.PosOperations.OrderBy(po => po.DateStarted).First().Id,
                    quantity: 1,
                    totalPrice: 5M,
                    expectedTotalPriceWithDiscount: 5M,
                    expectedCheckItemsCount: 1,
                    expectedDateTime: 25.March(2010), 
                    expectedBonus: 0M)
            });
        }

        private void EnsurePurchasesHistoryHasExpectedResult(int expectedCount, ICollection<ExpectedCheck> expectedChecks)
        {
            var purchasesHistoryResult = _purchasesHistoryMaker.MakeNonEmptyChecksForUserAsync(DefaultUserId).Result;
            purchasesHistoryResult.Checks.Should().NotBeNullOrEmpty();
            purchasesHistoryResult.Checks.Should().HaveCount(expectedCount);

            var checks = purchasesHistoryResult.Checks;
            checks.Should().BeInDescendingOrder(x => x.DateCreated);

            foreach (var check in checks)
            {
                var expectedCheck = expectedChecks.First(ec => ec.PosOperationId == check.Id);
                expectedCheck.Should().NotBeNull();
                check.Items.Should().HaveCount(expectedCheck.ExpectedCheckItemsCount);
                check.Summary.CostSummary.ItemsQuantity.Should().Be(expectedCheck.ExpectedQuantity);
                check.Summary.CostSummary.CostWithoutDiscount.Should().Be(expectedCheck.ExpectedTotalPrice);
                check.Summary.CostSummary.CostWithDiscount.Should().Be(expectedCheck.ExpectedTotalPriceWithDiscount);
                check.DateCreated.Should().Be(expectedCheck.ExpectedDateTime);
            }
        }

        private void EnsurePurchasesHistoryIsEmpty()
        {
            var purchasesHistoryResult = _purchasesHistoryMaker.MakeNonEmptyChecksForUserAsync(DefaultUserId).Result;
            purchasesHistoryResult.Checks.Should().BeNullOrEmpty();
        }

        private static CheckItem CreateCheckItem(int posId, CheckItemStatus status, int labeledGoodId, int posOperationId, decimal price)
        {
            return CheckItem.NewBuilder(
                    posId,
                    posOperationId,
                    1,
                    labeledGoodId,
                    1)
                .SetPrice(price)
                .SetStatus(status)
                .Build();
        }

        private void Insert<T>(IEnumerable<T> entities) where T : class
        {
            Seeder.Seed(entities);
        }
    }
}
