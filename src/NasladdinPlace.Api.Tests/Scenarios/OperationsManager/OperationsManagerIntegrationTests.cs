using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DataGenerators;
using NasladdinPlace.Api.Tests.DataGenerators.Models;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.Api.Tests.Scenarios.OperationsManager
{
    public class OperationsManagerIntegrationTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultTransactionId = 1;
        private const int DefaultPosId = 1;
        private const int DefaultGoodId = 1;
        private const int DefaultLabeledGoodId = 1;
        private const int DefaultCurrencyId = 1;
        private const int SecondPosOperation = 2;

        private IOperationsManager _operationManager;
        private IUnitOfWork _unitOfWork;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            Seeder.Seed(new PromotionSettingsDataSet());

            var serviceProvider = TestServiceProviderFactory.Create();
            _operationManager = serviceProvider.GetRequiredService<IOperationsManager>();

            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkFactory>().MakeUnitOfWork();
        }

        [Test]
        public void MarkPosOperationAsPaidAsync_ActiveOperationIsGiven_ShouldReturnExpectedResult(
            [ValueSource(typeof(OperationManagerTestsDataGenerator), "Data")]
            OperationManagerSource source)
        {
            Insert(source.PosOperations);

            var bankTransactionSummary =
                new BankTransactionSummary(Context.PaymentCards.First().Id, DefaultTransactionId, 3M);
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

            Context.SaveChanges();

            var posOperation = GetLatestCompletedUnpaidPosOperationHavingUnpaidCheckItemsOfUserAsync(
                operationPaymentInfo.UserId
            );
            var posOperationResult = _operationManager
                .MarkPosOperationAsPaidAsync(_unitOfWork, posOperation, operationPaymentInfo).Result;

            if (source.ExpectedStatus == PosOperationStatus.Paid)
            {
                posOperationResult.Succeeded.Should().BeTrue();
                posOperationResult.Value.Should().NotBeNull();
                AssertPosOperationInDatabaseMarkAsPaid(posOperationResult.Value.Id);
            }
            else
            {
                posOperationResult.Succeeded.Should().BeFalse();
            }

            AssertExpectedBonusAmountInDatabaseForDefaultUser(source.ExpectedBonus, source.ExpectedBonusesCount);
        }

        [Test]
        public void MarkPosOperationAsPaidAsync_ActiveOperationAndUserHaveFirstPayBonusAreGiven_ShouldReturnExpectedResult(
                [ValueSource(typeof(OperationManagerTestsDataGenerator), "Data")]
                OperationManagerSource source)
        {
            Insert(source.PosOperations);

            var user = Context.Users.SingleOrDefault(u => u.Id == DefaultUserId);
            user.AddBonusPoints(50M, BonusType.FirstPay);

            var bankTransactionSummary =
                new BankTransactionSummary(Context.PaymentCards.First().Id, DefaultTransactionId, 3M);
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

            Context.SaveChanges();

            var posOperation = GetLatestCompletedUnpaidPosOperationHavingUnpaidCheckItemsOfUserAsync(
                operationPaymentInfo.UserId
            );
            var posOperationResult = _operationManager
                .MarkPosOperationAsPaidAsync(_unitOfWork, posOperation, operationPaymentInfo).Result;

            if (source.ExpectedStatus == PosOperationStatus.Paid)
            {
                posOperationResult.Succeeded.Should().BeTrue();
                posOperationResult.Value.Should().NotBeNull();
                AssertPosOperationInDatabaseMarkAsPaid(posOperationResult.Value.Id);
            }
            else
            {
                posOperationResult.Succeeded.Should().BeFalse();
            }
            AssertExpectedBonusAmountInDatabaseForDefaultUser(50M, 1);
        }

        [Test]
        public void MarkPosOperationAsPaidAsync_PendingPaymentPosOperationHavingCheckItemsIsGiven_ShouldReturnSuccess()
        {
            var bankTransactionSummary =
                new BankTransactionSummary(Context.PaymentCards.First().Id, DefaultTransactionId, 3M);
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .SetCheckItems(
                    new List<CheckItem>
                    {
                        CheckItem.NewBuilder(
                                DefaultPosId,
                                SecondPosOperation,
                                DefaultGoodId,
                                DefaultLabeledGoodId,
                                DefaultCurrencyId)
                            .SetPrice(5M)
                            .SetStatus(CheckItemStatus.Unpaid)
                            .Build()
                    })
                .Build();

            Insert(new List<PosOperation>
            {
                posOperation
            });

            Context.SaveChanges();

            posOperation = GetLatestCompletedUnpaidPosOperationHavingUnpaidCheckItemsOfUserAsync(
                operationPaymentInfo.UserId
            );
            var posOperationResult = _operationManager
                .MarkPosOperationAsPaidAsync(_unitOfWork, posOperation, operationPaymentInfo).Result;

            posOperationResult.Should().NotBeNull();
            AssertPosOperationInDatabaseMarkAsPaid(posOperationResult.Value.Id);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(50M, 1);
        }

        private void AssertPosOperationInDatabaseMarkAsPaid(int operationId)
        {
            var posOperationResult = Context.PosOperations.AsNoTracking().First(pos => pos.Id == operationId);
            posOperationResult.Status.Should().Be(PosOperationStatus.Paid);
        }

        private void AssertExpectedBonusAmountInDatabaseForDefaultUser(decimal expectedBonus, int expectedBonusesCount)
        {
            var user = Context.Users
                .Include(u => u.BonusPoints)
                .AsNoTracking()
                .Single(u => u.Id == DefaultUserId);

            user.TotalBonusPoints.Should().Be(expectedBonus);
            user.BonusPoints.Count(ub => ub.Type == BonusType.FirstPay).Should().Be(expectedBonusesCount);
        }

        private void Insert<T>(IEnumerable<T> entities) where T : class
        {
            Seeder.Seed(entities);
        }

        private PosOperation GetLatestCompletedUnpaidPosOperationHavingUnpaidCheckItemsOfUserAsync(int userId)
        {
            return _unitOfWork.PosOperations.GetLatestCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(
                userId
            ).Result;
        }
    }
}