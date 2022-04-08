using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Dtos.PurchaseCompletionResult;
using NasladdinPlace.Api.Services.WebSocket.Controllers;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Contracts;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Models;
using NasladdinPlace.Api.Services.WebSocket.ControllersInvocation.Routing.Models;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.WebSocket.CommandsExecution;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.Core.Services.UnpaidPurchases.Finisher;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.Logging;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Tests.Scenarios.Shopping
{
    public class ShoppingCompletionScenario : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 1;
        private const int DefaultPosOperationId = 1;
        private const int DefaultGoodId = 1;
        private const int DefaultLabeledGoodId = 1;
        private const int DefaultCurrencyId = 1;
        private const decimal DefaultPrice = 15M;
        private const decimal DefaultBonusAmountForFirstPay = 50M;
        private const int DefaultThreadsCount = 8;
        private const int MiddleOfThreads = DefaultThreadsCount / 2;

        private AccountingBalancesController _wsAccountingBalancesController;

        private CurrentPurchaseController _currentPurchaseController;

        private PurchasesController _purchasesController;
        private IUnpaidPurchaseFinisher _unpaidPurchaseFinisher;
        private IOperationsManager _operationsManager;
        private IUnitOfWork _unitOfWork;
        private IOperationTransactionManager _operationTransactionManager;
        private AutoResetEvent _queueIsEmptyForFirstPosWaitHandle;
        private WsCommandsQueueProcessor _processor;

        private IServiceProvider ServiceProvider { get; set; }
        private ControllerContext ControllerContext { get; set; }

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
            Seeder.Seed(new IncompletePosOperationsDataSet(posId: DefaultPosId, userId: DefaultUserId));
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            Seeder.Seed(new PromotionSettingsDataSet());

            var firstUser = Context.Users
                .Include(u => u.PaymentCards)
                .Single(u => u.Id == 1);
            firstUser.SetActivePaymentCard(Context.PaymentCards.First().Id);
            Context.SaveChanges();

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();

            ServiceProvider = serviceProvider;

            _queueIsEmptyForFirstPosWaitHandle = new AutoResetEvent(false);

            _processor = CreateWsCommandsQueueProcessorAndSubscribeOnEvent(_queueIsEmptyForFirstPosWaitHandle);

            var mockPosRealTimeInfo = serviceProvider.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>();
            mockPosRealTimeInfo.Setup(p => p.GetOrAddById(DefaultPosId)).Returns(new PosRealTimeInfo(DefaultPosId)
            {
                CommandsQueueProcessor = _processor
            });
            mockPosRealTimeInfo.Setup(p => p.GetOrAddById(2)).Returns(new PosRealTimeInfo(2));

            var controllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);

            ControllerContext = controllerContext;
            _wsAccountingBalancesController = serviceProvider.GetRequiredService<AccountingBalancesController>();

            _currentPurchaseController = serviceProvider.GetRequiredService<CurrentPurchaseController>();
            _currentPurchaseController.ControllerContext = controllerContext;

            _purchasesController = serviceProvider.GetRequiredService<PurchasesController>();
            _purchasesController.ControllerContext = controllerContext;

            _unpaidPurchaseFinisher = serviceProvider.GetRequiredService<IUnpaidPurchaseFinisher>();

            _operationsManager = serviceProvider.GetRequiredService<IOperationsManager>();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _operationTransactionManager = serviceProvider.GetRequiredService<IOperationTransactionManager>();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldCreateCorrectCheckForUserAndPayForPurchase(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            MarkOpenedPosOperationsAsPendingCompletion();
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3
            );
            EnsureLastPosOperationIsPaidByMoney(
                expectedPaymentAmount: 25M
            );

            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 25M,
                expectedPosOperationCheckItemsBonusPointsAmount: 0M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 25M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 25M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 25M,
                expectedFiscalizationAmount: 25M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            VerifyLabelsSynchronizationAndCompletePurchase_TwoOutOfThreeGoodsAreTaken_ShouldAssignLabeledGoodsToUserAndNotifyCheckVerificationCompletionAndPayForPurchase(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            MarkOpenedPosOperationsAsPendingCompletion();
            VerifyLabelsSynchronization_TwoOutOfThreeGoodsAreTaken_ShouldAssignTakenGoodsToUserAndNotifyCheckVerificationCompletion(2);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 1,
                expectedTotalPrice: 10M,
                expectedTotalQuantity: 2
            );
            EnsureLastPosOperationIsPaidByMoney(
                expectedPaymentAmount: 10M
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 2,
                expectedPosOperationCheckItemsAmount: 10M,
                expectedPosOperationCheckItemsBonusPointsAmount: 0M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 10M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 10M,
                expectedFiscalizationAmount: 10M,
                expectedTotalCost: 10M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(true)]
        [TestCase(true, true)]
        [TestCase(false)]
        public void
            VerifyLabelsSynchronizationAndCompletePurchase_TwoOutOfThreeGoodsAreTakenTheSynchronizeCommandSentInMultiplyThreads_ShouldAssignLabeledGoodsToUserAndNotifyCheckVerificationCompletionAndPayForPurchase(
                bool useTheSameMessage, bool insertNewMessageInTheMiddle = false)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(true);
            MarkOpenedPosOperationsAsPendingCompletion();

            var labelsInPos = new Collection<string> { "E2 80 11 60 60 00 02 05 2A 98 AB 11" };

            var wsMessage = CreateWsMessage(labelsInPos, DefaultPosId);
            var newWsMessage = CreateWsMessage(labelsInPos, DefaultPosId);

            var tasks = new Collection<Task>();
            for (var i = 0; i < DefaultThreadsCount; i++)
            {
                var webSocket = CreateWebSocket();
                var message = useTheSameMessage ? wsMessage : CreateWsMessage(labelsInPos, DefaultPosId);
                var isInsertNewMessage = insertNewMessageInTheMiddle && i == MiddleOfThreads;

                var wsControllerInvoker = ServiceProvider.GetRequiredService<IWsControllerInvoker>();

                tasks.Add(Task.Factory.StartNew(() => wsControllerInvoker.InvokeAsync(webSocket, isInsertNewMessage ? newWsMessage : message).GetAwaiter().GetResult()));
            }

            Task.WaitAll(tasks.ToArray());

            _queueIsEmptyForFirstPosWaitHandle.WaitOne();

            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 1,
                expectedTotalPrice: 10M,
                expectedTotalQuantity: 2
            );
            EnsureLastPosOperationIsPaidByMoney(
                expectedPaymentAmount: 10M
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 2,
                expectedPosOperationCheckItemsAmount: 10M,
                expectedPosOperationCheckItemsBonusPointsAmount: 0M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 10M);

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
            VerifyLabelsSynchronizationAndCompletePurchase_NoGoodIsTaken_ShouldNotifyAboutCheckVerificationCompletionAndCompletePurchase(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            MarkOpenedPosOperationsAsPendingCompletion();
            VerifyLabelsSynchronization_NoGoodIsTaken_ShouldNotifyCheckVerificationCompletion();
            EnsureLastPosOperationCheckIsEmpty();
            EnsureLastPosOperationIsCompletedAndBankTransactionNotExists();
            EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            VerifyLabelsSynchronizationAndPayForPurchase_PosContentIsTakenAnd200BonusesAreGiven_ShouldAssignLabeledGoodsToUserAndNotifyCheckVerificationCompletionAndPayForPurchaseAdd50BonusesForFirstPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            AddBonusForUser(DefaultUserId, 200M);
            MarkOpenedPosOperationsAsPendingCompletion();
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3
            );
            EnsureLastPosOperationIsPaidByBonuses(
                expectedBonusesRemainder: 225M,
                expectedPosOperationsCount: 1
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 0M,
                expectedPosOperationCheckItemsBonusPointsAmount: 25M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 25M,
                expectedMoneyAmount: 0M,
                expectedFiscalizationAmount: 0M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            VerifyLabelsSynchronizationAndPayForPurchase_PosContentIsTakenAnd24BonusesAreGiven_ShouldAssignLabeledGoodsToUserAndNotifyCheckVerificationCompletionAndPayForPurchaseAdd50BonusesForFirstPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            AddBonusForUser(DefaultUserId, 24M);
            MarkOpenedPosOperationsAsPendingCompletion();
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 1M,
                expectedPosOperationCheckItemsBonusPointsAmount: 24M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 1M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 1M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 24M,
                expectedMoneyAmount: 1M,
                expectedFiscalizationAmount: 1M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            VerifyLabelsSynchronizationAndPayForPurchase_PosContentIsTakenAnd50BonusesAreGivenAndReassignedPrice_ShouldAssignLabeledGoodsToUserAndNotifyCheckVerificationCompletionAndPayForPurchaseAdd50BonusesForFirstPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            AddBonusForUser(DefaultUserId, 50M);
            MarkOpenedPosOperationsAsPendingCompletion();
            var labelsToChangePrice = new List<string> { "E2 00 00 16 18 0B 01 66 15 20 7E EA", "E2 80 11 60 60 00 02 05 2A 98 4B A1" };
            var labeledGoodsToChangePrice = Context.LabeledGoods.Where(lg => labelsToChangePrice.Contains(lg.Label)).ToImmutableList();
            var firstLabeledGood = labeledGoodsToChangePrice.First();
            firstLabeledGood.SetProperty("Price", 1M);
            var secondlabeledGood = labeledGoodsToChangePrice.Last();
            secondlabeledGood.SetProperty("Price", 50M);
            Context.SaveChanges();

            VerifyLabelsSynchronization_TwoOutOfThreeGoodsAreTaken_ShouldAssignTakenGoodsToUserAndNotifyCheckVerificationCompletion(2);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 51M,
                expectedTotalQuantity: 2
            );

            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 2,
                expectedPosOperationCheckItemsAmount: 1M,
                expectedPosOperationCheckItemsBonusPointsAmount: 50M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 1M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 1M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 50M,
                expectedMoneyAmount: 1M,
                expectedFiscalizationAmount: 1M,
                expectedTotalCost: 51M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void VerifyFirstPayBonus_PosContentIsTakenAnd200BonusesAreGiven_ShouldAdd50BonusesForFirstPayAnd0BonusesForSecondPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            AddBonusForUser(DefaultUserId, 200M);

            MarkOpenedPosOperationsAsPendingCompletion();

            var posAccountingBalances = new PosAccountingBalancesDto
            {
                Labels = new Collection<string>(),
                PosId = DefaultPosId
            };
            _wsAccountingBalancesController.Synchronize(posAccountingBalances).GetAwaiter().GetResult();

            EnsureLastPosOperationIsPaidByBonuses(
                expectedBonusesRemainder: 225M,
                expectedPosOperationsCount: 1
            );

            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 0M,
                expectedPosOperationCheckItemsBonusPointsAmount: 25M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 25M,
                expectedMoneyAmount: 0M,
                expectedFiscalizationAmount: 0M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 0M);

            const int posId = 2;

            Seeder.Seed(new IncompletePosOperationsDataSet(posId: posId, userId: DefaultUserId));
            Seeder.Seed(LabeledGoodsDataSet.FromPosIdWithLabels(
                posId,
                "E2 00 00 16 18 0B 01 66 15 20 7E 14",
                "E2 80 11 60 60 00 02 05 2A 98 4B 13",
                "E2 80 11 60 60 00 02 05 2A 98 AB 12"
            ));

            MarkOpenedPosOperationsAsPendingCompletion();

            posAccountingBalances = new PosAccountingBalancesDto
            {
                PosId = posId,
                Labels = new Collection<string>()
            };
            _wsAccountingBalancesController.Synchronize(posAccountingBalances).GetAwaiter().GetResult();

            EnsureLastPosOperationIsPaidByBonuses(
                expectedBonusesRemainder: 200M,
                expectedPosOperationsCount: 2
            );

            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 6,
                expectedPosOperationCheckItemsAmount: 0M,
                expectedPosOperationTransactionsCount: 2,
                expectedPosOperationCheckItemsBonusPointsAmount: 50M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 50M,
                expectedMoneyAmount: 0M,
                expectedFiscalizationAmount: 0M,
                expectedTotalCost: 50M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            VerifyLabelsSynchronizationAndPayForPurchase_PosContentIsTakenAnd10BonusesAreGiven_ShouldAssignLabeledGoodsToUserAndNotifyCheckVerificationCompletionAndPayForPurchaseAdd50BonusesForFirstPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            AddBonusForUser(DefaultUserId, 10M);
            MarkOpenedPosOperationsAsPendingCompletion();
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3
            );
            EnsureLastPosOperationIsPaidByBonusesAndMoney(
                expectedBonusesRemainder: 50M,
                expectedMoneyPaymentSum: 15M
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 15M,
                expectedPosOperationCheckItemsBonusPointsAmount: 10M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 15M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 15M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 10M,
                expectedMoneyAmount: 15M,
                expectedFiscalizationAmount: 15M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CompleteStaledPurchase_PosContentIsTakenAndStaledUnpaidPurchaseIsGiven_ShouldCompletePurchaseAndPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            MarkOpenedPosOperationsAsPendingCompletion();
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3
            );
            CompleteStaledPurchase_PosContentIsTaken_ShouldHaveBankTransactionInfoAndCompleteOperation(
                expectedPaymentAmount: 25M
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 25M,
                expectedPosOperationCheckItemsBonusPointsAmount: 0M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 25M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 25M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 25M,
                expectedFiscalizationAmount: 25M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            CompleteStaledPurchase_PosContentIsTakenAnd200BonusesAreGiven_ShouldPayForPurchaseAdd50BonusesForFirstPayAndCompleteOperation(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            AddBonusForUser(DefaultUserId, 200M);
            MarkOpenedPosOperationsAsPendingCompletion();
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3
            );
            CompleteStaledPurchase_BonusThatCoversCostIsGiven_ShouldCompleteOperationAndNotCreateBankTransaction(
                expectedBonusesRemainder: 225M
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 0M,
                expectedPosOperationCheckItemsBonusPointsAmount: 25M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 25M,
                expectedMoneyAmount: 0M,
                expectedFiscalizationAmount: 0M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CompleteStaledPurchase_PosContentIsTakenAndStaledUnpaidPurchaseAndIncorrectCardTokenAreGiven_ShouldNotCompletePurchaseAndPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            MarkOpenedPosOperationsAsPendingCompletion();
            DeleteUsersCardTokens();

            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3
            );
            CompleteStaledPurchase_PosContentIsTakenAndIncorrectCardTokenIsGiven_ShouldNotCompleteOperationAndCreateBankTransaction();
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 25M,
                expectedPosOperationCheckItemsBonusPointsAmount: 0M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 25M,
                expectedFiscalizationAmount: 25M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CompleteStaledPurchase_PosContentIsTakenAndTwoOutOfThreeGoodsAreBlocked_ShouldPayForPurchaseAndCompleteOperationWithCorrectCheck(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            MarkOpenedPosOperationsAsPendingCompletion();
            BlockLabels(new List<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 00 00 16 18 0B 01 66 15 20 7E EA"
            });
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 1,
                expectedTotalPrice: 5M,
                expectedTotalQuantity: 1
            );
            CompleteStaledPurchase_PosContentIsTaken_ShouldHaveBankTransactionInfoAndCompleteOperation(
                expectedPaymentAmount: 5M
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 1,
                expectedPosOperationCheckItemsAmount: 5M,
                expectedPosOperationCheckItemsBonusPointsAmount: 0M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 5M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 5M,
                expectedFiscalizationAmount: 5M,
                expectedTotalCost: 5M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            CompleteStaledPurchase_PosContentIsTakenAndSomeLabeledGoodIsBlocked_ShouldPayForPurchaseAndCompleteOperationWithCorrectCheck(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            MarkOpenedPosOperationsAsPendingCompletion();
            BlockLabels(new List<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11"
            });
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 1,
                expectedTotalPrice: 10M,
                expectedTotalQuantity: 2
            );
            CompleteStaledPurchase_PosContentIsTaken_ShouldHaveBankTransactionInfoAndCompleteOperation(
                expectedPaymentAmount: 10M
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 2,
                expectedPosOperationCheckItemsAmount: 10M,
                expectedPosOperationCheckItemsBonusPointsAmount: 0M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 10M);

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
            CompleteStaledPurchase_PosContentIsTakenAndLabeledGoodIsBlockedAnd10BonusesAreGiven_ShouldPayForPurchaseAndCompleteOperationWithCorrectCheckAndAdd50BonusesForFirstPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            MarkOpenedPosOperationsAsPendingCompletion();
            AddBonusForUser(DefaultUserId, 5M);
            BlockLabels(new List<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11"
            });
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 1,
                expectedTotalPrice: 10M,
                expectedTotalQuantity: 2
            );

            EnsureLastPosOperationIsPaidByBonusesAndMoney(
                expectedBonusesRemainder: 50M,
                expectedMoneyPaymentSum: 5M
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 2,
                expectedPosOperationCheckItemsAmount: 5M,
                expectedPosOperationCheckItemsBonusPointsAmount: 5M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 5M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 5M,
                expectedMoneyAmount: 5M,
                expectedFiscalizationAmount: 5M,
                expectedTotalCost: 10M,
                expectedTotalDiscount: 0M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CompleteStaledPurchase_PosContentIsTakenAndBlockedLabeledGoodsAreGiven_ShouldCompletePurchaseAndNotPaymentRequired(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            DeleteUsersCardTokens();
            BlockLabels(new List<string>
            {
                "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                "E2 00 00 16 18 0B 01 66 15 20 7E EA",
                "E2 80 11 60 60 00 02 05 2A 98 4B A1"
            });
            MarkOpenedPosOperationsAsPendingCompletion();
            VerifyLabelsSynchronization_TwoOutOfThreeGoodsAreTaken_ShouldAssignTakenGoodsToUserAndNotifyCheckVerificationCompletion(2);

            EnsureLastPosOperationCheckIsEmpty();

            EnsureLastPosOperationIsCompletedAndBankTransactionNotExists();

            EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void UpdateTakenLabelsAndPayForPurchase_PosContentIsTakenOperationWithDiscount_ShouldCreateCorrectCheckForUserAndPayForPurchaseWithDiscount(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            Seeder.Seed(new DiscountsDataSet());
            Seeder.Seed(new PosDiscountsDataSet());
            Seeder.Seed(new DiscountRulesDataSet());
            Seeder.Seed(new DiscountRuleValuesDataSet());

            MarkOpenedPosOperationsAsPendingCompletion();

            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);

            ReceiveCheckWithDiscount_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices
            (
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3,
                expectedDiscount: 4M,
                expectedTotalPriceWithDiscount: 21M
            );

            EnsureLastPosOperationIsPaidByMoney(
                expectedPaymentAmount: 21M
            );

            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 21M,
                expectedPosOperationCheckItemsBonusPointsAmount: 0M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 21M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 21M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 21M,
                expectedFiscalizationAmount: 21M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 4M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void VerifyLabelsSynchronizationAndPayForPurchase_PosContentIsTakenWithDiscountAnd200BonusesAreGiven_ShouldAssignLabeledGoodsToUserAndNotifyCheckVerificationCompletionAndPayForPurchaseAdd50BonusesForFirstPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            Seeder.Seed(new DiscountsDataSet());
            Seeder.Seed(new PosDiscountsDataSet());
            Seeder.Seed(new DiscountRulesDataSet());
            Seeder.Seed(new DiscountRuleValuesDataSet());

            AddBonusForUser(DefaultUserId, 200M);
            MarkOpenedPosOperationsAsPendingCompletion();
            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);

            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3
            );

            ReceiveCheckWithDiscount_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices
            (
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3,
                expectedDiscount: 4M,
                expectedTotalPriceWithDiscount: 21M
            );

            EnsureLastPosOperationIsPaidByBonuses(
                expectedBonusesRemainder: 229M,
                expectedPosOperationsCount: 1
            );

            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidByBonusPoints,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 0M,
                expectedPosOperationCheckItemsBonusPointsAmount: 21M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 21M,
                expectedMoneyAmount: 0M,
                expectedFiscalizationAmount: 0M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 4M);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            CompleteStaledPurchase_PosContentIsTakenWithDiscountAnd5BonusesAreGiven_ShouldPayForPurchaseAndCompleteOperationWithCorrectCheckAndAdd50BonusesForFirstPay(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            Seeder.Seed(new DiscountsDataSet());
            Seeder.Seed(new PosDiscountsDataSet());
            Seeder.Seed(new DiscountRulesDataSet());
            Seeder.Seed(new DiscountRuleValuesDataSet());

            MarkOpenedPosOperationsAsPendingCompletion();
            AddBonusForUser(DefaultUserId, 5M);

            UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(3);
            ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
                expectedGoodsCount: 2,
                expectedTotalPrice: 25M,
                expectedTotalQuantity: 3
            );

            EnsureLastPosOperationIsPaidByBonusesAndMoney(
                expectedBonusesRemainder: 50M,
                expectedMoneyPaymentSum: 16M
            );
            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 16M,
                expectedPosOperationCheckItemsBonusPointsAmount: 5M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 16M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 16M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 5M,
                expectedMoneyAmount: 16M,
                expectedFiscalizationAmount: 16M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 4M);
        }

        [TestCase(false)]
        [TestCase(true)]
        [NonParallelizable]
        public void CompleteUserPurchase_IncompleteUserPurchaseIsGiven_ShouldBePaidOnlyOnceIfRunningInSingleThread(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            CompletePosOperationInParallel(threadsCount: 1);

            EnsureCreatedSingleBankTransactionInfoAndSingleRowInCheckItems();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CompleteUserPurchase_IncompleteUserPurchaseIsGiven_ShouldBePaidOnlyOnceIfRunningInMultipleThreads(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            CompletePosOperationInParallel(threadsCount: 7);

            EnsureCreatedSingleBankTransactionInfoAndSingleRowInCheckItems();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CompleteUserPurchase_IncompletePurchaseWithCheckItemsIsGiven_ShouldAddPosOperationTransactionCheckItems(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var labeledGoods = Context.LabeledGoods.ToList();
            labeledGoods.ForEach(lg => lg.MarkAsUsedInPosOperation(DefaultPosOperationId));
            Context.SaveChanges();

            _operationsManager.CloseLatestPosOperationAsync(_unitOfWork, DefaultPosId).GetAwaiter().GetResult();

            var result = _purchasesController.CompleteUserPurchaseAsync().GetAwaiter().GetResult();

            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var completionResultDto = objectResult.Value as PurchaseCompletionResultDto;

            completionResultDto.Status.Should().Be(PurchaseCompletionStatus.Success);

            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 3,
                expectedPosOperationCheckItemsAmount: 25M,
                expectedPosOperationCheckItemsBonusPointsAmount: 0M);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 25M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 25M);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: 0M,
                expectedMoneyAmount: 25M,
                expectedFiscalizationAmount: 25M,
                expectedTotalCost: 25M,
                expectedTotalDiscount: 0M);
        }

        //FiscalizationScenario Case 1
        [TestCase(75, 0, 0, 0, 15, 0, 0, true)]
        [TestCase(60, 0, 0, 0, 0, 0, 0, true)]
        [TestCase(75, 0, 0, 0, 15, 0, 0, false)]
        [TestCase(60, 0, 0, 0, 0, 0, 0, false)]
        //FiscalizationScenario Case 2
        [TestCase(75, 0, 0, 0, 20, 5, 5, true)]
        [TestCase(55, 0, 0, 0, 0, 5, 5, true)]
        [TestCase(75, 0, 0, 0, 20, 5, 5, false)]
        [TestCase(55, 0, 0, 0, 0, 5, 5, false)]
        //FiscalizationScenario Case 3
        [TestCase(0, 1, 1, 60, 0, 0, 0, true)]
        [TestCase(0, 1, 1, 60, 0, 0, 0, false)]
        //FiscalizationScenario Case 4
        [TestCase(0, 1, 1, 55, 0, 5, 5, true)]
        [TestCase(0, 1, 1, 53, 0, 10, 7, true)]
        [TestCase(0, 1, 1, 55, 0, 5, 5, false)]
        [TestCase(0, 1, 1, 53, 0, 10, 7, false)]
        //FiscalizationScenario Case 5
        [TestCase(25, 1, 1, 35, 0, 0, 0, true)]
        [TestCase(5, 1, 1, 55, 0, 0, 0, true)]
        [TestCase(55, 1, 1, 5, 0, 0, 0, true)]
        [TestCase(25, 1, 1, 35, 0, 0, 0, false)]
        [TestCase(5, 1, 1, 55, 0, 0, 0, false)]
        [TestCase(55, 1, 1, 5, 0, 0, 0, false)]
        //FiscalizationScenario Case 6
        [TestCase(25, 1, 1, 30, 0, 5, 5, true)]
        [TestCase(40, 1, 1, 15, 0, 5, 5, true)]
        [TestCase(5, 1, 1, 50, 0, 5, 5, true)]
        [TestCase(25, 1, 1, 30, 0, 5, 5, false)]
        [TestCase(40, 1, 1, 15, 0, 5, 5, false)]
        [TestCase(5, 1, 1, 50, 0, 5, 5, false)]
        public void CompleteUserPurchase_IncompleteUserPurchaseWithSeveralCheckItemsAndUserBonusPointsAreGiven_ShouldReturnCorrectResultAndFiscalizationSumShouldBeTheSameAsBankTransactionInfosSumAndFiscalizationSumShouldBeWithoutDiscountsAndBonuses
            (
                decimal bonuses,
                int expectedBankTransactionsCount,
                int expectedFiscalizationInfosCount,
                decimal expectedBankTransactionsAndFiscalizationAmountSum,
                decimal expectedUserBonusPointsAfterPurchase,
                decimal discountInPercentage,
                decimal expectedTotalRoundedDiscountAmount,
                bool useNewPaymentSystem
            )
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            AddBonusForUser(DefaultUserId, bonuses);

            var good = new Good(
                id: 0,
                name: "Помидоры",
                description: "Описание помидор",
                goodParameters: new GoodParameters(1, 0)
            );
            Seeder.Seed(new[] { good });

            CompletePosOperationWithSeveralCheckItemsAndResultShouldBeSuccess(discountInPercentage);

            var bankTransactionInfos = Context.BankTransactionInfos
                .AsNoTracking()
                .Where(bti => bti.PosOperationId == DefaultPosOperationId)
                .ToImmutableList();
            bankTransactionInfos.Count
                .Should()
                .Be(expectedBankTransactionsCount);
            bankTransactionInfos
                .Sum(x => x.Amount)
                .Should()
                .Be(expectedBankTransactionsAndFiscalizationAmountSum);

            Context.Users
                .AsNoTracking()
                .Single(x => x.Id == DefaultUserId).TotalBonusPoints
                .Should()
                .Be(expectedUserBonusPointsAfterPurchase + DefaultBonusAmountForFirstPay);


            var fiscalizaitonInfos = Context.FiscalizationInfos
                .AsNoTracking()
                .Where(fi => fi.PosOperationId == DefaultPosOperationId)
                .ToImmutableList();
            fiscalizaitonInfos.Count
                .Should()
                .Be(expectedFiscalizationInfosCount);
            fiscalizaitonInfos
                .Sum(x => x.TotalFiscalizationAmount)
                .Should()
                .Be(expectedBankTransactionsAndFiscalizationAmountSum);

            Context.CheckItems
                .AsNoTracking()
                .Sum(x => x.RoundedDiscountAmount)
                .Should()
                .Be(expectedTotalRoundedDiscountAmount);

            var isPurchasePaidByBonusPoints = expectedBankTransactionsAndFiscalizationAmountSum == 0;
            var expectedBonusPointsAmount = bonuses - expectedUserBonusPointsAfterPurchase;
            var expectedTotalCost = expectedBonusPointsAmount + expectedBankTransactionsAndFiscalizationAmountSum + expectedTotalRoundedDiscountAmount;

            EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
                expectedTransactionStatus: isPurchasePaidByBonusPoints ? PosOperationTransactionStatus.PaidByBonusPoints : PosOperationTransactionStatus.PaidFiscalized,
                expectedTransactionType: PosOperationTransactionType.RegularPurchase,
                expectedPosOperationCheckItemsCount: 4,
                expectedPosOperationCheckItemsAmount: expectedBankTransactionsAndFiscalizationAmountSum,
                expectedPosOperationCheckItemsBonusPointsAmount: expectedBonusPointsAmount);

            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                transactionStatus: isPurchasePaidByBonusPoints ? PosOperationTransactionStatus.PaidByBonusPoints : PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: expectedBankTransactionsCount,
                expectedBankTransactionsAmount: expectedBankTransactionsAndFiscalizationAmountSum,
                expectedFiscalizationCount: expectedFiscalizationInfosCount,
                expectedFiscalizationAmount: expectedBankTransactionsAndFiscalizationAmountSum);

            EnsurePosOperationTransactionHasCorrectTotalAmounts(
                expectedBonusAmount: expectedBonusPointsAmount,
                expectedMoneyAmount: expectedBankTransactionsAndFiscalizationAmountSum,
                expectedFiscalizationAmount: expectedBankTransactionsAndFiscalizationAmountSum,
                expectedTotalCost: expectedTotalCost,
                expectedTotalDiscount: expectedTotalRoundedDiscountAmount);
        }

        private void CompletePosOperationWithSeveralCheckItemsAndResultShouldBeSuccess(decimal discountInPercentage)
        {
            var checkItems = new List<CheckItem>
            {
                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultPosOperationId,
                        DefaultGoodId,
                        DefaultLabeledGoodId,
                        DefaultCurrencyId)
                .SetPrice(10M)
                .SetStatus(CheckItemStatus.Unpaid)
                .Build(),

                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultPosOperationId,
                        DefaultGoodId,
                        DefaultLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(10M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build(),

                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultPosOperationId,
                        2,
                        DefaultLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(15M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build(),

                CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultPosOperationId,
                        3,
                        DefaultLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(25M)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build()
             };

            checkItems.ForEach(ci => ci.AddDiscount(discountInPercentage));

            Context.CheckItems.AddRange(checkItems);

            Context.SaveChanges();

            var posOperation = Context.PosOperations.FirstOrDefault(op => op.Id == DefaultPosOperationId);
            posOperation?.MarkAsPendingCompletion();
            posOperation?.MarkAsPendingCheckCreation();
            posOperation?.MarkAsCompletedAndRememberDate();
            posOperation?.WriteOffBonusPoints();

            Context.SaveChanges();

            _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, posOperation).GetAwaiter().GetResult();

            var result = _purchasesController.CompleteUserPurchaseAsync().GetAwaiter().GetResult();

            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var completionResultDto = objectResult.Value as PurchaseCompletionResultDto;

            completionResultDto.Status.Should().Be(PurchaseCompletionStatus.Success);
        }

        private void CompletePosOperationInParallel(int threadsCount)
        {
            var posOperation = Context.PosOperations.FirstOrDefault(op => op.Id == DefaultPosOperationId);
            posOperation?.MarkAsPendingCompletion();
            posOperation?.MarkAsPendingCheckCreation();
            posOperation?.MarkAsCompletedAndRememberDate();

            var checkItem = CheckItem.NewBuilder(
                        DefaultPosId,
                        DefaultPosOperationId,
                        DefaultGoodId,
                        DefaultLabeledGoodId,
                        DefaultCurrencyId)
                    .SetPrice(DefaultPrice)
                    .SetStatus(CheckItemStatus.Unpaid)
                    .Build();

            Context.CheckItems.Add(checkItem);

            Context.SaveChanges();

            _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, posOperation).GetAwaiter().GetResult();

            var tasks = new Collection<Task<IActionResult>>();
            for (var i = 0; i < threadsCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var controller = ServiceProvider.GetRequiredService<PurchasesController>();
                    controller.ControllerContext = ControllerContext;
                    return controller.CompleteUserPurchaseAsync().Result;
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private void EnsureCreatedSingleBankTransactionInfoAndSingleRowInCheckItems()
        {
            var posOperation = Context.PosOperations.FirstOrDefault(op => op.Id == DefaultPosOperationId);

            var checkItems = Context.CheckItems.Where(ch => ch.PosId == DefaultPosOperationId).ToList();
            var bankTransactionInfos =
                Context.BankTransactionInfos.Where(bti => bti.PosOperationId == posOperation.Id).ToList();

            bankTransactionInfos.Should().HaveCount(1);
            checkItems.Should().HaveCount(1);
        }

        private void DeleteUsersCardTokens()
        {
            using (var context = ProvideNewContext())
            {
                var paymentsSystems = context.PaymentCards.ToList();
                paymentsSystems.ForEach(ps => ps.ResetCardToken());
                context.SaveChanges();
            }
        }

        private void BlockLabels(IEnumerable<string> labels)
        {
            using (var context = ProvideNewContext())
            {
                var labeledGoods = context.LabeledGoods.Where(lg => labels.Contains(lg.Label)).ToList();
                labeledGoods.ForEach(lg => lg.Disable());
                context.SaveChanges();
            }
        }

        private void UpdateTakenLabelsAndPayForPurchase_PosContentIsTaken_ShouldUpdateTakenLabelsAndPayForPurchase(int expectedCount)
        {
            _wsAccountingBalancesController.Synchronize(new PosAccountingBalancesDto
            {
                Labels = new Collection<string>(),
                PosId = DefaultPosId
            }).Wait();
            Context.LabeledGoods.Count(lg => lg.PosOperationId != null).Should().Be(expectedCount);
        }

        private void
            VerifyLabelsSynchronization_TwoOutOfThreeGoodsAreTaken_ShouldAssignTakenGoodsToUserAndNotifyCheckVerificationCompletion(int expectedCount)
        {
            _wsAccountingBalancesController.Synchronize(new PosAccountingBalancesDto
            {
                Labels = new Collection<string> { "E2 80 11 60 60 00 02 05 2A 98 AB 11" },
                PosId = DefaultPosId
            }).Wait();
            Context.LabeledGoods.Count(lg => lg.PosOperationId != null).Should().Be(expectedCount);
        }

        private void VerifyLabelsSynchronization_NoGoodIsTaken_ShouldNotifyCheckVerificationCompletion()
        {
            _wsAccountingBalancesController.Synchronize(new PosAccountingBalancesDto
            {
                Labels = new Collection<string>
                {
                    "E2 80 11 60 60 00 02 05 2A 98 AB 11",
                    "E2 00 00 16 18 0B 01 66 15 20 7E EA",
                    "E2 80 11 60 60 00 02 05 2A 98 4B A1"
                },
                PosId = DefaultPosId
            }).Wait();
            Context.LabeledGoods.Count(lg => lg.PosOperationId != null).Should().Be(0);
        }

        private void ReceiveCheck_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices(
            int expectedGoodsCount, decimal expectedTotalPrice, int expectedTotalQuantity)
        {
            var result = _purchasesController.GetUserPurchasesHistoryAsync().Result;

            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var purchasesHistory = objectResult.Value as PurchasesHistoryDto;

            purchasesHistory.Checks.Should().NotBeEmpty();

            var lastCheck = purchasesHistory.Checks.First();

            lastCheck.Should().NotBeNull();

            lastCheck.Goods.Should().HaveCount(expectedGoodsCount);
            lastCheck.PriceInfo.PriceWithoutDiscount.Should().Be(expectedTotalPrice);
            lastCheck.IsZero.Should().Be(expectedTotalPrice == 0M);
            lastCheck.PriceInfo.Quantity.Should().Be(expectedTotalQuantity);

            Context.CheckItems.ToImmutableList().Should().NotBeEmpty();
        }

        private void ReceiveCheckWithDiscount_PosContentIsTaken_ShouldHaveCorrectNumberOfGoodsAndPrices
        (
            int expectedGoodsCount,
            decimal expectedTotalPrice,
            int expectedTotalQuantity,
            decimal expectedDiscount,
            decimal expectedTotalPriceWithDiscount
        )
        {
            var result = _purchasesController.GetUserPurchasesHistoryAsync().Result;
            result.Should().BeOfType<OkObjectResult>();
            var objectResult = result as OkObjectResult;
            var purchasesHistory = objectResult.Value as PurchasesHistoryDto;
            purchasesHistory.Checks.Should().NotBeEmpty();

            var lastCheck = purchasesHistory.Checks.First();
            lastCheck.Should().NotBeNull();

            lastCheck.Goods.Should().HaveCount(expectedGoodsCount);
            lastCheck.PriceInfo.PriceWithoutDiscount.Should().Be(expectedTotalPrice);
            lastCheck.PriceInfo.Quantity.Should().Be(expectedTotalQuantity);
            lastCheck.PriceInfo.TotalDiscount.Should().Be(expectedDiscount);
            lastCheck.PriceInfo.TotalPriceWithDiscount.Should().Be(expectedTotalPriceWithDiscount);
            lastCheck.PriceInfo.TotalPrice.Should().Be(expectedTotalPriceWithDiscount);

            Context.CheckItems.ToImmutableList().Should().NotBeEmpty();
        }


        private void EnsureLastPosOperationIsPaidByMoney(decimal expectedPaymentAmount)
        {
            Context.BankTransactionInfos.Should().HaveCount(1);
            Context.PosOperations.Where(o => o.DatePaid != null).Should().HaveCount(1);

            Context.BankTransactionInfos.First().Amount.Should().Be(expectedPaymentAmount);
        }

        private void EnsureLastPosOperationIsCompletedAndBankTransactionNotExists()
        {
            Context.BankTransactionInfos.Should().BeEmpty();
            Context.PosOperations
                .Where(o => o.DateCompleted != null && o.Status == PosOperationStatus.Completed)
                .Should()
                .HaveCount(1);
        }

        private void EnsureLastPosOperationIsPaidByBonuses(decimal expectedBonusesRemainder, int expectedPosOperationsCount)
        {
            Context.BankTransactionInfos
                .AsNoTracking()
                .Should()
                .BeEmpty();

            Context.PosOperations
                .AsNoTracking()
                .Where(o => o.DatePaid != null)
                .Should()
                .HaveCount(expectedPosOperationsCount);

            Context.Users
                .AsNoTracking()
                .SingleOrDefault(ub => ub.Id == Context.PosOperations.FirstOrDefault(o => o.DatePaid != null).UserId)
                .TotalBonusPoints
                .Should()
                .Be(expectedBonusesRemainder);
        }

        private void
            EnsureLastPosOperationIsPaidByBonusesAndMoney(
                decimal expectedBonusesRemainder, decimal expectedMoneyPaymentSum)
        {
            Context.BankTransactionInfos.Should().HaveCount(1);
            Context.PosOperations.Where(o => o.DatePaid != null).Should().HaveCount(1);

            Context.Users
                .AsNoTracking()
                .SingleOrDefault(ub => ub.Id == Context.PosOperations.SingleOrDefault(o => o.DatePaid != null).UserId).TotalBonusPoints
                .Should()
                .Be(expectedBonusesRemainder);
            Context.BankTransactionInfos.First().Amount.Should().Be(expectedMoneyPaymentSum);
        }
        private void EnsurePosOperationTransactionHasCorrectStatusAndTypeAndCorrectBankTransactionsAndFiscalizationAndPosOperationCheckItemsCountAndAmount(
            PosOperationTransactionStatus expectedTransactionStatus,
            PosOperationTransactionType expectedTransactionType,
            int expectedPosOperationCheckItemsCount,
            decimal expectedPosOperationCheckItemsAmount,
            decimal expectedPosOperationCheckItemsBonusPointsAmount,
            int expectedPosOperationTransactionsCount = 1)
        {
            EnsurePosOperationTransactionsHaveCount(expectedPosOperationTransactionsCount);
            EnsurePosOperationTransactionHasStatusAndType(expectedTransactionStatus, expectedTransactionType);
            EnsurePosOperationTransactionCheckItemsHaveCountAndMoneyAndBonusAmount(
                expectedPosOperationCheckItemsCount,
                expectedPosOperationCheckItemsAmount,
                expectedPosOperationCheckItemsBonusPointsAmount);
        }

        private void EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
            PosOperationTransactionStatus transactionStatus,
            int expectedBankTransactionsCount,
            decimal expectedBankTransactionsAmount,
            int expectedFiscalizationCount,
            decimal expectedFiscalizationAmount)
        {
            var fiscalizaionAmount = transactionStatus == PosOperationTransactionStatus.Unpaid
                ? 0
                : expectedFiscalizationAmount;
            var bankTransactionAmount = transactionStatus == PosOperationTransactionStatus.Unpaid
                ? 0
                : expectedBankTransactionsAmount;

            EnsureBankTransactionInfosVersionTwoHaveCountAndAmount(expectedBankTransactionsCount, bankTransactionAmount);
            EnsureFiscalizationInfosVersionTwoHaveCountAndAmount(expectedFiscalizationCount, fiscalizaionAmount);
        }

        private void EnsurePosOperationTransactionHasCorrectTotalAmounts(
            decimal expectedBonusAmount,
            decimal expectedMoneyAmount,
            decimal expectedFiscalizationAmount,
            decimal expectedTotalCost,
            decimal expectedTotalDiscount)
        {
            var posOperationTransactions = Context.PosOperationTransactions.ToImmutableList();
            posOperationTransactions.Sum(pot => pot.BonusAmount).Should().Be(expectedBonusAmount);
            posOperationTransactions.Sum(pot => pot.MoneyAmount).Should().Be(expectedMoneyAmount);
            posOperationTransactions.Sum(pot => pot.FiscalizationAmount).Should().Be(expectedFiscalizationAmount);
            posOperationTransactions.Sum(pot => pot.TotalCost).Should().Be(expectedTotalCost);
            posOperationTransactions.Sum(pot => pot.TotalDiscountAmount).Should().Be(expectedTotalDiscount);
        }

        private void EnsurePosOperationTransactionCheckItemsHaveCountAndMoneyAndBonusAmount(int expectedCount, decimal expectedAmount, decimal expectedBonusPointsAmount)
        {
            Context.PosOperationTransactionCheckItems.Count().Should().Be(expectedCount);
            Context.PosOperationTransactionCheckItems.Sum(cki => cki.Amount).Should().Be(expectedAmount);
            Context.PosOperationTransactionCheckItems.Sum(cki => cki.CostInBonusPoints).Should().Be(expectedBonusPointsAmount);
        }

        private void EnsurePosOperationTransactionsHaveCount(int expectedCount)
        {
            Context.PosOperationTransactions.Count().Should().Be(expectedCount);
        }

        private void EnsureBankTransactionInfosVersionTwoHaveCountAndAmount(int expectedCount, decimal expectedAmount)
        {
            Context.BankTransactionInfosVersionTwo.Count().Should().Be(expectedCount);
            Context.BankTransactionInfosVersionTwo.Sum(bti => bti.Amount).Should().Be(expectedAmount);
        }

        private void EnsureFiscalizationInfosVersionTwoHaveCountAndAmount(int expectedCount, decimal expectedAmount)
        {
            Context.FiscalizationInfosVersionTwo.Count().Should().Be(expectedCount);
            Context.FiscalizationInfosVersionTwo.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(expectedAmount);
        }

        private void EnsurePosOperationTransactionHasStatusAndType(
            PosOperationTransactionStatus expectedTransactionStatus,
            PosOperationTransactionType expectedTransactionType)
        {
            var posOperationTransactions = Context.PosOperationTransactions.ToImmutableList();
            posOperationTransactions.ForEach(pot => pot.Status.Should().Be(expectedTransactionStatus));
            posOperationTransactions.ForEach(pot => pot.Type.Should().Be(expectedTransactionType));
        }

        private void EnsurePosOperationTransactionAndBankTransactionInfoAndFiscalizationInfoAndPosOperationCheckItemsAreNotExists()
        {
            Context.PosOperationTransactions.Should().BeEmpty();
            Context.BankTransactionInfosVersionTwo.Should().BeEmpty();
            Context.FiscalizationInfosVersionTwo.Should().BeEmpty();
            Context.PosOperationTransactionCheckItems.Should().BeEmpty();
        }

        private void
            CompleteStaledPurchase_PosContentIsTaken_ShouldHaveBankTransactionInfoAndCompleteOperation(
                decimal expectedPaymentAmount)
        {
            _unpaidPurchaseFinisher.FinishUnpaidPurchasesAsync(TimeSpan.FromMinutes(30)).Wait();

            Context.BankTransactionInfos
                .Should()
                .HaveCount(1);
            Context.PosOperations
                .Where(o => o.DatePaid != null)
                .Should()
                .HaveCount(1);

            Context.BankTransactionInfos.First().Amount.Should().Be(expectedPaymentAmount);
        }

        private void CompleteStaledPurchase_BonusThatCoversCostIsGiven_ShouldCompleteOperationAndNotCreateBankTransaction(
            decimal expectedBonusesRemainder)
        {
            _unpaidPurchaseFinisher.FinishUnpaidPurchasesAsync(TimeSpan.FromMinutes(30)).Wait();

            Context.BankTransactionInfos.Should().BeEmpty();
            Context.PosOperations
                .AsNoTracking()
                .Where(o => o.DatePaid != null)
                .Should()
                .HaveCount(1);

            Context.Users
                .AsNoTracking()
                .SingleOrDefault(ub => ub.Id == Context.PosOperations.SingleOrDefault(o => o.DatePaid != null).UserId).TotalBonusPoints
                .Should()
                .Be(expectedBonusesRemainder);
        }

        private void
            CompleteStaledPurchase_PosContentIsTakenAndIncorrectCardTokenIsGiven_ShouldNotCompleteOperationAndCreateBankTransaction()
        {
            _unpaidPurchaseFinisher.FinishUnpaidPurchasesAsync(TimeSpan.FromMinutes(30)).Wait();

            Context.BankTransactionInfos.Should().BeEmpty();
            Context.PosOperations.Where(o => o.DatePaid != null).Should().BeEmpty();
        }

        private void EnsureLastPosOperationCheckIsEmpty()
        {
            var result = _purchasesController.GetUserPurchasesHistoryAsync().Result;

            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var purchasesHistory = objectResult.Value as PurchasesHistoryDto;
            purchasesHistory.Should().NotBeNull();
            purchasesHistory.Checks.Should().BeEmpty();

            Context.CheckItems
                .Where(ci => ci.Status == CheckItemStatus.Paid || ci.Status == CheckItemStatus.Unpaid)
                .ToImmutableList()
                .Should()
                .BeEmpty();
        }

        private void AddBonusForUser(int userId, decimal bonus)
        {
            var user = Context.Users.Single(u => u.Id == userId);
            user.AddBonusPoints(bonus, BonusType.Refund);
            Context.SaveChanges();
        }

        private void MarkOpenedPosOperationsAsPendingCompletion()
        {
            using (var context = ProvideNewContext())
            {
                var openedPosOperations = context.PosOperations
                    .Where(po => po.Status == PosOperationStatus.Opened)
                    .ToImmutableList();
                var pendingCompletionPosOperations = openedPosOperations.Select(po =>
                {
                    po.MarkAsPendingCompletion();
                    return po;
                }).ToImmutableList();
                context.UpdateRange(pendingCompletionPosOperations);
                context.SaveChanges();
            }
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

        private WsCommandsQueueProcessor CreateWsCommandsQueueProcessorAndSubscribeOnEvent(EventWaitHandle queueIsEmptyWaitHandler)
        {
            var processor = new WsCommandsQueueProcessor(
                10,
                TimeSpan.FromMilliseconds(7000),
                ServiceProvider.GetRequiredService<ILogger>());

            processor.OnQueueProcessed += (sender, eventArgs) =>
            {
                queueIsEmptyWaitHandler.Set();
            };

            return processor;
        }

        private static WsMessage CreateWsMessage(Collection<string> labelsInPos, int posId)
        {
            var wsControllerRoute = new WsControllerRoute("accountingBalances", "synchronize");
            var body = new JObject(
                new JProperty("posId", posId),
                new JProperty("labels", new JArray(labelsInPos.Select(lbl => new JValue(lbl)))),
                new JProperty("commandId", Guid.NewGuid()));
            var wsMessage = new WsMessage(wsControllerRoute, body);
            return wsMessage;
        }

        private static WebSocket CreateWebSocket()
        {
            return WebSocket.CreateFromStream(new MemoryStream(), true, null, TimeSpan.FromMinutes(1));
        }
    }
}