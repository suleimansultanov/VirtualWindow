using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Check.Refund.Calculator;
using NasladdinPlace.Core.Services.Check.Refund.Calculator.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Models;
using NasladdinPlace.Core.Services.CheckOnline;
using NasladdinPlace.Core.Services.Payment.Printer;
using NasladdinPlace.Core.Services.Payment.Printer.Contracts;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Services.Check.Factory;
using NasladdinPlace.Core.Services.Check.Helpers;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Models;
using NasladdinPlace.Core.Services.Payment.Adder;
using NasladdinPlace.Core.Services.Payment.Adder.Contracts;
using NasladdinPlace.Core.Services.Payment.Card;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Helpers;
using NasladdinPlace.Core.Services.Purchase.Completion.Contracts;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.Logging;
using Xunit;
using Currency = NasladdinPlace.Core.Models.Currency;
using PaymentResult = NasladdinPlace.Payment.Models.PaymentResult;
using RefundRequest = NasladdinPlace.Payment.Models.RefundRequest;

namespace NasladdinPlace.Core.Tests.Services.CheckManager
{
    public class CheckManagerTests
    {
        private const int DefaultPosId = 1;
        private const int DefaultPaymentCardId = 1;
        private const int DefaultPosOperationId = 1;
        private const int DefaultUserId = 1;
        private const int DefaultTransactionId = 1;
        private const int IncorrectPosOperationId = 2;
        private const int DefaultCheckItemId = 0;
        private readonly Good _defaultGood = Good.Unknown;

        private const string DefaultCardToken = "Default card token";

        private PosOperation _posOperation;
        private ICheckManager _checkManager;
        private readonly Mock<IPaymentService> _mockPaymentService;
        
        public CheckManagerTests()
        {
            _mockPaymentService = new Mock<IPaymentService>();
        }

        [Fact]
        public void RefundCheckItemsAsync_IncorrectPosOperationIdIsGiven_ShouldReturnFailureCheckManagerResult()
        {
            SetupTestPosOperation();

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(
                    IncorrectPosOperationId,
                    DefaultUserId,
                    Enumerable.Empty<int>()
                );

            var result = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).Result;

            result.IsSuccessful.Should().BeFalse();
            result.Error.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void AddItemAsync_IncorrectPosOperationIdIsGiven_ShouldReturnFailureAsCheckManagerResult()
        {
            SetupTestPosOperation();

            var defaultCheckItem =
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, CheckItemStatus.Unpaid,
                    5M, true);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                IncorrectPosOperationId,
                defaultCheckItem.GoodId,
                defaultCheckItem.LabeledGoodId,
                defaultCheckItem.CurrencyId,
                DefaultUserId,
                defaultCheckItem.Price
            );

            var result = _checkManager.AddItemAsync(checkItemAdditionInfo).Result;

            result.IsSuccessful.Should().BeFalse();
            result.Error.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void AddItemAsync_PaymentFailed_ShouldReturnFailureAsCheckManagerResult()
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);
                posOperationBuider
                    .MarkAsPendingPayment()
                    .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId));
            });
            
            _mockPaymentService.Setup(p => p.MakeRecurrentPaymentAsync(It.IsAny<RecurrentPaymentRequest>()))
                .Returns(Task.FromResult(Response<PaymentResult>.Failure("TestError")));

            var defaultCheckItem =
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, CheckItemStatus.Unpaid,
                    5M, true);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                _posOperation.Id,
                defaultCheckItem.GoodId,
                defaultCheckItem.LabeledGoodId,
                defaultCheckItem.CurrencyId,
                DefaultUserId,
                defaultCheckItem.Price
            );
            var result = _checkManager.AddItemAsync(checkItemAdditionInfo).Result;

            result.IsSuccessful.Should().BeFalse();
            result.Error.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void RefundCheckItemsAsync_RefundFailed_ShouldReturnFailureAsCheckManagerResult()
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 3M);
                var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

                posOperationBuider
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });

            _mockPaymentService.Setup(p => p.MakeRefundAsync(It.IsAny<RefundRequest>()))
                .Returns(Task.FromResult(Response<OperationResult>.Failure("TestError")));

            var defaultCheckItem =
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, CheckItemStatus.Paid,
                    10M);

            _posOperation.CheckItems = new List<CheckItem> { defaultCheckItem };

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(DefaultPosOperationId,
                DefaultUserId, new List<int> {DefaultCheckItemId});

            var result = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).Result;

            result.IsSuccessful.Should().BeFalse();
            result.Error.Should().NotBeNullOrWhiteSpace();
        }
        
        [Fact]
        public void AddItemAsync_PosOperationStatusIsNotPaid_ShouldReturnSuccessfulCheckManagerResultAndCheckItemsWithDefaultItem()
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                posOperationBuider
                    .SetCheckItems(new List<CheckItem>())
                    .MarkAsCompleted();
            });

            var defaultCheckItem =
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, CheckItemStatus.Unpaid,
                    10M, true);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                _posOperation.Id,
                defaultCheckItem.GoodId,
                defaultCheckItem.LabeledGoodId,
                defaultCheckItem.CurrencyId,
                DefaultUserId,
                defaultCheckItem.Price
            );
            var result = _checkManager.AddItemAsync(checkItemAdditionInfo).Result;

            result.IsSuccessful.Should().BeTrue();
            _posOperation.CheckItems.Should().Contain(defaultCheckItem);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void RefundCheckItemsAsync_PosOperationStatusNotPaidIsCreatedByAdminIsTaken_ShouldReturnSuccessfulCheckManagerResultAndCheckItemsWithDefaultItem(bool expectedIsModifiedByAdmin)
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                var defaultCheckItem =
                    CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, CheckItemStatus.Unpaid,
                        10M, expectedIsModifiedByAdmin);

                posOperationBuider.SetCheckItems(new List<CheckItem> { defaultCheckItem });
            });

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(DefaultPosOperationId,
                DefaultUserId, new List<int> { DefaultCheckItemId });

            var result =
                _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).Result;

            var checkItemAfterTest = _posOperation.CheckItems.FirstOrDefault();

            result.IsSuccessful.Should().BeTrue();
            checkItemAfterTest.Status.Should().Be(CheckItemStatus.Deleted);
            checkItemAfterTest.LabeledGood.PosOperationId.Should().BeNull();
            checkItemAfterTest.IsModifiedByAdmin.Should().Be(true);
        }

        [Fact]
        public void RefundCheckItemsAsync_CorrectInputData_ShouldReturnSuccessfulCheckManagerResult()
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                var defaultCheckItem =
                    CreateCheckItemFromPosOperationIdAndLabeledGoodId(
                        DefaultPosId, _defaultGood, CheckItemStatus.Paid, 10M);

                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 3M);
                var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);
                posOperationBuider
                    .SetCheckItems(new List<CheckItem> { defaultCheckItem })
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });
            
            _mockPaymentService.Setup(p => p.MakeRefundAsync(It.IsAny<RefundRequest>()))
                .Returns(Task.FromResult(Response<OperationResult>.Success(OperationResult.Success())));

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(DefaultPosOperationId,
                DefaultUserId, new List<int> { DefaultCheckItemId });

            var result =
                _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).Result;

            var checkItemAfterTest = _posOperation.CheckItems.FirstOrDefault();
            
            result.IsSuccessful.Should().BeTrue();
            _posOperation.BankTransactionInfos.Should().Contain(item => item.Type == BankTransactionInfoType.Refund);
            checkItemAfterTest.Status.Should().Be(CheckItemStatus.Refunded);
            checkItemAfterTest.LabeledGood.PosOperationId.Should().BeNull();
        }

        [Fact]
        public void AddItemAsync_CorrectInputData_ShouldReturnSuccessfulCheckManagerResult()
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 3M);
                var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

                posOperationBuider
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });

            _mockPaymentService.Setup(p => p.MakeRecurrentPaymentAsync(It.IsAny<RecurrentPaymentRequest>()))
                .Returns(Task.FromResult(
                    Response<PaymentResult>.Success(PaymentResult.Paid(DefaultTransactionId, null))));

            var defaultCheckItem =
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, CheckItemStatus.Unpaid,
                    10M, true);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                _posOperation.Id,
                defaultCheckItem.GoodId,
                defaultCheckItem.LabeledGoodId,
                defaultCheckItem.CurrencyId,
                DefaultUserId,
                defaultCheckItem.Price
            );
            var result = _checkManager.AddItemAsync(checkItemAdditionInfo).Result;
            result.IsSuccessful.Should().BeTrue();
            _posOperation.BankTransactionInfos.Count.Should().Be(2);
            _posOperation.CheckItems.Should().Contain(defaultCheckItem);
        }

        [Fact]
        public void RefundCheckItemsAsync_RefundedCheckItemIsGiven_ShouldReturnSuccessfulCorrectCheckManagerResult()
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                var defaultCheckItem =
                    CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, CheckItemStatus.Refunded,
                        10M);

                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 3M);
                var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

                posOperationBuider
                    .SetCheckItems(new List<CheckItem> { defaultCheckItem })
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });
            
            _mockPaymentService.Setup(p => p.MakeRefundAsync(It.IsAny<RefundRequest>()))
                .Returns(Task.FromResult(Response<OperationResult>.Success(OperationResult.Success())));

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(DefaultPosOperationId,
                DefaultUserId, new List<int> { DefaultCheckItemId });

            var result =
                _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).Result;

            var checkItemAfterTest = _posOperation.CheckItems.FirstOrDefault();

            result.IsSuccessful.Should().BeTrue();
            checkItemAfterTest.Status.Should().Be(CheckItemStatus.Refunded);
            checkItemAfterTest.LabeledGood.PosOperationId.Should().BeNull();
        }

        [Fact]
        public void MarkCheckItemAsVerifiedAsync_UnverifiedCheckItemAndPaidOperationAreGiven_ShouldPayForCheckItemAndReturnSuccessfulResult()
        {
            var unverifiedCheckItem =
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(
                    DefaultPosId,
                    _defaultGood,
                    CheckItemStatus.Unverified,
                    10M
                );

            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 0M);
                var operationPaymentInfo = OperationPaymentInfo.ForPaymentViaMoney(DefaultUserId, bankTransactionSummary);

                posOperationBuider
                    .SetCheckItems(new List<CheckItem> { unverifiedCheckItem })
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });

            _mockPaymentService.Setup(p => p.MakeRecurrentPaymentAsync(It.IsAny<RecurrentPaymentRequest>()))
                .Returns(Task.FromResult(
                    Response<PaymentResult>.Success(PaymentResult.Paid(DefaultTransactionId, null)))
                );

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(DefaultPosOperationId,
                DefaultUserId, new List<int> { unverifiedCheckItem.Id });

            var result = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemDeletionOrConfirmationInfo).Result;

            result.IsSuccessful.Should().BeTrue();

            _posOperation.CheckItems.Count.Should().Be(1);
            _posOperation.CheckItems.First().Status.Should().Be(CheckItemStatus.Paid);
        }

        [Theory]
        [InlineData(3, 3, 1)]
        [InlineData(3, 2, 2)]
        [InlineData(2, 2, 2)]
        [InlineData(2, 4, 2)]
        [InlineData(3, 4, 1)]
        public void
            RefundCheckItemsAsync_RefundedCheckItemsAreGiven_ShouldReturnSuccessfulCorrectCheckManagerResultAndExpectedRefundCountItems(
                int firstCheckItemStatus, int secondCheckItemStatus, int expectedBankTransactionCount)
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 5M);
                var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 15M);

                posOperationBuider
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });
            
            _mockPaymentService.Setup(p => p.MakeRefundAsync(It.IsAny<RefundRequest>()))
                .Returns(Task.FromResult(Response<OperationResult>.Success(OperationResult.Success())));

            _posOperation.CheckItems = new List<CheckItem>
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, (CheckItemStatus) firstCheckItemStatus, 10M),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, (CheckItemStatus) secondCheckItemStatus, 10M)
            };

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(DefaultPosOperationId,
                DefaultUserId, new List<int> { DefaultCheckItemId });

            var result =
                _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).Result;

            result.IsSuccessful.Should().BeTrue();
            _posOperation.BankTransactionInfos.Count.Should().Be(expectedBankTransactionCount);
            _posOperation.CheckItems.Where(item => 
                item.Status == CheckItemStatus.Deleted || item.Status == CheckItemStatus.Refunded).Should().HaveCount(2);
        }

        [Fact]
        public void RefundCheckItemsAsync_RefundedCheckItemWithDiscountIsGiven_ShouldReturnSuccessfulCorrectCheckManagerResult()
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                var defaultCheckItem = CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, CheckItemStatus.Refunded, price: 25M);
                defaultCheckItem.AddDiscount(discountInPercentage: 10M);

                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 30M);
                var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 5M);

                posOperationBuider
                    .SetCheckItems(new List<CheckItem> { defaultCheckItem })
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });

            _mockPaymentService.Setup(p => p.MakeRefundAsync(It.IsAny<RefundRequest>())).Returns(Task.FromResult(Response<OperationResult>.Success(OperationResult.Success())));

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(DefaultPosOperationId,
                DefaultUserId, new List<int> { DefaultCheckItemId });

            var result =
                _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).Result;

            var checkItemAfterTest = _posOperation.CheckItems.FirstOrDefault();

            result.IsSuccessful.Should().BeTrue();
            checkItemAfterTest.Status.Should().Be(CheckItemStatus.Refunded);
            checkItemAfterTest.LabeledGood.PosOperationId.Should().BeNull();
        }

        [Fact]
        public void CheckRefundCalculator_RefundedCheckItemWithDiscountIsGiven_ShouldReturnValidRefundCalculationResult()
        {
            SetupTestPosOperation(posOperationBuider =>
            {
                InitializePosOperationWithUserHavingActivePaymentCard(posOperationBuider);

                var defaultCheckItem = CreateCheckItemFromPosOperationIdAndLabeledGoodId(DefaultPosId, _defaultGood, CheckItemStatus.Refunded, price: 25M);
                defaultCheckItem.AddDiscount(discountInPercentage: 10M);

                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 30M);
                var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 5M);

                posOperationBuider
                    .SetCheckItems(new List<CheckItem> { defaultCheckItem })
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });

            var checkRefundCalculator = new CheckRefundCalculator();
            var calculatorResult = checkRefundCalculator.Calculate(_posOperation, _posOperation.CheckItems);
            calculatorResult.MoneyAmount.Should().Be(22);
        }

        private static CheckItem CreateCheckItemFromPosOperationIdAndLabeledGoodId(int defaultPosId, Good defaultGood,
            CheckItemStatus status, decimal price, bool isModifiedByAdmin = false)
        {
            var currency = Currency.Ruble;
            var posId = Pos.Default.Id;
            var posOperationId = PosOperation.OfUserAndPos(DefaultUserId, defaultPosId).Id;
            var labeledGood = LabeledGood.NewOfPosBuilder(defaultPosId, "DefaultLabel")
                .TieToGood(defaultGood.Id, new LabeledGoodPrice(price, 1), new ExpirationPeriod())
                .Build();

            var checkItemBuilder = CheckItem.NewBuilder(posId, posOperationId, defaultGood.Id, labeledGood.Id, currency.Id)
                .SetPrice(price)
                .SetStatus(status)
                .SetCurrency(currency)
                .SetLabeledGood(labeledGood);

            if (isModifiedByAdmin)
                checkItemBuilder.MarkAsModifiedByAdmin();

            return checkItemBuilder.Build();
        }

        private static void InitializePosOperationWithUserHavingActivePaymentCard(
            PosOperationOfUserAndPosBuilder posOperationOfUserAndPosBuilder)
        {
            var user = new ApplicationUser();
            var paymentCardNumber = new PaymentCardNumber("123456", "7890");
            var extendedPaymentCardInfo = 
                new ExtendedPaymentCardInfo(paymentCardNumber, DateTime.UtcNow, DefaultCardToken);
            user.SetActivePaymentCard(extendedPaymentCardInfo);

            posOperationOfUserAndPosBuilder.SetUser(user);
        }

        private void SetupTestPosOperation(Action<PosOperationOfUserAndPosBuilder> additionalInitializationFunc = null)
        {
            var mockUoW = new Mock<IUnitOfWork>();
            var mockPosOperationRepository = new Mock<IPosOperationRepository>();
            var mockCheckOnlineManager = new Mock<ICheckOnlineManager>();
            var mockPurchaseCompletionManager = new Mock<IPurchaseCompletionManager>();
            var mockLogger = new Mock<ILogger>();

            var posAddressCoordinates = new Location(10, 11);
            var posAddress = Address.FromCityStreetAtCoordinates(1, "Тыныстанова 14", posAddressCoordinates);
            var operationOfUserAndPosBuilder = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetId(DefaultPosOperationId)
                .SetPos(new Pos(DefaultPosId, "Витрина 2", "Test2", posAddress)
                {
                    QrCode = "9C9FA2DD-1CCC-4885-8D3D-86F19123261B"
                });

            additionalInitializationFunc?.Invoke(operationOfUserAndPosBuilder);

            _posOperation = operationOfUserAndPosBuilder.Build();

            mockUoW.SetupGet(u => u.PosOperations).Returns(mockPosOperationRepository.Object);
            mockPosOperationRepository
                .Setup(r => r.GetIncludingCheckItemsAsync(It.Is<int>(op => op == DefaultPosOperationId)))
                .Returns(Task.FromResult(_posOperation));
            mockPurchaseCompletionManager
                .Setup(r => r.CompletePurchaseByPosOperationIdAsync(
                    It.Is<int>(op => op == DefaultPosOperationId),
                    It.Is<int>(us => us == DefaultUserId),
                    It.Is<PosOperationTransactionType>(pot => pot == PosOperationTransactionType.RegularPurchase || pot == PosOperationTransactionType.Addition)))
                .Returns(Task.FromResult(PurchaseCompletionResult.Success(new ApplicationUser(), _posOperation, SimpleCheck.Empty)));


            var mockUoWFactory = new Mock<IUnitOfWorkFactory>();
            mockUoWFactory.Setup(f => f.MakeUnitOfWork()).Returns(mockUoW.Object);

            var diServices = new ServiceCollection();

            diServices.AddTransient(sp => _mockPaymentService.Object);
            diServices.AddTransient(sp => mockCheckOnlineManager.Object);
            diServices.AddTransient<ICheckRefundCalculator,CheckRefundCalculator>();
            diServices.AddTransient(sp => mockUoWFactory.Object);
            diServices.AddTransient<IPaymentDescriptionPrinter, PaymentRussianDescriptionPrinter>();
            diServices.AddSingleton(sp => mockPurchaseCompletionManager.Object);
            diServices.AddTransient<IPosOperationTransactionCheckItemsMaker, PosOperationTransactionCheckItemsMaker>();
            diServices.AddTransient<IPosOperationTransactionCreationUpdatingService, PosOperationTransactionCreationUpdatingService>();
            diServices.AddTransient<IOperationTransactionManager, OperationTransactionManager>();
            diServices.AddTransient<IFirstPayBonusAdder, FirstPayBonusAdder>();
            diServices.AddSingleton<ICheckManagerBonusPointsHelper, CheckManagerBonusPointsHelper>();
            diServices.AddTransient(sp => mockLogger.Object);

            var serviceProvider = diServices.BuildServiceProvider();

            _checkManager = CheckManagerFactory.Create(serviceProvider);
        }
    }
}
