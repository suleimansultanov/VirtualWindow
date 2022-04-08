using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.CheckOnlineManager.Models;
using NasladdinPlace.Api.Tests.Utils.CheckOnline;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers;
using NasladdinPlace.CheckOnline.Infrastructure;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;
using NasladdinPlace.CheckOnline.Infrastructure.Models;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Services.CheckOnline;
using NasladdinPlace.Core.Services.CheckOnline.Helpers;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models;
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
using System.Threading.Tasks;
using Currency = NasladdinPlace.Payment.Models.Currency;

namespace NasladdinPlace.Api.Tests.Scenarios.CheckOnlineManager
{
    public class CheckOnlineManagerShould : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultFiscalizationInfoId = 1;
        private const int DefaultPosOperationId = 1;
        private const int SecondPosOperationId = 2;
        private const int DefaultUserId = 1;
        private const int DefaultCurrencyId = 1;
        private const int DefaultThreadsCount = 4;

        private const string FirstLabeledGood = "E2 80 11 60 60 00 02 05 2A 98 4B A1";

        private const string DefaultCryptogram = "015200828210221102gCBD/gnsVGyBszpkn89/LpE0WUOGEpALM3143Fn2Ud4htLRlDJiig/UOImIO3mPMbh6wk/sN/DwwKrEXMBDllkznC9SpcGLqCB8zjyU65Q6e2bH7S65Qb3h3snqJwitLmQkWqL8Dy9XGQQEGiNaNzomtjeilwjb+9QQXuPQzFHiAwL7SGRg9ZaV3+7bnXtsnx2sQFsRJDAC4RCBZ0JLKV9No/uCllKgQc8j9gP9q4kUX1lOBhkBU6jCXdb+b3CqoMmD1Y9AUDIGx+ICcdnaOT+Oif3pYZZmuMHllHIwBeBQqod87I8SN9xvB2WJOJP/Rmt4FwDw2oJokbQl6Trbk7g==";

        private const string CardHolder = "TEST USER";

        private const string UserIpAddress = "192.168.1.122";
        private const string UserIdentifier = "user_1";
        private const string PaymentTestDescription = "test";

        private ICheckOnlineManager _checkOnlineManager;
        private IPaymentService _paymentService;
        private IUnitOfWork _unitOfWork;
        private IOperationTransactionManager _operationTransactionManager;
        private IPosOperationTransactionCreationUpdatingService _transactionCreationUpdatingService;
        private IServiceProvider _serviceProvider;

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

            var serviceCollection = TestServiceProviderFactory.CreateServiceCollection();

            TestServiceProviderFactory.ExchangeService<ICheckOnlineRequestProvider>(serviceCollection,
                provider => TestCheckOnlineRequestProviderFactory.Create(provider, false));

            _serviceProvider = serviceCollection.BuildServiceProvider();

            _paymentService = _serviceProvider.GetRequiredService<IPaymentService>();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _checkOnlineManager = _serviceProvider.GetRequiredService<ICheckOnlineManager>();
            _operationTransactionManager = _serviceProvider.GetRequiredService<IOperationTransactionManager>();
            _transactionCreationUpdatingService = _serviceProvider.GetRequiredService<IPosOperationTransactionCreationUpdatingService>();
        }

        [TestCase(15, 5, true, 1, 15, true)]
        [TestCase(15, 5, true, 1, 15, false)]
        [TestCase(10, 10, false, 1, 10, true)]
        [TestCase(10, 10, false, 1, 10, false)]
        public void
            MakeFiscalizationWithPaidPosOperationAndFiscalizationInfoAndReturnCorrectFiscalizationInfoCountAndCorrectFiscalizationAmountSum(
                decimal initialMoney,
                decimal initialBonus,
                bool addFiscalizationInfo,
                int expectedFiscalizationInfosCount,
                decimal expectedTotalFiscalizationAmountSum,
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var fiscalizationParameters = CreatePosOperationAsPaidAndAddFiscalizationInfos(initialMoney, initialBonus,
                additionalInitializationFunc: posOperationBuilder =>
                {
                    posOperationBuilder.SetCheckItems(new List<CheckItem>
                    {
                        CreateCheckItem(DefaultPosOperationId), CreateCheckItem(DefaultPosOperationId)
                    });
                },
                addFiscalizationInfo: addFiscalizationInfo);

            _checkOnlineManager.MakeFiscalizationAsync(_unitOfWork, fiscalizationParameters.PosOperation, fiscalizationParameters.PosOperationTransaction).GetAwaiter().GetResult();

            AssertExpectedFiscalizationInfosCount(expectedFiscalizationInfosCount);
            AssertExpectedFiscalizationInfosStateAndTypeAndTotalFiscalizationAmountSum(
                DefaultFiscalizationInfoId,
                FiscalizationState.Success,
                FiscalizationType.Income,
                expectedTotalFiscalizationAmountSum);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MakeFiscalizationWithPaidPosOperationAndFiscalizationInfoAndReturnCorrectFiscalizationInfoCountAndCorrectFiscalizationAmountSumInMultiplyThreads(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var fiscalizationParameters = CreatePosOperationAsPaidAndAddFiscalizationInfos(
                initialMoney: 15M,
                initialBonus: 5M,
                additionalInitializationFunc: posOperationBuilder =>
            {
                posOperationBuilder.SetCheckItems(new List<CheckItem>
                {
                    CreateCheckItem(DefaultPosOperationId), CreateCheckItem(DefaultPosOperationId)
                });
            });

            var tasks = new Collection<Task>();
            for (var i = 0; i < DefaultThreadsCount; i++)
            {
                var checkOnlineManager = _serviceProvider.GetRequiredService<ICheckOnlineManager>();
                var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();

                tasks.Add(Task.Factory.StartNew(() => checkOnlineManager.MakeFiscalizationAsync(unitOfWork, fiscalizationParameters.PosOperation, fiscalizationParameters.PosOperationTransaction).Wait()));
                tasks.Add(Task.Factory.StartNew(() => checkOnlineManager.MakeReFiscalizationAsync(new List<int> { DefaultFiscalizationInfoId }).Wait()));
            }

            Task.WaitAll(tasks.ToArray());

            AssertExpectedFiscalizationInfosCount(expectedFiscalizationInfosCount: 1);
            AssertExpectedFiscalizationInfosStateAndTypeAndTotalFiscalizationAmountSum(
                DefaultFiscalizationInfoId,
                FiscalizationState.Success,
                FiscalizationType.Income,
                expectedTotalFiscalizationAmountSum: 15M);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MakeFiscalizationWithEmptyPosOperationAndReturnNoRecordsInFiscalizationInfos(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var fiscalizationParameters = CreatePosOperationAsPaidAndAddFiscalizationInfos(
                initialMoney: 0M,
                initialBonus: 0M,
                additionalInitializationFunc: null,
                addFiscalizationInfo: false);

            _checkOnlineManager.MakeFiscalizationAsync(_unitOfWork, fiscalizationParameters.PosOperation, fiscalizationParameters.PosOperationTransaction).Wait();

            AssertExpectedFiscalizationInfosCount(expectedFiscalizationInfosCount: 0);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MakeFiscalizationWhenFiscalizationInProcessAndReturnFiscalizationInfoStateInProcess(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var fiscalizationParameters = CreatePosOperationAsPaidAndAddFiscalizationInfos(
                initialMoney: 15M,
                initialBonus: 5M,
                additionalInitializationFunc: posOperationBuilder =>
                {
                    posOperationBuilder.SetCheckItems(new List<CheckItem>
                    {
                        CreateCheckItem(DefaultPosOperationId), CreateCheckItem(DefaultPosOperationId)
                    });
                },
                addFiscalizationInfo: true);

            var fiscalizationInfo = Context.FiscalizationInfos.Single(fi => fi.PosOperationId == DefaultPosOperationId);
            fiscalizationInfo.MarkAsInProcess();
            Context.SaveChanges();

            _checkOnlineManager.MakeFiscalizationAsync(_unitOfWork, fiscalizationParameters.PosOperation, fiscalizationParameters.PosOperationTransaction).Wait();

            AssertExpectedFiscalizationInfosCount(expectedFiscalizationInfosCount: 1);

            AssertExpectedFiscalizationInfosStateAndTypeAndTotalFiscalizationAmountSum(
                DefaultFiscalizationInfoId,
                FiscalizationState.InProcess,
                FiscalizationType.Income);
        }

        [TestCase(15, 5, true, 2, 30, true)]
        [TestCase(15, 5, true, 2, 30, false)]
        public void
            MakeFiscalizationWithPaidPosOperationsAndTwoFiscalizationInfosAreGivenAndOneFiscalizationInfoInPendingAndTneOtherInPendingErrorStatusAndReturnCorrectFiscalizationInfoCountAndCorrectFiscalizationAmountSum(
                decimal initialMoney,
                decimal initialBonus,
                bool addFiscalizationInfo,
                int expectedFiscalizationInfosCount,
                decimal expectedTotalFiscalizationAmountSum,
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            CreatePosOperationAsPaidAndAddFiscalizationInfos(initialMoney, initialBonus,
                additionalInitializationFunc: posOperationBuilder =>
                {
                    posOperationBuilder.SetCheckItems(new List<CheckItem>
                    {
                        CreateCheckItem(DefaultPosOperationId),
                        CreateCheckItem(DefaultPosOperationId)
                    });
                },
                addFiscalizationInfo: true);

            CreatePosOperationAsPaidAndAddFiscalizationInfos(initialMoney, initialBonus,
                additionalInitializationFunc: posOperationBuilder =>
                {
                    posOperationBuilder.SetCheckItems(new List<CheckItem>
                    {
                        CreateCheckItem(SecondPosOperationId),
                        CreateCheckItem(SecondPosOperationId)
                    });
                },
                addFiscalizationInfo: true,
                posOperationId: SecondPosOperationId);

            var fiscalizationInfo = Context.FiscalizationInfos.Single(fi => fi.PosOperationId == SecondPosOperationId);
            fiscalizationInfo.MarkAsPendingError("Some error");
            Context.SaveChanges();

            _checkOnlineManager.MakeReFiscalizationAsync(Context.FiscalizationInfos.Select(fi => fi.Id).ToList()).Wait();

            AssertExpectedFiscalizationInfosCount(expectedFiscalizationInfosCount);
            var fiscalizationInfos = Context.FiscalizationInfos.AsNoTracking();
            fiscalizationInfos.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(expectedTotalFiscalizationAmountSum);
            fiscalizationInfos.Count(fi => fi.Type == FiscalizationType.Income).Should().Be(expectedFiscalizationInfosCount);
            fiscalizationInfos.Count(fi => fi.State == FiscalizationState.Success).Should().Be(expectedFiscalizationInfosCount);

        }

        [TestCase(15, 5, true, 2, 30, 4, true)]
        [TestCase(15, 5, true, 2, 30, 4, false)]
        public void
            MakeFiscalizationWithPaidPosOperationsWithPosOperationTransactionsAndTwoFiscalizationInfosAreGivenAndOneFiscalizationInfoInPendingAndTneOtherInPendingErrorStatusAndBrokenFiscalizationServiceAndReturnCorrectFiscalizationInfoCountAndCorrectFiscalizationAmountSum(
                decimal initialMoney,
                decimal initialBonus,
                bool addFiscalizationInfo,
                int expectedFiscalizationInfosCount,
                decimal expectedTotalFiscalizationAmountSum,
                int expectedErrorFiscalizationInfosVersionTwo,
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            CreatePosOperationAsPaidAndAddFiscalizationInfos(initialMoney, initialBonus,
                additionalInitializationFunc: posOperationBuilder =>
                {
                    posOperationBuilder.SetCheckItems(new List<CheckItem>
                    {
                        CreateCheckItem(DefaultPosOperationId), CreateCheckItem(DefaultPosOperationId)
                    });
                },
                addFiscalizationInfo: true);

            CreatePosOperationAsPaidAndAddFiscalizationInfos(initialMoney, initialBonus,
                additionalInitializationFunc: posOperationBuilder =>
                {
                    posOperationBuilder.SetCheckItems(new List<CheckItem>
                    {
                        CreateCheckItem(SecondPosOperationId), CreateCheckItem(SecondPosOperationId)
                    });
                },
                addFiscalizationInfo: true,
                posOperationId: SecondPosOperationId);

            var fiscalizationInfo = Context.FiscalizationInfos.Single(fi => fi.PosOperationId == SecondPosOperationId);
            fiscalizationInfo.MarkAsPendingError("Some error");
            Context.SaveChanges();

            var incorrectCheckOnlineManager = CreateMockCheckOnlineManager();
            incorrectCheckOnlineManager.MakeReFiscalizationAsync(Context.FiscalizationInfos.Select(fi => fi.Id).ToList()).GetAwaiter().GetResult();
            incorrectCheckOnlineManager.MakeReFiscalizationAsync(Context.FiscalizationInfos.Select(fi => fi.Id).ToList()).GetAwaiter().GetResult();

            _checkOnlineManager.MakeReFiscalizationAsync(Context.FiscalizationInfos.Select(fi => fi.Id).ToList()).Wait();

            AssertExpectedFiscalizationInfosCount(expectedFiscalizationInfosCount);
            var fiscalizationInfos = Context.FiscalizationInfos.AsNoTracking();
            fiscalizationInfos.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(expectedTotalFiscalizationAmountSum);
            fiscalizationInfos.Count(fi => fi.Type == FiscalizationType.Income).Should().Be(expectedFiscalizationInfosCount);
            fiscalizationInfos.Count(fi => fi.State == FiscalizationState.Success).Should().Be(expectedFiscalizationInfosCount);

            var fiscalizationInfosVersionTwo = Context.FiscalizationInfosVersionTwo.AsNoTracking();
            fiscalizationInfosVersionTwo.Count(fi => fi.State == FiscalizationState.Error).Should().Be(expectedErrorFiscalizationInfosVersionTwo);
            fiscalizationInfosVersionTwo.Count().Should().Be(expectedErrorFiscalizationInfosVersionTwo + expectedFiscalizationInfosCount);
            fiscalizationInfosVersionTwo.Count(fi => fi.State == FiscalizationState.Success).Should().Be(expectedFiscalizationInfosCount);
            fiscalizationInfosVersionTwo.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(expectedTotalFiscalizationAmountSum);

        }

        [TestCase(15, 5, 2, 25, 4, 2, 25, true)]
        [TestCase(15, 5, 2, 25, 4, 2, 25, false)]
        public void
            MakeFiscalizationWithPaidPosOperationWithPosOperationTransactionAndTwoFiscalizationInfosAreGivenAndOneFiscalizationInfoInPendingAndTneOtherInPendingErrorStatusWithIncomeRefundTypeAndBrokenFiscalizationServiceAndReturnCorrectFiscalizationInfoCountAndCorrectFiscalizationAmountSum(
                decimal initialMoney,
                decimal initialBonus,
                int expectedFiscalizationInfosCount,
                decimal expectedTotalFiscalizationAmountSum,
                int expectedErrorFiscalizationInfosVersionTwo,
                int expectedSuccessFiscalizationInfosVersionTwo,
                decimal expectedSuccessFiscalizationInfosVersionTwoSum,
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            CreatePosOperationAsPaidAndAddFiscalizationInfos(initialMoney, initialBonus,
                additionalInitializationFunc: posOperationBuilder =>
                {
                    posOperationBuilder.SetCheckItems(new List<CheckItem>
                    {
                        CreateCheckItem(DefaultPosOperationId), CreateCheckItem(DefaultPosOperationId)
                    });
                },
                addFiscalizationInfo: true);

            var posOperation = Context.PosOperations.Single(po => po.Id == DefaultPosOperationId);
            var checkItem = Context.CheckItems.FirstOrDefault();

            var incomeRefundFiscalizationInfo = new FiscalizationInfo(posOperation, new List<CheckItem> { checkItem }, 0M);
            Seeder.Seed(new List<FiscalizationInfo> { incomeRefundFiscalizationInfo });

            var posOperationTransactionCreationInfo = new PosOperationTransactionCreationInfo(
                posOperation,
                new List<CheckItem> { checkItem },
                0M,
                PosOperationTransactionType.Refund);

            var operationTransaction = _transactionCreationUpdatingService.CreateTransaction(posOperationTransactionCreationInfo);
            operationTransaction.MarkAsInProcess();
            operationTransaction.MarkAsPaidUnfiscalized();

            Seeder.Seed(new List<PosOperationTransaction> { operationTransaction });

            var incorrectCheckOnlineManager = CreateMockCheckOnlineManager();

            incorrectCheckOnlineManager.MakeReFiscalizationAsync(Context.FiscalizationInfos.Select(fi => fi.Id).ToList()).GetAwaiter().GetResult();
            incorrectCheckOnlineManager.MakeReFiscalizationAsync(Context.FiscalizationInfos.Select(fi => fi.Id).ToList()).GetAwaiter().GetResult();

            _checkOnlineManager.MakeReFiscalizationAsync(Context.FiscalizationInfos.Select(fi => fi.Id).ToList()).Wait();

            AssertExpectedFiscalizationInfosCount(expectedFiscalizationInfosCount);
            var fiscalizationInfos = Context.FiscalizationInfos.AsNoTracking();
            fiscalizationInfos.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(expectedTotalFiscalizationAmountSum);
            fiscalizationInfos.Count(fi => fi.State == FiscalizationState.Success).Should().Be(expectedFiscalizationInfosCount);

            var fiscalizationInfosVersionTwo = Context.FiscalizationInfosVersionTwo.AsNoTracking();
            fiscalizationInfosVersionTwo.Count(fi => fi.State == FiscalizationState.Error).Should().Be(expectedErrorFiscalizationInfosVersionTwo);
            fiscalizationInfosVersionTwo.Count().Should().Be(expectedErrorFiscalizationInfosVersionTwo + expectedSuccessFiscalizationInfosVersionTwo);
            fiscalizationInfosVersionTwo.Count(fi => fi.State == FiscalizationState.Success).Should().Be(expectedSuccessFiscalizationInfosVersionTwo);
            fiscalizationInfosVersionTwo.Sum(fi => fi.TotalFiscalizationAmount).Should().Be(expectedSuccessFiscalizationInfosVersionTwoSum);

        }

        private Core.Services.CheckOnline.CheckOnlineManager CreateMockCheckOnlineManager()
        {
            var mockCheckOnlineBuilder = new Mock<ICheckOnlineBuilder>();
            mockCheckOnlineBuilder.Setup(b => b.BuildCheck(It.IsAny<IOnlineCashierAuth>(),
                    It.IsAny<IOnlineCashierRequest>(), It.IsAny<FiscalizationType>()))
                .Returns(new BaseOnlineCashierResponse { Errors = "Somethig went wrong" });

            var outErr = "";
            mockCheckOnlineBuilder.Setup(b => b.ValidateRequest(It.IsAny<IOnlineCashierAuth>(),
                    It.IsAny<IOnlineCashierRequest>(), out outErr))
                .Returns(true);

            var diServices = new ServiceCollection();

            diServices.AddTransient(sp => mockCheckOnlineBuilder.Object);
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<IOnlineCashierAuth>());
            diServices.AddSingleton<IPosOperationTransactionTypeProvider, PosOperationTransactionTypeProvider>();
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<ILogger>());
            diServices.AddTransient(sp => _serviceProvider.GetRequiredService<IUnitOfWorkFactory>());


            var serviceProvider = diServices.BuildServiceProvider();

            var incorrectCheckOnlineManager = new Core.Services.CheckOnline.CheckOnlineManager(
                serviceProvider.GetRequiredService<IUnitOfWorkFactory>(),
                serviceProvider.GetRequiredService<ICheckOnlineBuilder>(),
                serviceProvider.GetRequiredService<IOnlineCashierAuth>(),
                serviceProvider.GetRequiredService<IPosOperationTransactionTypeProvider>(),
                serviceProvider.GetRequiredService<ILogger>(),
                3);
            return incorrectCheckOnlineManager;
        }

        private void AssertExpectedFiscalizationInfosCount(int expectedFiscalizationInfosCount)
        {
            var fiscalizationInfo = Context.FiscalizationInfos.AsNoTracking();
            fiscalizationInfo.Count().Should().Be(expectedFiscalizationInfosCount);
        }

        private void AssertExpectedFiscalizationInfosStateAndTypeAndTotalFiscalizationAmountSum(
            int fiscalizationInfoId,
            FiscalizationState expectedState,
            FiscalizationType expectedType,
            decimal? expectedTotalFiscalizationAmountSum = null)
        {
            var fiscalizationInfo = Context.FiscalizationInfos
                .AsNoTracking()
                .SingleOrDefault(fi => fi.Id == fiscalizationInfoId);

            fiscalizationInfo.State.Should().Be(expectedState);
            fiscalizationInfo.Type.Should().Be(expectedType);
            fiscalizationInfo.TotalFiscalizationAmount.Should().Be(expectedTotalFiscalizationAmountSum);
        }

        private CheckItem CreateCheckItem(int posOperationId, decimal amount = 10)
        {
            var defaultLabeledGood = Context.LabeledGoods.Single(lg => lg.Label == FirstLabeledGood);

            return CheckItem.NewBuilder(
                    DefaultPosId,
                    posOperationId,
                    defaultLabeledGood.GoodId.Value,
                    defaultLabeledGood.Id,
                    DefaultCurrencyId)
                .SetPrice(amount)
                .SetStatus(CheckItemStatus.Unpaid)
                .Build();
        }

        private OperationPaymentInfo MakePaymentThenCreateOperationPaymentInfo(decimal initializeMoney,
            decimal initializeBonus)
        {
            if (initializeBonus == decimal.Zero && initializeMoney == decimal.Zero)
                return OperationPaymentInfo.ForNoPayment(DefaultUserId);

            if (initializeMoney == decimal.Zero && initializeBonus > 0)
                return OperationPaymentInfo.ForPaymentViaBonuses(DefaultUserId, initializeBonus);

            var paymentRequest = new PaymentRequest(
                initializeMoney, Currency.Rubles, DefaultCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResult = _paymentService.MakePaymentAsync(paymentRequest).Result;
            var bankTransactionSummary = new BankTransactionSummary(
                Context.PaymentCards.First().Id, paymentResult.Result.TransactionId, initializeMoney
            );

            return OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, initializeBonus);
        }

        private FiscalizationParameters CreatePosOperationAsPaidAndAddFiscalizationInfos(
            decimal initialMoney,
            decimal initialBonus,
            Action<PosOperationOfUserAndPosBuilder> additionalInitializationFunc = null,
            bool addFiscalizationInfo = true,
            int posOperationId = DefaultPosOperationId)
        {
            var user = Context.Users.Single(u => u.Id == DefaultUserId);
            user.AddBonusPoints(initialBonus, BonusType.Payment);
            Context.SaveChanges();

            var operationPaymentInfo = MakePaymentThenCreateOperationPaymentInfo(initialMoney, initialBonus);

            var operationOfUserAndPosBuilder = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId);

            additionalInitializationFunc?.Invoke(operationOfUserAndPosBuilder);

            var posOperation = operationOfUserAndPosBuilder.Build();

            Seeder.Seed(new List<PosOperation> { posOperation });

            var savedPosOperation =
                Context.PosOperations.First(po => po.Id == posOperationId);
            savedPosOperation.WriteOffBonusPoints();

            var createTransactionResult = _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, savedPosOperation).GetAwaiter().GetResult();

            savedPosOperation.MarkAs(PosOperationStatus.PendingPayment);
            savedPosOperation.MarkAsPaid(operationPaymentInfo);

            Context.SaveChanges();

            if (addFiscalizationInfo)
            {
                var fiscalizationInfo = new FiscalizationInfo(savedPosOperation);
                Seeder.Seed(new List<FiscalizationInfo> { fiscalizationInfo });
            }

            var operation = _unitOfWork.PosOperations.GetIncludingCheckItemsAsync(DefaultPosOperationId).GetAwaiter().GetResult();

            var operationTransaction = createTransactionResult.Value;
            operationTransaction.MarkAsInProcess();
            operationTransaction.MarkAsPaidUnfiscalized();

            _unitOfWork.CompleteAsync().GetAwaiter().GetResult();

            return new FiscalizationParameters(operation, operationTransaction);
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
    }
}
