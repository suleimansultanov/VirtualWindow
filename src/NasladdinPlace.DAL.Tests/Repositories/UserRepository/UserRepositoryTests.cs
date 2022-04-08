using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Users.Search.Model;
using NasladdinPlace.DAL.Tests.Repositories.PosOperationRepository.DataGenerators;
using NasladdinPlace.DAL.Tests.Repositories.UserRepository.DataGenerators;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;

namespace NasladdinPlace.DAL.Tests.Repositories.UserRepository
{
    public class UserRepositoryTests : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int UserWithActivePaymentCardId = 1;
        private const int UserWithoutPaymentCardId = 2;
        private const int DefaultBankTransactionId = 1;

        private DAL.Repositories.UserRepository _userRepository;

        public override void SetUp()
        {
            base.SetUp();
            
            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new PaymentCardsDataSet(UserWithActivePaymentCardId));
            Seeder.Seed(new PaymentCardsDataSet(UserWithoutPaymentCardId));
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            TryAssignPaymentSystemToUser(
                UserWithActivePaymentCardId,
                GetPaymentCardIdByUserId(UserWithActivePaymentCardId)
            );

            _userRepository = new DAL.Repositories.UserRepository(Context);
        }

        [TestCase(CheckItemStatus.Unpaid, 1)]
        [TestCase(CheckItemStatus.Refunded, 0)]
        [TestCase(CheckItemStatus.Deleted, 0)]
        public void GetDebtorsAbleToPay_OperationInPastAndCheckItemsWithDifferentActionsAreGiven_ShouldReturnCorrectDebtorsNumber(CheckItemStatus action, int expected)
        {
            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserWithActivePaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddMinutes(-100))
                    .Build()
            };
            Insert(posOperations.ToArray());

            Insert(CheckItemsDataGenerator.CreateCheckItem(DefaultPosId, action, 1, 1));

            var debtors = _userRepository.GetDebtorsAbleToPayAsync(TimeSpan.FromMinutes(30)).Result;
            debtors.Should().HaveCount(expected);
        }

        [TestCase(CheckItemStatus.Unpaid)]
        [TestCase(CheckItemStatus.Refunded)]
        [TestCase(CheckItemStatus.Deleted)]
        public void GetDebtorsAbleToPay_RecentOperationIsGiven_ShouldNotReturnAnyDebtors(CheckItemStatus action)
        {
            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserWithActivePaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow)
                    .Build()
            };
            Insert(posOperations.ToArray());

            Insert(CheckItemsDataGenerator.CreateCheckItem(DefaultPosId, action, 1 , 1));

            var debtors = _userRepository.GetDebtorsAbleToPayAsync(TimeSpan.FromMinutes(30)).Result;
            debtors.Should().HaveCount(0);
        }

        [Test]
        public void GetDebtorsAbleToPay_PaidPosOperationIsGiven_ShouldNotReturnAnyDebtors()
        {
            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserWithActivePaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow)
                    .MarkAsPendingPayment()
                    .Build()
            };

            var bankTransactionSummary = new BankTransactionSummary(
                GetPaymentCardIdByUserId(UserWithActivePaymentCardId), DefaultBankTransactionId, 5M
            );
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(UserWithActivePaymentCardId, bankTransactionSummary, 5M);
            posOperations.FirstOrDefault()?.MarkAsPaid(operationPaymentInfo);
            
            Insert(posOperations.ToArray());

            Insert(CheckItemsDataGenerator.CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, 1, 1));

            var debtors = _userRepository.GetDebtorsAbleToPayAsync(TimeSpan.FromMinutes(30)).Result;
            debtors.Should().BeEmpty();
        }

        [TestCase(true, true, 2)]
        [TestCase(false, true, 1)]
        [TestCase(true, false, 1)]
        [TestCase(false, false, 0)]
        public void GetDebtorsAbleToPay_TwoDebtorsWithDifferentPaymentSystemsAreGiven_ShouldReturnCorrectDebtorsNumber(
            bool doesFirstUserHavePaymentSystem, bool doesSecondUserHavePaymentSystem, int expected)
        {
            TryAssignPaymentSystemToUser(UserWithActivePaymentCardId, 
                doesFirstUserHavePaymentSystem 
                    ? (int?) GetPaymentCardIdByUserId(UserWithActivePaymentCardId) 
                    : null
                );
            TryAssignPaymentSystemToUser(UserWithoutPaymentCardId, doesSecondUserHavePaymentSystem 
                ? (int?) GetPaymentCardIdByUserId(UserWithoutPaymentCardId) 
                : null
            );

            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserWithActivePaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserWithoutPaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build()
            };
            Insert(posOperations.ToArray());

            var checkItems = new Collection<CheckItem>
            {
                CheckItemsDataGenerator.CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, 1, 1),
                CheckItemsDataGenerator.CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, 2, 2)
            };
            Insert(checkItems.ToArray());

            var debtors = _userRepository.GetDebtorsAbleToPayAsync(TimeSpan.FromMinutes(30)).Result;
            debtors.Should().HaveCount(expected);
        }

        [Test]
        public void GetDebtorsAbleToPay_UsersPaymentSystemIsNotNullAndCheckItemsIsGiven_ShouldReturnAnyDebtors(
            [ValueSource(typeof(CheckItemsDataGenerator), "Data")] Collection<CheckItem> checkItems)
        {
            TryAssignPaymentSystemToUser(UserWithoutPaymentCardId, GetPaymentCardIdByUserId(UserWithoutPaymentCardId));

            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserWithActivePaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserWithoutPaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build()
            };
            Insert(posOperations.ToArray());

            Insert(checkItems.ToArray());

            var debtors = _userRepository.GetDebtorsAbleToPayAsync(TimeSpan.FromMinutes(30)).Result;
            debtors.Should().HaveCount(1);
        }

        [Test]
        public void GetDebtorsAbleToPay_SecondUserPaymentSystemIsNullAndPosOperationForFirstUserIsPaid_ShouldReturnEmpty()
        {
            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserWithActivePaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .MarkAsPendingPayment()
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserWithoutPaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build()
            };
            
            var bankTransactionSummary = new BankTransactionSummary(
                GetPaymentCardIdByUserId(UserWithActivePaymentCardId), DefaultBankTransactionId, 5M
            );
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(UserWithActivePaymentCardId, bankTransactionSummary, 5M);
            posOperations.FirstOrDefault()?.MarkAsPaid(operationPaymentInfo);

            Insert(posOperations.ToArray());

            var checkItems = new Collection<CheckItem>
            {
                CheckItemsDataGenerator.CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, 1, 1),
                CheckItemsDataGenerator.CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, 2, 2)
            };
            Insert(checkItems.ToArray());

            var debtors = _userRepository.GetDebtorsAbleToPayAsync(TimeSpan.FromMinutes(30)).Result;
            debtors.Should().BeEmpty();
        }

        [TestCase(100, 5, 1)]
        [TestCase(5, 100, 1)]
        [TestCase(100, 100, 2)]
        [TestCase(5, 5, 0)]
        public void
            GetDebtorsAbleToPay_PosOperationDifferenceDateTimeCompleteAndUsersPaymentSystemIsNull_ShouldReturnExpectedDebtorsNumber(
                int firstOperationStartDateNegativeDelayInMinutesFromNow,
                int secondOperationStartDateNegativeDelayInMinutesFromNow, 
                int expectedDebtorsNumber)
        {
            TryAssignPaymentSystemToUser(UserWithoutPaymentCardId, GetPaymentCardIdByUserId(UserWithoutPaymentCardId));

            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserWithActivePaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddMinutes(-firstOperationStartDateNegativeDelayInMinutesFromNow))
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserWithoutPaymentCardId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddMinutes(-secondOperationStartDateNegativeDelayInMinutesFromNow))
                    .Build()
            };
            Insert(posOperations.ToArray());

            var checkItems = new Collection<CheckItem>
            {
                CheckItemsDataGenerator.CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, 1, 1),
                CheckItemsDataGenerator.CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, 1, 2)
            };
            Insert(checkItems.ToArray());

            var debtors = _userRepository.GetDebtorsAbleToPayAsync(TimeSpan.FromMinutes(30)).Result;
            debtors.Should().HaveCount(expectedDebtorsNumber);
        }

        [Test, TestCaseSource(typeof(UsersFilterDataGenerator))]
        public void GetByFilter_FilterIsGiven_ShouldReturnExpectedUsersCountAndCorrectOrderedListOfUser(
            IEnumerable<ApplicationUser> inputUsers, Filter inputFilter, int expectedUsersCount)
        {
            var existingUsers = Context.Users.ToImmutableList();
            existingUsers.ForEach(u => u.ResetActivePaymentCard());
            Context.SaveChanges();
            
            var paymentCards = Context.PaymentCards.ToImmutableList();
            paymentCards.ForEach(pc => Context.PaymentCards.Remove(pc));
            Context.SaveChanges();

            existingUsers.ForEach(u => Context.Users.Remove(u));
            Context.SaveChanges();
            
            Insert(inputUsers.ToArray());
            var users = _userRepository.GetByFilterAsync(inputFilter).Result;
            users.Should().HaveCount(expectedUsersCount);
            users.Should().BeInAscendingOrder(u => u.Id);
        }

        [Test, TestCaseSource(typeof(SortByDataGenerator))]
        public void GetByFilter_SortByFieldNameIsGiven_ShouldReturnCorrectOrderedListOfUsers<T>(string sortBy, Expression<Func<ApplicationUser, T>> orderExpression)
        {
            Insert(new Collection<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "Test User",
                    PhoneNumberConfirmed = false
                }
            }.ToArray());
            Context.SaveChanges();

            var users = _userRepository.GetByFilterAsync(new Filter
            {
                RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                SortBy = sortBy
            }).Result;

            users.Should().BeInAscendingOrder(orderExpression);
        }

        [Test]
        public void GetByFilter_SortByDatePaidIsGiven_ShouldReturnCorrectOrderedListOfUsers()
        {
            var user = new ApplicationUser
            {
                UserName = "Test User",
                PhoneNumberConfirmed = false
            };

            var operation = PosOperation.NewOfUserAndPosBuilder(1, 1)
                .MarkAsPendingPayment()
                .Build();
            
            var bankTransactionSummary = new BankTransactionSummary(
                GetPaymentCardIdByUserId(UserWithActivePaymentCardId), DefaultBankTransactionId, 5M
            );
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(UserWithActivePaymentCardId, bankTransactionSummary, 5M);
            operation.MarkAsPaid(operationPaymentInfo);
            
            user.PosOperations.Add(operation);

            Insert(new Collection<ApplicationUser> {user}.ToArray());
            Context.SaveChanges();

            var users = _userRepository.GetByFilterAsync(new Filter
            {
                RegistrationInitiationDateRange = new OptionalDateTimeRange(),
                RegistrationCompletionDateRange = new OptionalDateTimeRange(),
                PaymentCardVerificationInitiationDateRange = new OptionalDateTimeRange(),
                PaymentCardVerificationCompletionDateRange = new OptionalDateTimeRange(),
                SortBy = "DatePaid"
            }).Result;

             var expectedOrderBy = Context.Users.OrderBy(u => u.PosOperations.Where(pos => pos.DatePaid != null)
                .OrderByDescending(pos => pos.DatePaid).FirstOrDefault());

            users.Should().BeEquivalentTo(expectedOrderBy);
        }

        [Test]
        public void AddBonus_UserIsGiven_ShouldReturnExpectedUsersBonus()
        {
            var user = Context.Users
                .Include(u => u.BonusPoints)
                .AsNoTracking()
                .FirstOrDefault();

            user.AddBonusPoints(15M, BonusType.Payment);

            Context.SaveChanges();

            user.TotalBonusPoints.Should().Be(15M);
            user.BonusPoints.Should().HaveCount(1);
        }

        [Test]
        public void SubtractBonus_UserIsGiven_ShouldReturnExpectedUsersBonus()
        {
            var user = Context.Users
                .Include(u => u.BonusPoints)
                .AsNoTracking()
                .FirstOrDefault();

            user.AddBonusPoints(15M, BonusType.Payment);
            user.SubtractBonusPoints(10M, BonusType.Refund);
            Context.SaveChanges();

            user.TotalBonusPoints.Should().Be(5M);
            user.BonusPoints.Should().HaveCount(2);
        }

        [Test]
        public void FindHavingBonusAmountExceedingAsync_UsersHavingAbnormalBonusesAreGiven_ShouldReturnExpectedCount()
        {
            var user = Context.Users
                .Include(u => u.BonusPoints)
                .FirstOrDefault();

            user.AddBonusPoints(255M, BonusType.Payment);

            Context.SaveChanges();

            var users = _userRepository.FindHavingBonusAmountExceedingAsync(250M).Result;

            users.Should().HaveCount(1);
            users.Should().Contain(u => u.Id == user.Id);
        }

        [Test]
        public void FindHavingBonusAmountExceedingAsync_UsersHavingBonusesWithinNormalBoundariesAreGiven_ShouldReturnEmptyCollection()
        {
            var user = Context.Users
                .Include(u => u.BonusPoints)
                .FirstOrDefault();

            user.AddBonusPoints(125M, BonusType.Payment);

            Context.SaveChanges();

            var users = _userRepository.FindHavingBonusAmountExceedingAsync(250M).Result;

            users.Should().BeEmpty();
        }

        private void TryAssignPaymentSystemToUser(int userId, int? paymentCardId)
        {
            var user = Context.Users
                .Include(u => u.PaymentCards)
                .Single(u => u.Id == userId);
            
            if (!paymentCardId.HasValue)
            {
                user.ResetActivePaymentCard();
            }
            else
            {
                user.SetActivePaymentCard(paymentCardId.Value);
            }

            Context.SaveChanges();
        }

        private void Insert<T>(params T[] entities) where T : class
        {
            Seeder.Seed(entities);
        }

        private int GetPaymentCardIdByUserId(int userId)
        {
            return Context.PaymentCards.First(pc => pc.UserId == userId)?.Id ?? 0;
        }
    }
}
