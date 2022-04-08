using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Services.Check.Factory;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Calculator.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Models;
using NasladdinPlace.Core.Services.CheckOnline;
using NasladdinPlace.Core.Services.Payment.Printer.Contracts;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models;
using NasladdinPlace.Core.Services.Purchase.Completion.Contracts;
using NasladdinPlace.Logging;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Currency = NasladdinPlace.Payment.Models.Currency;

namespace NasladdinPlace.Api.Tests.Scenarios.CheckManager
{
    public class CheckManagerIntegrationTests : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int FirstPosOperationId = 1;
        private const int SecondPosOperationId = 2;
        private const int FirstCheckItemId = 1;
        private const int DefaultUserId = 1;
        private const int DefaultCurrencyId = 1;
        private const int DefaultPaymentTransactionId = 555;
        private const int DefaultThreadsCount = 4;

        private const string FirstLabeledGood = "E2 80 11 60 60 00 02 05 2A 98 4B A1";
        private const int IncorrectPosOperationId = 999;

        private const string DefaultCryptogram = "015200828210221102gCBD/gnsVGyBszpkn89/LpE0WUOGEpALM3143Fn2Ud4htLRlDJiig/UOImIO3mPMbh6wk/sN/DwwKrEXMBDllkznC9SpcGLqCB8zjyU65Q6e2bH7S65Qb3h3snqJwitLmQkWqL8Dy9XGQQEGiNaNzomtjeilwjb+9QQXuPQzFHiAwL7SGRg9ZaV3+7bnXtsnx2sQFsRJDAC4RCBZ0JLKV9No/uCllKgQc8j9gP9q4kUX1lOBhkBU6jCXdb+b3CqoMmD1Y9AUDIGx+ICcdnaOT+Oif3pYZZmuMHllHIwBeBQqod87I8SN9xvB2WJOJP/Rmt4FwDw2oJokbQl6Trbk7g==";

        private const string CardHolder = "TEST USER";

        private const string UserIpAddress = "192.168.1.122";
        private const string UserIdentifier = "user_1";
        private const string PaymentTestDescription = "test";
        private const string DefaultErrorMessage = "error";

        private ICheckManager _checkManager;
        private IPaymentService _paymentService;
        private Mock<IPaymentService> _mockPaymentService;
        private IServiceProvider _serviceProvider;
        private AutoResetEvent _waitHandler;
        private IOperationTransactionManager _operationTransactionManager;
        private IPosOperationTransactionCreationUpdatingService _transactionCreationUpdatingService;
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
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(posId: DefaultPosId, userId: DefaultUserId));
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));
            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            _serviceProvider = TestServiceProviderFactory.Create();

            _mockPaymentService = new Mock<IPaymentService>();

            _paymentService = _serviceProvider.GetRequiredService<IPaymentService>();

            _checkManager = _serviceProvider.GetRequiredService<ICheckManager>();

            var checkManagerSmsSender = _serviceProvider.GetRequiredService<ICheckManagerSmsSender>();

            _waitHandler = new AutoResetEvent(false);

            _checkManager.CheckItemsEditingCompleted += async (sender, checkEditingInfo) =>
            {
                await checkManagerSmsSender.SendSmsAsync(checkEditingInfo);
                _waitHandler.Set();
            };

            _operationTransactionManager = _serviceProvider.GetRequiredService<IOperationTransactionManager>();

            _transactionCreationUpdatingService = _serviceProvider.GetRequiredService<IPosOperationTransactionCreationUpdatingService>();

            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWorkFactory>().MakeUnitOfWork();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void AppendItem_IncorrectPosOperationIdIsGiven_ShouldReturnFailureAsCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                IncorrectPosOperationId,
                defaultLabeledGood.GoodId.Value,
                defaultLabeledGood.Id,
                DefaultCurrencyId,
                DefaultUserId,
                price: 10M
            );
            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeFalse();
            appendItemResult.Error.Should().NotBeNullOrWhiteSpace();
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 0);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 0);
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 0);
            EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void AppendItem_CorrectPaidPosOperationIdIsGivenAndActivePaymentSystemIsNull_ShouldReturnFailureAsCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var operationPaymentInfo = OperationPaymentInfo.ForPaymentViaBonuses(DefaultUserId, bonusAmount: 10M);
            var posOperation = Context.PosOperations.First(po => po.Id == FirstPosOperationId);
            posOperation.MarkAsPendingCompletion();
            posOperation.MarkAsPendingCheckCreation();
            posOperation.MarkAsCompleted();
            posOperation.MarkAsPendingPayment();
            posOperation.MarkAsPaid(operationPaymentInfo);
            Context.SaveChanges();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                FirstPosOperationId,
                defaultLabeledGood.GoodId.Value,
                defaultLabeledGood.Id,
                DefaultCurrencyId,
                DefaultUserId,
                price: 10M
            );

            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeFalse();
            appendItemResult.Error.Should().NotBeNullOrWhiteSpace();
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 0);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 0);
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 0);
            EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            AppendItem_CorrectOpenedPosOperationIdIsGivenAndActivePaymentSystemIsNotNullAndOperationStatusIsNotPaidAndCheckItemIsAdded_ShouldReturnSuccessfulCheckManagerResultAndCheckItemsWithDefaultItem(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var posOperation = MarkFirstPosOperationAsCompletedAndRememberDate();

            CreateTransactionForPosOperation(posOperation);

            Context.SaveChanges();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                posOperationId: FirstPosOperationId,
                goodId: defaultLabeledGood.GoodId.Value,
                labeledGoodId: defaultLabeledGood.PosId.Value,
                currencyId: DefaultCurrencyId,
                editorId: DefaultUserId,
                price: 10M
            );

            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 1);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 1);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonus: 0, expectedBonusesCount: 0);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 1, expectedPaymentAmount: 10M, bankTransactionInfoType: BankTransactionInfoType.Payment);
            AssertExpectedPosOperationBonusInDatabase(0, FirstPosOperationId);
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 1);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 10M,
                fiscalizationType: FiscalizationType.Income);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 10M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(1, 1, 9, 1, 1, BankTransactionInfoType.Payment, false)]
        [TestCase(5, 1, 5, 1, 1, BankTransactionInfoType.Payment, false)]
        [TestCase(10, 0, 0, 0, 0, BankTransactionInfoType.Payment, false)]
        [TestCase(1, 1, 9, 1, 1, BankTransactionInfoType.Payment, true)]
        [TestCase(5, 1, 5, 1, 1, BankTransactionInfoType.Payment, true)]
        [TestCase(10, 0, 0, 0, 0, BankTransactionInfoType.Payment, true)]
        public void
            AppendItem_CorrectPaidPosOperationIdIsGivenAndActivePaymentSystemIsNotNullAndOperationStatusIsPaid_ShouldReturnSuccessfulCheckManagerResultAndBankTransactionsEqualExpectedResult(
                decimal amountBonuses,
                int expectedCountBankTransaction,
                decimal expectedPaymentAmount,
                int expectedFiscalizationCount,
                int expectedUserNotificationCount,
                BankTransactionInfoType bankTransactionInfoType,
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardMarkOperationAsPaidAddUserBonusPoints(amountBonuses);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                posOperationId: SecondPosOperationId,
                goodId: defaultLabeledGood.GoodId.Value,
                labeledGoodId: defaultLabeledGood.PosId.Value,
                currencyId: DefaultCurrencyId,
                editorId: DefaultUserId,
                price: 10M
            );

            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonus: 0, expectedBonusesCount: 2);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCountBankTransaction, expectedPaymentAmount, bankTransactionInfoType);
            AssertExpectedPosOperationBonusInDatabase(amountBonuses, SecondPosOperationId);
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 1);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount, NotificationArea.AdditionOrVerification);

            if (expectedFiscalizationCount > 0)
            {
                AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.Correction);
                AssertExpectedFiscalizationCorrectionSum(-expectedPaymentAmount, SecondPosOperationId);
            }

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: expectedCountBankTransaction,
                expectedBankTransactionsAmount: expectedPaymentAmount,
                expectedFiscalizationCount: expectedFiscalizationCount,
                expectedFiscalizationAmount: -expectedPaymentAmount,
                fiscalizationType: FiscalizationType.Correction);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: amountBonuses,
                expectedMoneyAmount: expectedPaymentAmount,
                expectedFiscalizationAmount: expectedPaymentAmount,
                expectedTotalCost: expectedPaymentAmount + amountBonuses,
                expectedTotalDiscount: 0M);
        }

        [TestCase(1, 1, null, BankTransactionInfoType.Payment, false)]
        [TestCase(1, 1, null, BankTransactionInfoType.Payment, true)]
        public void
            AppendItem_CorrectPosOperationIdIsGivenAndActivePaymentSystemIsNotNullAndOperationStatusIsPaidAndPaymentServiceReturnsErrorPaymentStatus_ShouldReturnFailureAsCheckManagerResultAndBankTransactionsEqualExpectedResult(
                decimal amountBonuses,
                int expectedCountBankTransaction,
                decimal? expectedPaymentAmount,
                BankTransactionInfoType bankTransactionInfoType,
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardMarkOperationAsPaidAddUserBonusPoints(amountBonuses);

            _mockPaymentService.Setup(p => p.MakeRecurrentPaymentAsync(It.IsAny<RecurrentPaymentRequest>()))
                .Returns(
                        Task
                        .FromResult(Response<PaymentResult>
                        .Success(PaymentResult.NotPaid(DefaultPaymentTransactionId, DefaultErrorMessage, DefaultErrorMessage))));

            var diServices = new ServiceCollection();

            diServices.AddTransient(sp => _mockPaymentService.Object);
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<ICheckOnlineManager>());
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<IPurchaseCompletionManager>());
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<ICheckRefundCalculator>());
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<IUnitOfWorkFactory>());
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<IPaymentDescriptionPrinter>());
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<ICheckManagerBonusPointsHelper>());
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<IPosOperationTransactionCreationUpdatingService>());
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<IOperationTransactionManager>());
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<ILogger>());

            var serviceProvider = diServices.BuildServiceProvider();

            var checkManager = CheckManagerFactory.Create(serviceProvider);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                posOperationId: SecondPosOperationId,
                goodId: defaultLabeledGood.GoodId.Value,
                labeledGoodId: defaultLabeledGood.PosId.Value,
                currencyId: DefaultCurrencyId,
                editorId: DefaultUserId,
                price: 10M
            );

            var appendItemResult = checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeFalse();
            appendItemResult.Error.Should().BeNullOrWhiteSpace();
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCountBankTransaction, expectedPaymentAmount, bankTransactionInfoType);
            AssertExpectedCheckItemAuditRecordInDatabase(0);
            AssertExpectedFiscalizationCountInDatabase(0);
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 0);
            EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists();
        }

        [TestCase(1, 1, 9, 9, 2, 1, BankTransactionInfoType.Payment, false)]
        [TestCase(5, 1, 5, 5, 2, 1, BankTransactionInfoType.Payment, false)]
        [TestCase(10, 0, 0, 0, 1, 0, BankTransactionInfoType.Payment, false)]
        [TestCase(1, 1, 9, 9, 2, 1, BankTransactionInfoType.Payment, true)]
        [TestCase(5, 1, 5, 5, 2, 1, BankTransactionInfoType.Payment, true)]
        [TestCase(10, 0, 0, 0, 1, 0, BankTransactionInfoType.Payment, true)]
        public void
            AppendItem_CorrectPaidPosOperationIdIsGivenAndActivePaymentSystemIsNotNullAndOperationStatusIsPaidAndOneCheckItemIsPaid_ShouldReturnSuccessfulCheckManagerResultAndBankTransactionsEqualExpectedResult(
                decimal amountBonuses,
                int expectedCountBankTransaction,
                decimal expectedPaymentAmount,
                decimal expectedFiscalizationCorrectionAmount,
                int expectedFiscalizationCount,
                int expectedUserNotificationCount,
                BankTransactionInfoType bankTransactionInfoType,
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardMarkOperationAsPaidAddUserBonusPoints(amountBonuses);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            Seeder.Seed(new List<CheckItem>
            {
               CheckItem.NewBuilder(
                       DefaultPosId,
                       SecondPosOperationId,
                       defaultLabeledGood.GoodId.Value,
                       defaultLabeledGood.Id,
                       DefaultCurrencyId)
                .SetPrice(15M)
                .SetStatus(CheckItemStatus.Paid)
                .Build()
            });

            var posOperation = Context.PosOperations.FirstOrDefault(p => p.Id == SecondPosOperationId);
            var fiscalizationInfo = new FiscalizationInfo(posOperation);

            Seeder.Seed(new List<FiscalizationInfo>
            {
                fiscalizationInfo
            });

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                posOperationId: SecondPosOperationId,
                goodId: defaultLabeledGood.GoodId.Value,
                labeledGoodId: defaultLabeledGood.Id,
                currencyId: DefaultCurrencyId,
                editorId: DefaultUserId,
                price: 10M
            );

            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeTrue();
            AssertExpectedBonusAmountInDatabaseForDefaultUser(0, 2);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCountBankTransaction,
                expectedPaymentAmount, bankTransactionInfoType);
            AssertExpectedPosOperationBonusInDatabase(amountBonuses, SecondPosOperationId);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount);
            AssertExpectedFiscalizationCorrectionAmount(-expectedFiscalizationCorrectionAmount, SecondPosOperationId);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount, NotificationArea.AdditionOrVerification);

            if (expectedFiscalizationCount > 1)
                AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.Correction);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: expectedCountBankTransaction,
                expectedBankTransactionsAmount: expectedPaymentAmount,
                expectedFiscalizationCount: expectedFiscalizationCount - 1,
                expectedFiscalizationAmount: -expectedPaymentAmount,
                fiscalizationType: FiscalizationType.Correction);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: amountBonuses,
                expectedMoneyAmount: expectedPaymentAmount,
                expectedFiscalizationAmount: expectedPaymentAmount,
                expectedTotalCost: expectedPaymentAmount + amountBonuses,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void AppendItem_CorrectPosOperationIdIsGivenAndActivePaymentSystemIsNotNullAndOperationStatusIsCompleted_ShouldReturnSuccessfulCheckManagerResultAndCompletePurchase(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var posOperation = CreateCompletedPosOperation(addInitialCheckItem: true);

            CreateTransactionForPosOperation(posOperation);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                posOperationId: SecondPosOperationId,
                goodId: defaultLabeledGood.GoodId.Value,
                labeledGoodId: defaultLabeledGood.PosId.Value,
                currencyId: DefaultCurrencyId,
                editorId: DefaultUserId,
                price: 10M
            );

            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 1);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: useNewPaymentSystem ? 2 : 1);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonus: 0, expectedBonusesCount: 0);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(
                expectedCount: useNewPaymentSystem ? 2 : 1,
                expectedPaymentAmount: 20M,
                bankTransactionInfoType: BankTransactionInfoType.Payment,
                expectedFirstBankTransactionAmount: useNewPaymentSystem ? 10M : 20M);
            AssertExpectedPosOperationBonusInDatabase(0, FirstPosOperationId);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: useNewPaymentSystem ? 2 : 1,
                expectedBankTransactionsAmount: 20M,
                expectedFiscalizationCount: useNewPaymentSystem ? 2 : 1,
                expectedFiscalizationAmount: 20M,
                fiscalizationType: FiscalizationType.Income);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 20M,
                expectedFiscalizationAmount: 20M,
                expectedTotalCost: 20M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            AppendItem_CorrectCompletedPosOperationsAreGivenAndActivePaymentSystemIsNotNullAndCheckItemIsAddedToNotLastPosOperation_ShouldReturnSuccessfulCheckManagerResultForNotLastOperation(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var posOperation = MarkFirstPosOperationAsCompletedAndRememberDate();

            CreateTransactionForPosOperation(posOperation);

            var secondPosOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsCompleted()
                .Build();
            Context.PosOperations.Add(secondPosOperation);

            Context.SaveChanges();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                posOperationId: FirstPosOperationId,
                goodId: defaultLabeledGood.GoodId.Value,
                labeledGoodId: defaultLabeledGood.PosId.Value,
                currencyId: DefaultCurrencyId,
                editorId: DefaultUserId,
                price: 10M
            );

            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertCheckItemsBelongsToPosOperation(FirstPosOperationId);
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 1);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 1);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonus: 0, expectedBonusesCount: 0);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 1, expectedPaymentAmount: 10M, bankTransactionInfoType: BankTransactionInfoType.Payment);
            AssertExpectedPosOperationBonusInDatabase(0, FirstPosOperationId);
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 1);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 10M,
                fiscalizationType: FiscalizationType.Income);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 10M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void AppendItem_CorrectPosOperationIdIsGivenAndActivePaymentSystemIsNotNullAndOperationStatusIsCompletedAndUserHwoAddCheckItemIsDifferentThenPosOperationUser_ShouldReturnSuccessfulCheckManagerResultAndCompletePurchase(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var posOperation = CreateCompletedPosOperation();

            CreateTransactionForPosOperation(posOperation);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var secondUserId = Context.Users.Last().Id;

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                posOperationId: SecondPosOperationId,
                goodId: defaultLabeledGood.GoodId.Value,
                labeledGoodId: defaultLabeledGood.PosId.Value,
                currencyId: DefaultCurrencyId,
                editorId: secondUserId,
                price: 10M
            );

            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).Result;

            appendItemResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 1);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 1);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonus: 0, expectedBonusesCount: 0);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 1, expectedPaymentAmount: 10M, bankTransactionInfoType: BankTransactionInfoType.Payment);
            AssertExpectedPosOperationBonusInDatabase(0, FirstPosOperationId);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 10M,
                fiscalizationType: FiscalizationType.Income);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 10M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
        AppendItem_CorrectPosOperationIdIsGivenAndActivePaymentSystemIsNotNullAndOperationStatusIsCompletedAndMoreThanOneOperationTransactionsAreGiven_ShouldReturnSuccessfulCheckManagerResultAndCompletePurchase(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var posOperation = CreateCompletedPosOperation(addInitialCheckItem: true);

            CreateTransactionForPosOperation(posOperation);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItem = CheckItem.NewBuilder(
                    DefaultPosId,
                    SecondPosOperationId,
                    defaultLabeledGood.GoodId.Value,
                    defaultLabeledGood.Id,
                    DefaultCurrencyId)
                .SetPrice(10M)
                .MarkAsModifiedByAdmin()
                .SetStatus(CheckItemStatus.Unpaid)
                .Build();

            CreateExtraTransactionForPosOperation(posOperation, checkItem, PosOperationTransactionType.Addition);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                posOperationId: SecondPosOperationId,
                goodId: defaultLabeledGood.GoodId.Value,
                labeledGoodId: defaultLabeledGood.PosId.Value,
                currencyId: DefaultCurrencyId,
                editorId: DefaultUserId,
                price: 10M
            );

            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertExpectedCheckItemAuditRecordInDatabase(expectedCount: 1);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: useNewPaymentSystem ? 3 : 1);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonus: 0, expectedBonusesCount: 0);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(
                expectedCount: useNewPaymentSystem ? 3 : 1,
                expectedPaymentAmount: 30M,
                bankTransactionInfoType: BankTransactionInfoType.Payment,
                expectedFirstBankTransactionAmount: useNewPaymentSystem ? 10M : 30M);
            AssertExpectedPosOperationBonusInDatabase(0, FirstPosOperationId);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 3,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: useNewPaymentSystem ? 3 : 1,
                expectedBankTransactionsAmount: 30M,
                expectedFiscalizationCount: useNewPaymentSystem ? 3 : 1,
                expectedFiscalizationAmount: 30M,
                fiscalizationType: FiscalizationType.Income);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 30M,
                expectedFiscalizationAmount: 30M,
                expectedTotalCost: 30M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RefundItems_IncorrectPosOperationIdIsGiven_ShouldReturnFailureAsCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(
                IncorrectPosOperationId,
                DefaultUserId,
                Enumerable.Empty<int>()
            );

            var refundItemsResult = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult();

            refundItemsResult.IsSuccessful.Should().BeFalse();
            refundItemsResult.Error.Should().NotBeNullOrWhiteSpace();
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 0);
            EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RefundItems_CorrectPaidPosOperationIsGivenAndOneOfCheckItemsIsNotPaid_ShouldReturnSuccessfulCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build(),

                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .MarkAsModifiedByAdmin()
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            };

            MakePaymentAndMarkOperationAsPaid(checkItems);

            var checkItemsToModify = new Dictionary<int, CheckItemStatus>
            {
                {2, CheckItemStatus.Refunded},
            };

            UpdateCheckItemsStatuses(checkItemsToModify);

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1, 2 });

            var refundItemsResult = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult();

            refundItemsResult.IsSuccessful.Should().BeTrue();
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(2, 5M, BankTransactionInfoType.Refund);
            AssertExpectedPosOperationBonusInDatabase(10M, SecondPosOperationId);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(5M, 3);
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Refunded, true);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationIncomeRefundAmount(5M, SecondPosOperationId);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.IncomeRefund);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.Refund);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 5M,
                fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 20M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 30M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RefundItems_CorrectPaidPosOperationIsGivenAndPosOperationPaidAndOneOfCheckItemsIsPaidUnverified_ShouldReturnSuccessfulCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build(),

                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .MarkAsModifiedByAdmin()
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build(),
            };

            MakePaymentAndMarkOperationAsPaid(checkItems);

            var checkItemsToModify = new Dictionary<int, CheckItemStatus>
            {
                {1, CheckItemStatus.PaidUnverified},
                {2, CheckItemStatus.Refunded},
            };

            UpdateCheckItemsStatuses(checkItemsToModify);

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1, 2 });

            var refundItemsResult = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult();

            refundItemsResult.IsSuccessful.Should().BeTrue();
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(2, 5M, BankTransactionInfoType.Refund);
            AssertExpectedPosOperationBonusInDatabase(10M, SecondPosOperationId);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(5M, 3);
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Refunded, true);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationIncomeRefundAmount(5M, SecondPosOperationId);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.IncomeRefund);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.Refund);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 5M,
                fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 20M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 30M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false, 0, 20, 10, 10, 0)]
        [TestCase(true, 0, 20, 10, 10, 0)]
        [TestCase(false, 5, 15, 5, 10, 5)]
        [TestCase(true, 5, 15, 5, 10, 5)]
        [TestCase(false, 15, 5, 0, 10, 10)]
        [TestCase(true, 15, 5, 0, 10, 10)]
        [TestCase(false, 10, 10, 0, 10, 10)]
        [TestCase(true, 10, 10, 0, 10, 10)]
        public void
            RefundItems_CorrectPosOperationIdIsGivenAndPosOperationIsNotPaidAndCheckItemIsDeleted_ShouldReturnSuccessCheckManagerResult(
                bool useNewPaymentSystem,
                decimal bonusPoints,
                decimal expectedMoneyAndFiscalizationAmountBeforeDeletion,
                decimal expectedMoneyAndFiscalizationAmount,
                decimal expectedTotalCostAfterDeletion,
                decimal expectedBonusAmountAfterDeletion)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        FirstPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .MarkAsModifiedByAdmin()
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build(),
                CheckItem.NewBuilder(
                        DefaultPosId,
                        FirstPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .MarkAsModifiedByAdmin()
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            };

            Context.CheckItems.AddRange(checkItems);
            Context.SaveChanges();

            var savedPosOperation = Context.PosOperations.First(po => po.Id == FirstPosOperationId);

            savedPosOperation.AddBonusPoints(bonusPoints);

            Context.SaveChanges();

            CreateTransactionForPosOperation(savedPosOperation);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: bonusPoints,
                expectedMoneyAmount: expectedMoneyAndFiscalizationAmountBeforeDeletion,
                expectedFiscalizationAmount: expectedMoneyAndFiscalizationAmountBeforeDeletion,
                expectedTotalCost: 20M,
                expectedTotalDiscount: 0M);

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(FirstPosOperationId, DefaultUserId, new List<int> { 1 });

            var refundItemsResult = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult();

            refundItemsResult.IsSuccessful.Should().BeTrue();
            var checkItemsInDatabase = Context.CheckItems.AsNoTracking();
            checkItemsInDatabase.Should().NotBeNull();

            foreach (var checkItem in checkItemsInDatabase)
            {
                checkItem.IsModifiedByAdmin.Should().Be(true);
            }

            var firstCheckItem = checkItemsInDatabase.First(cki => cki.Id == FirstCheckItemId);
            firstCheckItem.Status.Should().Be(CheckItemStatus.Deleted);

            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationCountInDatabase(0);
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 0);
            EnsureBankTransactionInfosVersionTwoHaveCountAndAmount(expectedCount: 0, expectedAmount: 0);
            EnsureFiscalizationInfosVersionTwoHaveCountAndAmount(expectedCount: 0, expectedAmount: 0, fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: expectedBonusAmountAfterDeletion,
                expectedMoneyAmount: expectedMoneyAndFiscalizationAmount,
                expectedFiscalizationAmount: expectedMoneyAndFiscalizationAmount,
                expectedTotalCost: expectedTotalCostAfterDeletion,
                expectedTotalDiscount: 0M);

            var paidByBonusPoints = expectedBonusAmountAfterDeletion >= 10;

            if (!paidByBonusPoints)
            {
                Context.PosOperationTransactionCheckItems.Should().HaveCount(1);
                var posOperationTransactionCheckItem = Context.PosOperationTransactionCheckItems.AsNoTracking().First();
                posOperationTransactionCheckItem.Amount.Should().Be(expectedMoneyAndFiscalizationAmount);
                posOperationTransactionCheckItem.CostInBonusPoints.Should().Be(expectedBonusAmountAfterDeletion);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            RefundItems_CorrectPosOperationIdIsGivenAndPosOperationIsNotPaidAndOnlyOneCheckItemIsAddedAndCheckItemIsDeleted_ShouldReturnSuccessCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItem = CheckItem.NewBuilder(
                        DefaultPosId,
                        FirstPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .MarkAsModifiedByAdmin()
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build();

            Context.CheckItems.Add(checkItem);
            Context.SaveChanges();

            var savedPosOperation = Context.PosOperations.First(po => po.Id == FirstPosOperationId);

            Context.SaveChanges();

            CreateTransactionForPosOperation(savedPosOperation);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 10M,
                expectedTotalDiscount: 0M);

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(FirstPosOperationId, DefaultUserId, new List<int> { 1 });

            var refundItemsResult = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult();

            refundItemsResult.IsSuccessful.Should().BeTrue();
            var checkItemsInDatabase = Context.CheckItems.AsNoTracking();
            checkItemsInDatabase.Should().NotBeNull();

            foreach (var cki in checkItemsInDatabase)
            {
                cki.IsModifiedByAdmin.Should().Be(true);
            }

            var firstCheckItem = checkItemsInDatabase.First(cki => cki.Id == FirstCheckItemId);
            firstCheckItem.Status.Should().Be(CheckItemStatus.Deleted);

            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationCountInDatabase(0);
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 0);
            EnsureBankTransactionInfosVersionTwoHaveCountAndAmount(expectedCount: 0, expectedAmount: 0);
            EnsureFiscalizationInfosVersionTwoHaveCountAndAmount(expectedCount: 0, expectedAmount: 0, fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionsHaveCount(expectedCount: 1);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 0M,
                expectedFiscalizationAmount: 0M,
                expectedTotalCost: 0M,
                expectedTotalDiscount: 0M);
            Context.PosOperationTransactionCheckItems.Should().HaveCount(0);
        }

        [TestCase(10, 0, 0, 0, 2, 0, 10, 1, BankTransactionInfoType.Refund, false)]
        [TestCase(5, 5, 5, 1, 2, 0, 5, 1, BankTransactionInfoType.Refund, false)]
        [TestCase(0, 10, 10, 1, 1, 0, 0, 0, BankTransactionInfoType.Payment, false)]
        [TestCase(10, 0, 0, 0, 2, 0, 10, 1, BankTransactionInfoType.Refund, true)]
        [TestCase(5, 5, 5, 1, 2, 0, 5, 1, BankTransactionInfoType.Refund, true)]
        [TestCase(0, 10, 10, 1, 1, 0, 0, 0, BankTransactionInfoType.Payment, true)]
        public void
            RefundItems_CorrectPosOperationIdIsGivenAndPosOperationIsPaid_ShouldReturnSuccessfulCheckManagerResult(
                decimal amountMoney,
                decimal amountBonuses,
                decimal expectedBonusRefund,
                int expectedBonusCount,
                int expectedBankTransactionsCount,
                decimal discountInPercentage,
                decimal expectedFiscalizationRefundAmount,
                int expectedFiscalizationCount,
                BankTransactionInfoType bankTransactionInfoType,
                bool useNewPaymentSystem
            )
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            CreatePosOperationMakeMixPaymentAddCheckItem(amountMoney, amountBonuses, discountInPercentage);

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1 });

            var refundItemsResult = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult();

            refundItemsResult.IsSuccessful.Should().BeTrue();
            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonusRefund, expectedBonusCount);
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Refunded, true);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedBankTransactionsCount, amountMoney, bankTransactionInfoType);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount);
            AssertExpectedFiscalizationIncomeRefundAmount(expectedFiscalizationRefundAmount, SecondPosOperationId);

            if (expectedFiscalizationCount > 0)
            {
                AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.IncomeRefund);
                AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.Refund);
            }

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: expectedBankTransactionsCount - 1,
                expectedBankTransactionsAmount: amountMoney,
                expectedFiscalizationCount: expectedFiscalizationCount,
                expectedFiscalizationAmount: expectedFiscalizationRefundAmount,
                fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: expectedBonusRefund,
                expectedMoneyAmount: amountMoney,
                expectedFiscalizationAmount: expectedFiscalizationRefundAmount,
                expectedTotalCost: amountMoney + amountBonuses,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            RefundItems_CorrectPosOperationIdIsGivenAndPosOperationIsPaidAndOnlyOneCheckItemWithStatusPaidIsGivenAndAddNewCheckItem_ShouldReturnSuccessfulCheckManagerResultAndTwoCorrectionRecordsInFiscalizationInfos(
              bool useNewPaymentSystem
            )
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(20M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            };

            MakePaymentAndMarkOperationAsPaid(checkItems);

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1 });

            var refundItemsResult = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult();

            refundItemsResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Refunded, true);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(15M, 3);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(2, 5M, BankTransactionInfoType.Refund);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationIncomeRefundAmount(5M, SecondPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.IncomeRefund);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.Refund);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 5M,
                fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 30M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 40M,
                expectedTotalDiscount: 0M);

            var checkItemAdditionInfo = CheckItemAdditionInfo.ForAdmin(
                posOperationId: SecondPosOperationId,
                goodId: defaultLabeledGood.GoodId.Value,
                labeledGoodId: defaultLabeledGood.PosId.Value,
                currencyId: DefaultCurrencyId,
                editorId: DefaultUserId,
                price: 20M
            );

            var appendItemResult = _checkManager.AddItemAsync(checkItemAdditionInfo).GetAwaiter().GetResult();

            appendItemResult.IsSuccessful.Should().BeTrue();
            AssertExpectedBonusAmountInDatabaseForDefaultUser(0, 4);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(3, 10M, BankTransactionInfoType.Payment, true);
            AssertExpectedPosOperationBonusInDatabase(15M, SecondPosOperationId);
            AssertExpectedCheckItemAuditRecordInDatabase(2);
            AssertExpectedFiscalizationCountInDatabase(2);
            AssertExpectedFiscalizationCorrectionSum(-5M, SecondPosOperationId);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.Correction);
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 2);
            Context.UserNotifications.Last().NotificationArea.Should().Be(NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 3,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 2,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 2,
                expectedFiscalizationAmount: -5M,
                fiscalizationType: FiscalizationType.Correction);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 45M,
                expectedMoneyAmount: 15M,
                expectedFiscalizationAmount: 15M,
                expectedTotalCost: 60M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(3, 7, 7, 1, 2, 0, 3, 0, BankTransactionInfoType.Refund, false)]
        [TestCase(8, 2, 2, 1, 2, 0, 8, 0, BankTransactionInfoType.Refund, false)]
        [TestCase(7, 2, 2, 1, 2, 5, 7, 1, BankTransactionInfoType.Refund, false)]
        [TestCase(5, 3, 3, 1, 2, 15, 5, 2, BankTransactionInfoType.Refund, false)]
        [TestCase(3, 7, 7, 1, 2, 0, 3, 0, BankTransactionInfoType.Refund, true)]
        [TestCase(8, 2, 2, 1, 2, 0, 8, 0, BankTransactionInfoType.Refund, true)]
        [TestCase(7, 2, 2, 1, 2, 5, 7, 1, BankTransactionInfoType.Refund, true)]
        [TestCase(5, 3, 3, 1, 2, 15, 5, 2, BankTransactionInfoType.Refund, true)]
        public void
            RefundItems_CorrectPaidPosOperationIdIsGiven_ShouldReturnSuccessfulCheckManagerResultInMultipleThreads(
                decimal amountMoney,
                decimal amountBonuses,
                decimal expectedBonusRefund,
                int expectedBonusCount,
                int expectedBankTransactionsCount,
                decimal discountInPercentage,
                decimal expectedFiscalizationRefundAmount,
                decimal discountAmount,
                BankTransactionInfoType bankTransactionInfoType,
                bool useNewPaymentSystem
            )
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            CreatePosOperationMakeMixPaymentAddCheckItem(amountMoney, amountBonuses, discountInPercentage);

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1 });

            var tasks = new Collection<Task<ICheckManagerResult>>();
            for (var i = 0; i < DefaultThreadsCount; i++)
            {
                var checkManager = _serviceProvider.GetRequiredService<ICheckManager>();
                tasks.Add(Task.Factory.StartNew(() => checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult()));
            }

            Task.WaitAll(tasks.ToArray());

            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonusRefund, expectedBonusCount);
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Refunded, true);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedBankTransactionsCount, amountMoney, bankTransactionInfoType);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationIncomeRefundAmount(expectedFiscalizationRefundAmount, SecondPosOperationId);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success,
                FiscalizationType.IncomeRefund);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.Refund);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: expectedBankTransactionsCount - 1,
                expectedBankTransactionsAmount: amountMoney,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: expectedFiscalizationRefundAmount,
                fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: expectedBonusRefund,
                expectedMoneyAmount: amountMoney,
                expectedFiscalizationAmount: expectedFiscalizationRefundAmount,
                expectedTotalCost: amountMoney + amountBonuses + discountAmount,
                expectedTotalDiscount: discountAmount);
        }

        [TestCase(50, 0, 0, 0, 2, 0, 10, 0, BankTransactionInfoType.Refund, false)]
        [TestCase(50, 0, 0, 0, 2, 5, 9, 1, BankTransactionInfoType.Refund, false)]
        [TestCase(50, 0, 0, 0, 2, 0, 10, 0, BankTransactionInfoType.Refund, true)]
        [TestCase(50, 0, 0, 0, 2, 5, 9, 1, BankTransactionInfoType.Refund, true)]
        public void
            RefundItems_CorrectPaidPosOperationIdIsGivenAndTwoCheckItemsWithDifferentAmount_ShouldReturnSuccessfulCheckManagerResultInMultipleThreads(
                decimal amountMoney,
                decimal amountBonuses,
                decimal expectedBonusRefund,
                int expectedBonusCount,
                int expectedBankTransactionsCount,
                decimal discountInPercentage,
                decimal expectedFiscalizationRefundAmount,
                decimal discountAmount,
                BankTransactionInfoType bankTransactionInfoType,
                bool useNewPaymentSystem
            )
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            CreatePosOperationMakeMixPaymentAddCheckItem(amountMoney, amountBonuses, discountInPercentage);

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);
            defaultLabeledGood.MarkAsUsedInPosOperation(SecondPosOperationId);
            var checkItem = CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(40M)
                    .SetStatus(CheckItemStatus.Paid)
                    .Build();

            Context.CheckItems.Add(checkItem);
            Context.SaveChanges();

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1 });

            var tasks = new Collection<Task<ICheckManagerResult>>();
            for (var i = 0; i < DefaultThreadsCount; i++)
            {
                var checkManager = _serviceProvider.GetRequiredService<ICheckManager>();
                tasks.Add(Task.Factory.StartNew(() => checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult()));
            }

            Task.WaitAll(tasks.ToArray());

            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonusRefund, expectedBonusCount);
            var checkItems = Context.CheckItems.AsNoTracking();
            checkItems.Should().NotBeNull();

            checkItems.Single(x => x.Status == CheckItemStatus.Paid).Should().NotBeNull();
            checkItems.Single(x => x.Status == CheckItemStatus.Refunded).Should().NotBeNull();
            checkItems.Single(x => x.Status == CheckItemStatus.Refunded).IsModifiedByAdmin.Should().BeTrue();

            var bankTransactionResult = Context.BankTransactionInfos.AsNoTracking();
            bankTransactionResult.Should().HaveCount(expectedBankTransactionsCount);

            Context.BankTransactionInfos.Where(bti => bti.Type == bankTransactionInfoType).Sum(s => s.Amount)
                .Should().Be(expectedFiscalizationRefundAmount);

            Context.BankTransactionInfos.FirstOrDefault(bti => bti.Type == BankTransactionInfoType.Payment)?.Amount.Should().Be(amountMoney);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationIncomeRefundAmount(expectedFiscalizationRefundAmount, SecondPosOperationId);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success,
                FiscalizationType.IncomeRefund);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.Refund);
            var expectedBankTransacitonAmount = expectedFiscalizationRefundAmount;
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: expectedBankTransactionsCount - 1,
                expectedBankTransactionsAmount: expectedBankTransacitonAmount,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: expectedFiscalizationRefundAmount,
                fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: expectedBonusRefund,
                expectedMoneyAmount: expectedBankTransacitonAmount,
                expectedFiscalizationAmount: expectedFiscalizationRefundAmount,
                expectedTotalCost: expectedBankTransacitonAmount + amountBonuses + discountAmount,
                expectedTotalDiscount: discountAmount);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void MarkCheckItemsAsVerified_IncorrectPosOperationIdIsGiven_ShouldReturnFailureAsCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(
                IncorrectPosOperationId,
                DefaultUserId,
                new List<int>()
            );
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).GetAwaiter().GetResult();

            checkItemsVerifiedResult.IsSuccessful.Should().BeFalse();
            checkItemsVerifiedResult.Error.Should().NotBeNullOrWhiteSpace();
            AssertExpectedCheckItemAuditRecordInDatabase(0);
            AssertExpectedFiscalizationCountInDatabase(0);
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 0);
            EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void MarkCheckItemsAsVerified_CorrectPosOperationIdIsGivenAndNoCheckItemsToEdit_ShouldReturnFailureAsCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(
                FirstPosOperationId,
                DefaultUserId,
                new List<int>()
            );
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).GetAwaiter().GetResult();

            checkItemsVerifiedResult.IsSuccessful.Should().BeFalse();
            checkItemsVerifiedResult.Error.Should().NotBeNullOrWhiteSpace();
            AssertExpectedCheckItemAuditRecordInDatabase(0);
            AssertExpectedFiscalizationCountInDatabase(0);
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 0);
            EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void MarkCheckItemsAsVerified_CorrectPosOperationIdIsGivenAndAddedCorrectCheckItemAndActivePaymentSystemIsNull_ShouldReturnFailureAsCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var operationPaymentInfo = OperationPaymentInfo.ForPaymentViaBonuses(DefaultUserId, 10M);
            var posOperation = Context.PosOperations.First(po => po.Id == FirstPosOperationId);
            posOperation.MarkAs(PosOperationStatus.PendingPayment, operationPaymentInfo);
            Context.SaveChanges();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            Context.CheckItems.Add(
                CheckItem.NewBuilder(
                        DefaultPosId,
                        FirstPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build()
            );

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1 });
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).GetAwaiter().GetResult();

            checkItemsVerifiedResult.IsSuccessful.Should().BeFalse();
            checkItemsVerifiedResult.Error.Should().NotBeNullOrWhiteSpace();
            AssertExpectedCheckItemAuditRecordInDatabase(0);
            AssertExpectedFiscalizationCountInDatabase(0);
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount: 0);
            EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            MarkCheckItemsAsVerified_CorrectPosOperationIsGivenAndActivePaymentSystemIsNotNullAndPosOperationPaidAndFirstCheckItemIsPaidUnverifiedAndSecondCheckItemIsUnverified_ShouldReturnSuccessfulCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.PaidUnverified)
                    .Build(),

                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            };

            MakePaymentAndMarkOperationAsPaid(checkItems, userBonusPoints: 5M);

            var checkItemsToModify = new Dictionary<int, CheckItemStatus>
            {
                {1, CheckItemStatus.PaidUnverified},
                {2, CheckItemStatus.Unverified},
            };

            UpdateCheckItemsStatuses(checkItemsToModify);

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1, 2 });
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).GetAwaiter().GetResult();

            checkItemsVerifiedResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(2, 15M, BankTransactionInfoType.Payment, true);
            AssertExpectedPosOperationBonusInDatabase(5M, SecondPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationCorrectionSum(-10, SecondPosOperationId);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success,
                FiscalizationType.Correction);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: -10M,
                fiscalizationType: FiscalizationType.Correction);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 5M,
                expectedMoneyAmount: 15M,
                expectedFiscalizationAmount: 15M,
                expectedTotalCost: 20M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            MarkCheckItemsAsVerified_CorrectPosOperationIsGivenAndActivePaymentSystemIsNotNullAndPosOperationPaidAndCheckItemIsPaidUnverified_ShouldReturnSuccessfulCheckManagerResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            };

            MakePaymentAndMarkOperationAsPaid(checkItems, userBonusPoints: 5M);

            var checkItemsToModify = new Dictionary<int, CheckItemStatus>
            {
                {1, CheckItemStatus.PaidUnverified}
            };

            UpdateCheckItemsStatuses(checkItemsToModify);

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1 });
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).GetAwaiter().GetResult();

            checkItemsVerifiedResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertExpectedCheckItemAuditRecordInDatabase(2);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(1, 5M, BankTransactionInfoType.Payment, true);
            AssertExpectedPosOperationBonusInDatabase(5M, SecondPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(0);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 0, expectedNotificationArea: NotificationArea.AdditionOrVerification);
        }

        [TestCase(13, 0, 2, 12, 3, 0, 13, PosOperationTransactionStatus.PaidFiscalized, false)]
        [TestCase(20, 0, 1, 5, 3, 0, 20, PosOperationTransactionStatus.PaidByBonusPoints, false)]
        [TestCase(10, 0, 2, 15, 3, 0, 10, PosOperationTransactionStatus.PaidFiscalized, false)]
        [TestCase(25, 5, 1, 5, 3, 0, 20, PosOperationTransactionStatus.PaidByBonusPoints, false)]
        [TestCase(0, 0, 2, 20, 0, 0, 0, PosOperationTransactionStatus.PaidFiscalized, false)]
        [TestCase(13, 0, 2, 9, 3, 10, 13, PosOperationTransactionStatus.PaidFiscalized, false)]
        [TestCase(20, 3, 1, 5, 3, 10, 17, PosOperationTransactionStatus.PaidByBonusPoints, false)]
        [TestCase(0, 0, 2, 18, 0, 10, 0, PosOperationTransactionStatus.PaidFiscalized, false)]
        [TestCase(25, 8, 1, 5, 3, 10, 17, PosOperationTransactionStatus.PaidByBonusPoints, false)]
        [TestCase(13, 0, 2, 12, 3, 0, 13, PosOperationTransactionStatus.PaidFiscalized, true)]
        [TestCase(20, 0, 1, 5, 3, 0, 20, PosOperationTransactionStatus.PaidByBonusPoints, true)]
        [TestCase(10, 0, 2, 15, 3, 0, 10, PosOperationTransactionStatus.PaidFiscalized, true)]
        [TestCase(25, 5, 1, 5, 3, 0, 20, PosOperationTransactionStatus.PaidByBonusPoints, true)]
        [TestCase(0, 0, 2, 20, 0, 0, 0, PosOperationTransactionStatus.PaidFiscalized, true)]
        [TestCase(13, 0, 2, 9, 3, 10, 13, PosOperationTransactionStatus.PaidFiscalized, true)]
        [TestCase(20, 3, 1, 5, 3, 10, 17, PosOperationTransactionStatus.PaidByBonusPoints, true)]
        [TestCase(0, 0, 2, 18, 0, 10, 0, PosOperationTransactionStatus.PaidFiscalized, true)]
        [TestCase(25, 8, 1, 5, 3, 10, 17, PosOperationTransactionStatus.PaidByBonusPoints, true)]
        public void
            MarkCheckItemsAsVerified_CorrectPosOperationIsGivenAndActivePaymentSystemIsNotNullAndPosOperationPaidAndCheckItemsIsUnverifiedAndUserBonusesIsGiven_ShouldReturnSuccessfulCheckManagerResultAndUserBonusesShouldBeAsExpectedAndBankTransactionAmountShouldBeWithoutUserBonusAmount(
                 decimal userBonusPoints,
                 int expectedBonuses,
                 int expectedBankTransactionsCount,
                 decimal expectedBankTransactionsAmount,
                 int expectedBonusesCount,
                 decimal discountInPercentage,
                 decimal posOperationBonusAmount,
                 PosOperationTransactionStatus transactionStatus,
                 bool useNewPaymentSystem
            )
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var firstCheckItemAmount = 5;

            SetActivePaymentCardForDefaultUser();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItems = new List<CheckItem>
            {
CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .MarkAsModifiedByAdmin()
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build(),

                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build(),

                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build()
            };

            if (discountInPercentage > 0)
                checkItems.ForEach(x => x.AddDiscount(discountInPercentage));

            MakePaymentAndMarkOperationAsPaid(checkItems, userBonusPoints);

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 2, 3 });
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).GetAwaiter().GetResult();

            var bankAndFiscalizationAmountWithoutFirstCheckItem = transactionStatus == PosOperationTransactionStatus.PaidByBonusPoints
                ? 0
                : expectedBankTransactionsAmount - firstCheckItemAmount;

            var expectedBankTransactionsAmountWithDiscount = discountInPercentage > 0
                ? expectedBankTransactionsAmount - 1M
                : expectedBankTransactionsAmount;

            var totalAmountWithoutFirstCheckItem = userBonusPoints > firstCheckItemAmount
                ? expectedBankTransactionsAmount - firstCheckItemAmount
                : expectedBankTransactionsAmountWithDiscount;

            var discountAmount = discountInPercentage > 0 ? 3M : 0M;
            var expectedOperationTransactionTotalCost =
                posOperationBonusAmount + totalAmountWithoutFirstCheckItem + discountAmount;

            var expectedBankTransactionAndFiscalizationCount =
                transactionStatus == PosOperationTransactionStatus.PaidByBonusPoints ? 0 : 1;

            checkItemsVerifiedResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedBonusAmountInDatabaseForDefaultUser(expectedBonuses, expectedBonusesCount);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedBankTransactionsCount, expectedBankTransactionsAmount, BankTransactionInfoType.Payment, true);
            AssertExpectedPosOperationBonusInDatabase(posOperationBonusAmount, SecondPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(expectedBankTransactionsCount - 1);
            AssertExpectedFiscalizationCorrectionSum(-(bankAndFiscalizationAmountWithoutFirstCheckItem), SecondPosOperationId);
            if (bankAndFiscalizationAmountWithoutFirstCheckItem > 0)
            {
                AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.Correction);
                AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            }
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: transactionStatus,
                expectedBankTransactionsCount: expectedBankTransactionAndFiscalizationCount,
                expectedBankTransactionsAmount: bankAndFiscalizationAmountWithoutFirstCheckItem,
                expectedFiscalizationCount: expectedBankTransactionAndFiscalizationCount,
                expectedFiscalizationAmount: -bankAndFiscalizationAmountWithoutFirstCheckItem,
                fiscalizationType: FiscalizationType.Correction);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: posOperationBonusAmount,
                expectedMoneyAmount: totalAmountWithoutFirstCheckItem,
                expectedFiscalizationAmount: totalAmountWithoutFirstCheckItem,
                expectedTotalCost: expectedOperationTransactionTotalCost,
                expectedTotalDiscount: discountAmount);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            MarkCheckItemsAsVerified_CorrectCompletedPosOperationIsGivenAndActivePaymentSystemIsNotNullAndCheckItemIsUnverified_ShouldReturnSuccessfulCheckManagerResultAndFiscalizeOperationOnce(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var posOperation = CreateCompletedPosOperation();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);
            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build(),
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            };

            Seeder.Seed(checkItems);

            CreateTransactionForPosOperation(posOperation);

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1 });
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).Result;

            AssertExpectedCheckItemsStatusAndModifiedByAdminSign(checkItemsVerifiedResult);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(1, 20M, BankTransactionInfoType.Payment, true);
            AssertExpectedPosOperationBonusInDatabase(0M, SecondPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationCorrectionSum(0, SecondPosOperationId);
            Context.FiscalizationInfos.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(20M);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.Income);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 20M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 20M,
                fiscalizationType: FiscalizationType.Income);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 20M,
                expectedFiscalizationAmount: 20M,
                expectedTotalCost: 20M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            MarkCheckItemsAsVerified_CorrectCompletedPosOperationIsGivenAndActivePaymentSystemIsNotNullAndCheckItemIsUnverifiedAndUserHwoVerifiedCheckItemsIsDifferentThenPosOperationUser_ShouldReturnSuccessfulCheckManagerResultAndFiscalizeOperationOnce(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            CreateCompletedPosOperation();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            Context.CheckItems.Add(
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build()
            );

            Context.SaveChanges();

            var secondUserId = Context.Users.Last().Id;

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, secondUserId, new List<int> { 1 });
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).Result;

            checkItemsVerifiedResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(1, 10M, BankTransactionInfoType.Payment, true);
            AssertExpectedPosOperationBonusInDatabase(0M, SecondPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationCorrectionSum(0, SecondPosOperationId);
            Context.FiscalizationInfos.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(10M);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.Income);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 10M,
                fiscalizationType: FiscalizationType.Income);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 10M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            MarkCheckItemsAsVerified_CorrectCompletedPosOperationsAreGivenAndActivePaymentSystemIsNotNullAndCheckItemIsUnverified_ShouldReturnSuccessfulCheckManagerResultForNotLastOperation(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            MarkFirstPosOperationAsCompletedAndRememberDate();

            var secondPosOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsCompleted()
                .Build();

            Context.PosOperations.Add(secondPosOperation);

            Context.SaveChanges();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            Context.CheckItems.Add(
                CheckItem.NewBuilder(
                        DefaultPosId,
                        FirstPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build()
            );

            Context.SaveChanges();

            var checkItemsEditingInfo =
                CheckItemsEditingInfo.ForAdmin(FirstPosOperationId, DefaultUserId, new List<int> { 1 });
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo)
                .GetAwaiter().GetResult();

            checkItemsVerifiedResult.IsSuccessful.Should().BeTrue();
            AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus.Paid, true);
            AssertCheckItemsBelongsToPosOperation(FirstPosOperationId);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(1, 10M, BankTransactionInfoType.Payment, true);
            AssertExpectedPosOperationBonusInDatabase(0M, FirstPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationCorrectionSum(0, FirstPosOperationId);
            Context.FiscalizationInfos.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(10M);
            AssertExpectedFiscalizationStateAndType(FirstPosOperationId, FiscalizationState.Success, FiscalizationType.Income);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1,
                expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 10M,
                fiscalizationType: FiscalizationType.Income);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 10M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
           MarkCheckItemsAsVerified_CorrectPaidPosOperationIsGivenAndActivePaymentSystemIsNotNullAndCheckItemIsUnverifiedAndAfterPayUserHasBonusPoints_ShouldReturnSuccessfulCheckManagerResultAndWriteOffBonusPointFromUser(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build(),
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            };

            MakePaymentAndMarkOperationAsPaid(checkItems, userBonusPoints: 15M);

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1 });
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).GetAwaiter().GetResult();

            checkItemsVerifiedResult.IsSuccessful.Should().BeTrue();
            AssertExpectedCheckItemsStatusAndModifiedByAdminSign(checkItemsVerifiedResult);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(2, 10M, BankTransactionInfoType.Payment, true);
            AssertExpectedPosOperationBonusInDatabase(15M, SecondPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(1);
            AssertExpectedFiscalizationCorrectionSum(-5M, SecondPosOperationId);
            Context.FiscalizationInfos.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(0M);
            AssertExpectedFiscalizationStateAndType(SecondPosOperationId, FiscalizationState.Success, FiscalizationType.Correction);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 1, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: -5M,
                fiscalizationType: FiscalizationType.Correction);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 15M,
                expectedMoneyAmount: 5M,
                expectedFiscalizationAmount: 5M,
                expectedTotalCost: 20M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
           MarkCheckItemsAsVerified_CorrectPaidPosOperationIsGivenAndActivePaymentSystemIsNotNullAllBonusPointsWroteOffRefundOneCheckItemAndVerifiAnotherCheckItem_ShouldReturnSuccessfulCheckManagerResultAndWriteOffRefundedBonusPointFromUser(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SetActivePaymentCardForDefaultUser();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build(),
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(5M)
                    .SetStatus(CheckItemStatus.Unverified)
                    .Build(),
                CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
            };

            MakePaymentAndMarkOperationAsPaid(checkItems: checkItems, userBonusPoints: 15M, paidByBonusPoints: true);

            var checkItemDeletionOrConfirmationInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 3 });

            var refundItemsResult = _checkManager.RefundOrDeleteItemsAsync(checkItemDeletionOrConfirmationInfo).GetAwaiter().GetResult();

            refundItemsResult.IsSuccessful.Should().BeTrue();
            AssertExpectedBonusAmountInDatabaseForDefaultUser(15M, 3);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(0, 0M, BankTransactionInfoType.Refund);
            AssertExpectedCheckItemAuditRecordInDatabase(1);
            AssertExpectedFiscalizationIncomeRefundAmount(0M, SecondPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(0);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 0, expectedNotificationArea: NotificationArea.Refund);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.IncomeRefund);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 20M,
                expectedMoneyAmount: 0M,
                expectedFiscalizationAmount: 0M,
                expectedTotalCost: 20M,
                expectedTotalDiscount: 0M);

            var checkItemsEditingInfo = CheckItemsEditingInfo.ForAdmin(SecondPosOperationId, DefaultUserId, new List<int> { 1, 2 });
            var checkItemsVerifiedResult = _checkManager.MarkCheckItemsAsVerifiedAsync(checkItemsEditingInfo).GetAwaiter().GetResult();

            checkItemsVerifiedResult.IsSuccessful.Should().BeTrue();
            var checkItemsInDatabase = Context.CheckItems.AsNoTracking();
            checkItemsInDatabase.Should().NotBeNull();
            var firstcheckItem = checkItemsInDatabase.OrderBy(i => i.Id).First();
            firstcheckItem.IsModifiedByAdmin.Should().Be(true);
            var secondCheckItem = checkItemsInDatabase.OrderBy(i => i.Id).Last();
            secondCheckItem.IsModifiedByAdmin.Should().Be(true);

            AssertExpectedCheckItemAuditRecordInDatabase(3);
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(0, 0M, BankTransactionInfoType.Payment, true);
            AssertExpectedPosOperationBonusInDatabase(10M, SecondPosOperationId);
            AssertExpectedFiscalizationCountInDatabase(0);
            AssertExpectedUserNotificationsCountAndArea(expectedUserNotificationCount: 0, expectedNotificationArea: NotificationArea.AdditionOrVerification);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 3,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.Correction);
            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 30M,
                expectedMoneyAmount: 0M,
                expectedFiscalizationAmount: 0M,
                expectedTotalCost: 30M,
                expectedTotalDiscount: 0M);
        }

        private PosOperation CreateCompletedPosOperation(bool addInitialCheckItem = false)
        {
            var posOperationBuilder = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsCompleted();
            if (addInitialCheckItem)
            {
                var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);
                posOperationBuilder.SetCheckItems(new List<CheckItem>
                {
                    CheckItem.NewBuilder(
                            DefaultPosId,
                            SecondPosOperationId,
                            defaultLabeledGood.GoodId.Value,
                            defaultLabeledGood.Id,
                            DefaultCurrencyId)
                        .SetPrice(10M)
                        .MarkAsModifiedByAdmin()
                        .SetStatus(CheckItemStatus.Unpaid)
                        .Build()});
            }

            var posOperation = posOperationBuilder.Build();

            Context.PosOperations.Add(posOperation);

            Context.SaveChanges();

            return posOperation;
        }
        private void AssertExpectedCheckItemsStatusAndModifiedByAdminSign(ICheckManagerResult checkItemsVerifiedResult)
        {
            checkItemsVerifiedResult.IsSuccessful.Should().BeTrue();
            var checkItemsInDatabase = Context.CheckItems.AsNoTracking();
            checkItemsInDatabase.Should().NotBeNull();
            var firstcheckItem = checkItemsInDatabase.OrderBy(i => i.Id).First();
            firstcheckItem.IsModifiedByAdmin.Should().Be(true);
            var secondCheckItem = checkItemsInDatabase.OrderBy(i => i.Id).Last();
            secondCheckItem.IsModifiedByAdmin.Should().Be(false);
            foreach (var checkItem in checkItemsInDatabase)
            {
                checkItem.Status.Should().Be(CheckItemStatus.Paid);
            }
        }

        private void MakePaymentAndMarkOperationAsPaid(ICollection<CheckItem> checkItems, decimal? userBonusPoints = null, bool paidByBonusPoints = false)
        {
            var firstUser = Context.Users.Single(u => u.Id == DefaultUserId);
            firstUser.AddBonusPoints(userBonusPoints ?? 15M, BonusType.Payment);

            var paymentRequest = new PaymentRequest(
                15M, Currency.Rubles, DefaultCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResult = _paymentService.MakePaymentAsync(paymentRequest).GetAwaiter().GetResult();

            var bankTransactionSummary = new BankTransactionSummary(
                Context.PaymentCards.First().Id, paymentResult.Result.TransactionId, 5M
            );

            var operationPaymentInfo = paidByBonusPoints
                ? OperationPaymentInfo.ForPaymentViaBonuses(DefaultUserId, 10M)
                : OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, userBonusPoints ?? 15M);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetCheckItems(checkItems)
                .Build();

            Seeder.Seed(new List<PosOperation>
            {
                posOperation
            });

            AddBonusPointsToPosOperationAndMarkAsPaidAndAddPosOperationTransaction(operationPaymentInfo);
        }

        private void AddBonusPointsToPosOperationAndMarkAsPaidAndAddPosOperationTransaction(OperationPaymentInfo operationPaymentInfo)
        {
            var savedPosOperation =
                Context.PosOperations.First(po => po.Id == SecondPosOperationId);
            savedPosOperation.WriteOffBonusPoints();

            CreateTransactionForPosOperation(savedPosOperation);

            savedPosOperation.MarkAs(PosOperationStatus.Paid, operationPaymentInfo);

            Context.SaveChanges();

        }

        private void UpdateCheckItemsStatuses(Dictionary<int, CheckItemStatus> checkItemsToModify)
        {
            var savedCheckItems = Context.CheckItems.Where(cki => checkItemsToModify.ContainsKey(cki.Id)).ToList();
            savedCheckItems.ForEach(cki => cki.SetProperty("Status", checkItemsToModify[cki.Id]));
            Context.SaveChanges();
        }

        private void CreatePosOperationMakeMixPaymentAddCheckItem(decimal amountMoney, decimal amountBonuses, decimal discountInPercentage = 0)
        {
            var paymentRequest = new PaymentRequest(
                amountMoney + amountBonuses, Currency.Rubles, DefaultCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResult = _paymentService.MakePaymentAsync(paymentRequest).GetAwaiter().GetResult();

            var bankTransactionSummary = new BankTransactionSummary(
                Context.PaymentCards.First().Id, paymentResult.Result.TransactionId, amountMoney
            );
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, amountBonuses);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(operationPaymentInfo)
                .Build();

            Context.PosOperations.Add(posOperation);

            Context.SaveChanges();

            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);
            defaultLabeledGood.MarkAsUsedInPosOperation(SecondPosOperationId);
            var checkItem = CheckItem.NewBuilder(
                        DefaultPosId,
                        SecondPosOperationId,
                        defaultLabeledGood.GoodId.Value,
                        defaultLabeledGood.Id,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Paid)
                    .Build();

            if (discountInPercentage > 0)
                checkItem.AddDiscount(discountInPercentage);

            Context.CheckItems.Add(checkItem);
            Context.SaveChanges();
        }

        private void AssertExpectedFiscalizationCountInDatabase(int expectedFiscalizationCount)
        {
            Context.FiscalizationInfos.AsNoTracking().Count().Should().Be(expectedFiscalizationCount);
        }

        private void AssertCheckItemsInDatabaseCorrespondToAction(CheckItemStatus expectedAction, bool expectedIsModifiedByAdmin = false)
        {
            var checkItems = Context.CheckItems.AsNoTracking();
            checkItems.Should().NotBeNull();

            foreach (var checkItem in checkItems)
            {
                checkItem.Status.Should().Be(expectedAction);
                checkItem.IsModifiedByAdmin.Should().Be(expectedIsModifiedByAdmin);
            }
        }

        private void AssertCheckItemsBelongsToPosOperation(int posOperationId)
        {
            var checkItems = Context.CheckItems.AsNoTracking();
            checkItems.Should().NotBeNull();

            foreach (var checkItem in checkItems)
                checkItem.PosOperationId.Should().Be(posOperationId);
        }

        private void
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(
                int expectedCount,
                decimal? expectedPaymentAmount,
                BankTransactionInfoType bankTransactionInfoType,
                bool isCheckItemVerifiedMethod = false,
                decimal? expectedFirstBankTransactionAmount = null)
        {
            var bankTransactionResult = Context.BankTransactionInfos.AsNoTracking();
            bankTransactionResult.Should().HaveCount(expectedCount);

            if (expectedCount > 1)
                Context.BankTransactionInfos.Where(bti => bti.Type == bankTransactionInfoType).Sum(s => s.Amount)
                    .Should().Be(expectedPaymentAmount);

            if (isCheckItemVerifiedMethod)
                return;

            Context.BankTransactionInfos.FirstOrDefault(bti => bti.Type == BankTransactionInfoType.Payment)?.Amount.Should().Be(expectedFirstBankTransactionAmount ?? expectedPaymentAmount);
        }

        private void AssertExpectedFiscalizationStateAndType(int posOperationId, FiscalizationState expectedState,
            FiscalizationType expectedType)
        {
            var fiscalizationInfo = Context.FiscalizationInfos.AsNoTracking().OrderByDescending(x => x.DateTimeRequest)
                .FirstOrDefault(x => x.PosOperationId == posOperationId);

            fiscalizationInfo.State.Should().Be(expectedState);
            fiscalizationInfo.Type.Should().Be(expectedType);
        }

        private void AssertExpectedBonusAmountInDatabaseForDefaultUser(decimal? expectedBonus, int expectedBonusesCount)
        {
            var user = Context.Users
                .Include(u => u.BonusPoints)
                .AsNoTracking()
                .Single(u => u.Id == DefaultUserId);

            user.TotalBonusPoints.Should().Be(expectedBonus);
            user.BonusPoints.Should().HaveCount(expectedBonusesCount);
        }

        private void AssertExpectedPosOperationBonusInDatabase(decimal? expectedBonus, int operationId)
        {
            var posOperationResult = Context.PosOperations.AsNoTracking().First(pos => pos.Id == operationId);
            posOperationResult.BonusAmount.Should().Be(expectedBonus);
        }

        private void AssertExpectedCheckItemAuditRecordInDatabase(int expectedCount)
        {
            Context.CheckItemsAuditHistory.Should().HaveCountGreaterOrEqualTo(expectedCount);
        }

        private void AssertExpectedFiscalizationCorrectionAmount(decimal expectedFiscalizationCorrectionAmount,
            int posOperationId)
        {
            Context.FiscalizationInfos.AsNoTracking().Where(x => x.PosOperationId == posOperationId)
                ?.Sum(p => p.CorrectionAmount)
                .Should().Be(expectedFiscalizationCorrectionAmount);
        }

        private void AssertExpectedFiscalizationCorrectionSum(decimal expectedFiscalizationCorrectionSum,
            int posOperationId)
        {
            Context.FiscalizationInfos.AsNoTracking().Where(x => x.PosOperationId == posOperationId).Sum(x => x.CorrectionAmount)
                .Should().Be(expectedFiscalizationCorrectionSum);
        }

        private void SetActivePaymentCardMarkOperationAsPaidAddUserBonusPoints(decimal amountBonuses)
        {
            SetActivePaymentCardForDefaultUser();

            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                .Build();
            Context.PosOperations.Add(posOperation);

            var firstUser = Context.Users.Single(u => u.Id == DefaultUserId);
            firstUser.AddBonusPoints(amountBonuses, BonusType.FirstPay);

            Context.SaveChanges();
        }

        private void SetActivePaymentCardForDefaultUser()
        {
            var firstUser = Context.Users
                .Include(u => u.PaymentCards)
                .Include(u => u.BonusPoints)
                .Single(u => u.Id == DefaultUserId);
            firstUser.SetActivePaymentCard(Context.PaymentCards.First().Id);

            Context.SaveChanges();
        }

        private void AssertExpectedFiscalizationIncomeRefundAmount(decimal expectedFiscalizationIncomeRefundAmount, int posOperationId)
        {
            Context.FiscalizationInfos
                .AsNoTracking()
                .FirstOrDefault(x => x.PosOperationId == posOperationId &&
                                x.State == FiscalizationState.Success &&
                                x.Type == FiscalizationType.IncomeRefund)?.TotalFiscalizationAmount
                .Should()
                .Be(expectedFiscalizationIncomeRefundAmount);
        }

        private void AssertExpectedUserNotificationsCount(int expectedUserNotificationCount)
        {
            _waitHandler.WaitOne(TimeSpan.FromSeconds(5));
            Context.UserNotifications.Should().HaveCount(expectedUserNotificationCount);
        }

        private void AssertExpectedUserNotificationsCountAndArea(int expectedUserNotificationCount,
            NotificationArea expectedNotificationArea)
        {
            AssertExpectedUserNotificationsCount(expectedUserNotificationCount);

            if (expectedUserNotificationCount > 0)
                Context.UserNotifications.FirstOrDefault()?.NotificationArea.Should().Be(expectedNotificationArea);
        }

        private PosOperation MarkFirstPosOperationAsCompletedAndRememberDate()
        {
            var posOperation = Context.PosOperations.First(p => p.Id == FirstPosOperationId);
            posOperation.MarkAsPendingCompletion();
            posOperation.MarkAsPendingCheckCreation();
            posOperation.MarkAsCompletedAndRememberDate();

            return posOperation;
        }

        private void CreateTransactionForPosOperation(PosOperation posOperation)
        {
            _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, posOperation).GetAwaiter().GetResult();
        }

        private void CreateExtraTransactionForPosOperation(PosOperation posOperation, CheckItem checkItem, PosOperationTransactionType transactionType)
        {
            posOperation.AddCheckItem(checkItem);
            Context.SaveChanges();

            var posOperationTransactionCreationInfo = new PosOperationTransactionCreationInfo(
                posOperation,
                new List<CheckItem> { checkItem },
                0M,
                transactionType);

            var operationTransaction = _transactionCreationUpdatingService.CreateTransaction(posOperationTransactionCreationInfo);

            posOperation.AddTransaction(operationTransaction);

            Context.SaveChanges();
        }

        private void MarkPosToUseNewPaymentSystemIfItIsNeeded(bool useNewPaymentSystem)
        {
            if (useNewPaymentSystem)
            {
                var poses = Context.PointsOfSale.ToImmutableList();
                poses.ForEach(pos => pos.SetProperty("UseNewPaymentSystem", true));
                Context.SaveChanges();
            }
        }

        private void EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
            int expectedTransactionsCount,
            PosOperationTransactionStatus expectedTransactionStatus,
            int expectedBankTransactionsCount,
            decimal expectedBankTransactionsAmount,
            int expectedFiscalizationCount,
            decimal expectedFiscalizationAmount,
            FiscalizationType fiscalizationType)
        {
            var fiscalizaionAmount = expectedTransactionStatus == PosOperationTransactionStatus.Unpaid
                ? 0
                : expectedFiscalizationAmount;
            var bankTransactionAmount = expectedTransactionStatus == PosOperationTransactionStatus.Unpaid
                ? 0
                : expectedBankTransactionsAmount;

            if (fiscalizaionAmount > 0 && bankTransactionAmount > 0)
                EnsurePosOperationTransactionsHaveLastBankTransactionInfoAndLastFisclizationInfos();

            EnsurePosOperationTransactionsHaveCount(expectedTransactionsCount);
            EnsureBankTransactionInfosVersionTwoHaveCountAndAmount(expectedBankTransactionsCount, bankTransactionAmount);
            EnsureFiscalizationInfosVersionTwoHaveCountAndAmount(expectedFiscalizationCount, fiscalizaionAmount, fiscalizationType);
        }

        private void EnsurePosOperationTransactionHasCorrectTotalAmounts(
            decimal expectedBonusAmount,
            decimal expectedMoneyAmount,
            decimal expectedFiscalizationAmount,
            decimal expectedTotalCost,
            decimal expectedTotalDiscount)
        {
            var posOperationTransactions = Context.PosOperationTransactions.AsNoTracking().ToImmutableList();
            posOperationTransactions.Sum(pot => pot.BonusAmount).Should().Be(expectedBonusAmount);
            posOperationTransactions.Sum(pot => pot.MoneyAmount).Should().Be(expectedMoneyAmount);
            posOperationTransactions.Sum(pot => pot.FiscalizationAmount).Should().Be(expectedFiscalizationAmount);
            posOperationTransactions.Sum(pot => pot.TotalCost).Should().Be(expectedTotalCost);
            posOperationTransactions.Sum(pot => pot.TotalDiscountAmount).Should().Be(expectedTotalDiscount);
        }

        private void EnsureBankTransactionInfosVersionTwoHaveCountAndAmount(int expectedCount, decimal expectedAmount)
        {
            Context.BankTransactionInfosVersionTwo.Count().Should().Be(expectedCount);
            Context.BankTransactionInfosVersionTwo.Sum(bti => bti.Amount).Should().Be(expectedAmount);
        }

        private void EnsureFiscalizationInfosVersionTwoHaveCountAndAmount(int expectedCount, decimal expectedAmount, FiscalizationType fiscalizationType)
        {
            Context.FiscalizationInfosVersionTwo.Count().Should().Be(expectedCount);
            if (fiscalizationType == FiscalizationType.Correction)
                Context.FiscalizationInfosVersionTwo.Sum(fi => fi.CorrectionAmount).Should().Be(expectedAmount);
            else
                Context.FiscalizationInfosVersionTwo.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(expectedAmount);
        }

        private void EnsurePosOperationTransactionsHaveCount(int expectedCount)
        {
            Context.PosOperationTransactions.Should().HaveCount(expectedCount);
        }

        private void EnsurePosOperationTransactionsHaveLastBankTransactionInfoAndLastFisclizationInfos()
        {
            Context.PosOperationTransactions.Last().LastBankTransactionInfoId.Should().NotBeNull();
            Context.PosOperationTransactions.Last().LastFiscalizationInfoId.Should().NotBeNull();
        }

        private void EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists()
        {
            Context.PosOperationTransactions.Should().BeEmpty();
            Context.BankTransactionInfosVersionTwo.Should().BeEmpty();
            Context.FiscalizationInfosVersionTwo.Should().BeEmpty();
            Context.PosOperationTransactionCheckItems.Should().BeEmpty();
        }

        private void EnsurePosOperationTransactionHasStatusAndType(
            PosOperationTransactionStatus expectedTransactionStatus,
            PosOperationTransactionType expectedTransactionType)
        {
            var posOperationTransactions = Context.PosOperationTransactions.ToImmutableList();
            posOperationTransactions.ForEach(pot => pot.Status.Should().Be(expectedTransactionStatus));
            posOperationTransactions.ForEach(pot => pot.Type.Should().Be(expectedTransactionType));
        }
    }
}

