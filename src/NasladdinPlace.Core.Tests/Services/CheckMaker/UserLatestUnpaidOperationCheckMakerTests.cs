using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Check.Detailed.Mappers;
using NasladdinPlace.Core.Services.Check.Simple.Makers;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models;
using NasladdinPlace.Core.Services.Check.Simple.Mappers;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Tests.Services.CheckMaker.DataGenerator;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.CheckMaker
{
    public class UserLatestUnpaidOperationCheckMakerTests
    {
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 1;
        private const int DefaultPaymentCardId = 1;
        private const int DefaultBankTransactionId = 1;

        private static readonly ApplicationUser DefaultUser = new ApplicationUser
        {
            Id = DefaultUserId, 
            UserName = "support@nasladdin.com"
        };

        private static readonly Pos DefaultPos = Pos.Default;

        private readonly IUserLatestOperationCheckMaker _checkMaker;
        private readonly Mock<IPosOperationRepository> _mockPosOperationRepository;
        
        public UserLatestUnpaidOperationCheckMakerTests()
        {
            _mockPosOperationRepository = new Mock<IPosOperationRepository>();

            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.SetupGet(u => u.PosOperations).Returns(_mockPosOperationRepository.Object);
            
            var mockUoWFactory = new Mock<IUnitOfWorkFactory>();
            mockUoWFactory.Setup(f => f.MakeUnitOfWork()).Returns(mockUoW.Object);

            var simpleCheckMaker = new SimpleCheckMaker(
                new Core.Services.Check.Detailed.Makers.DetailedCheckMaker(
                    new DetailedCheckGoodInstanceCreator(),
                    string.Empty,
                    string.Empty
                ),
                new SimpleCheckMapper()
            );

            _checkMaker = new UserLatestOperationCheckMaker(
                mockUoWFactory.Object,
                simpleCheckMaker
            );
        }

        [Theory]
        [ClassData(typeof(CheckItemsDataGenerator))]
        public void MakeForUserAsync_PosOperationAndCheckItemsAreGiven_ShouldReturnExpectedCheck(
            Collection<CheckItem> checkItems, int expectedGoodsCount, decimal expectedTotalPrice, decimal expectedPriceWithDiscount,
            int expectedTotalQuantity, bool expectedIsZero)
        {
            var posOperation = CreatePendingPaymentPosOperation();
            posOperation.MarkAsCompletedAndRememberDate();

            foreach (var checkItem in checkItems)
            {
                posOperation.CheckItems.Add(checkItem);
            }

            _mockPosOperationRepository.Setup(r => r.GetLatestCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(DefaultUserId))
                .Returns(Task.FromResult(posOperation));

            EnsureSimpleCheckContainsExpectedValues(
                expectedGoodsCount,
                expectedTotalPrice,
                expectedPriceWithDiscount,
                expectedTotalQuantity,
                expectedIsZero
            );
        }

        [Fact]
        public void MakeForUserAsync_PosOperationIsNull_ShouldReturnEmptyCheck()
        {
            _mockPosOperationRepository.Setup(r => r.GetLatestUnpaidOfUserAsync(DefaultUserId))
                .Returns(Task.FromResult<PosOperation>(null));

            var checkResult = _checkMaker.MakeForUserIfOperationUnpaidAsync(DefaultPosId).GetAwaiter().GetResult();

            checkResult.Status.Should().Be(UserOperationCheckMakerStatus.PosOperationNotFound);
            checkResult.Check.Should().BeNull();
        }

        [Theory]
        [ClassData(typeof(CheckItemsDataGenerator))]
        public void MakeForUserAsync_PosOperationPaidAndCheckItemsAreGiven_ShouldReturnExpectedCheck(
            Collection<CheckItem> checkItems, 
            int expectedGoodsCount, 
            decimal expectedTotalPrice,
            decimal expectedPriceWithDiscount,
            int expectedTotalQuantity, 
            bool expectedIsZero)
        {
            var posOperation = CreatePendingPaymentPosOperation();

            foreach (var checkItem in checkItems)
            {
                posOperation.CheckItems.Add(checkItem);
            }
            
            var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultBankTransactionId, 5M);
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 5M);
            posOperation.MarkAsPaid(operationPaymentInfo);

            _mockPosOperationRepository.Setup(r => r.GetLatestCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(DefaultUserId))
                .Returns(Task.FromResult(posOperation));

            EnsureSimpleCheckContainsExpectedValues(
                expectedGoodsCount,
                expectedTotalPrice,
                expectedPriceWithDiscount,
                expectedTotalQuantity,
                expectedIsZero
            );
        }

        [Fact]
        private void MakeForUserAsync_PosOperationPaidAndCheckItemsWithDiscountAreGiven_ShouldReturnExpectedCheck()
        {
            var posOperation = CreatePendingPaymentPosOperation();
            var posId = DefaultPos.Id;
            var goodId = Good.Unknown.Id;
            var labeledGood = LabeledGood.OfPos(DefaultPosId, "Label");
            var currency = Currency.Ruble;

            var checkItemWithDiscount = CheckItem
                .NewBuilder(posId, posOperation.Id, goodId, labeledGood.Id, currency.Id)
                .SetPrice(5M)
                .SetStatus(CheckItemStatus.Unpaid)
                .SetCurrency(currency)
                .SetLabeledGood(labeledGood)
                .Build();
            
            checkItemWithDiscount.AddDiscount(20M);

            var checkItems = new List<CheckItem>
            {
                CheckItem
                    .NewBuilder(posId, posOperation.Id, goodId, labeledGood.Id, currency.Id)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .SetCurrency(currency)
                    .SetLabeledGood(labeledGood)
                    .Build(),
            
                checkItemWithDiscount
            };

            foreach (var checkItem in checkItems)
            {
                posOperation.CheckItems.Add(checkItem);
            }

            var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultBankTransactionId, 5M);
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 4M);
            posOperation.MarkAsPaid(operationPaymentInfo);

            _mockPosOperationRepository.Setup(r => r.GetLatestCompletedUnpaidHavingUnpaidCheckItemsOfUserAsync(DefaultUserId))
                .Returns(Task.FromResult(posOperation));

            EnsureSimpleCheckContainsExpectedValues(
                2,
                10M,
                9M,
                2,
                false
            );
        }

        private void EnsureSimpleCheckContainsExpectedValues(
            int expectedGoodsCount, 
            decimal expectedTotalPrice,
            decimal expectedPriceWithDiscount,
            int expectedTotalQuantity,
            bool expectedIsZero)
        {
            var simpleCheckResult = _checkMaker.MakeForUserIfOperationUnpaidAsync(DefaultUserId).GetAwaiter().GetResult();

            var simpleCheck = simpleCheckResult.Check;

            simpleCheck.Summary.CostSummary.IsFree.Should().Be(expectedIsZero);
            simpleCheck.Summary.CostSummary.CostWithoutDiscount.Should().Be(expectedTotalPrice);
            simpleCheck.Summary.CostSummary.CostWithDiscount.Should().Be(expectedPriceWithDiscount);
            simpleCheck.Summary.CostSummary.ItemsQuantity.Should().Be(expectedTotalQuantity);
            simpleCheck.Items.Should().HaveCount(expectedGoodsCount);
        }

        private static PosOperation CreatePendingPaymentPosOperation()
        {
            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetId(1)
                .SetUser(DefaultUser)
                .SetPos(DefaultPos)
                .MarkAsPendingPayment()
                .Build();
            return posOperation;
        }      
    }
}