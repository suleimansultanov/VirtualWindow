using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Models;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Shared.Models;
using NasladdinPlace.Core.Services.UnpaidPurchases.Finisher;
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

namespace NasladdinPlace.Api.Tests.Scenarios.UnpaidManager
{
    public class UnpaidPurchasesFinisherIntegrationTests : TestsBase
    {
        private const int UserIdWithActivePaymentCard = 1;
        private const int UserWithoutPaymentSystemId = 2;
        private const int DefaultPosId = 1;
        private const int DefaultBankTransactionId = 1;
        private const int DefaultDebtorsCount = 5;
        private const int DefaultPaymentCardId = 1;

        private IUnpaidPurchaseFinisher _unpaidPurchaseFinisher;
        private IPurchaseManager _purchaseManager;
        private IOperationTransactionManager _operationTransactionManager;
        private IPosOperationTransactionCreationUpdatingService _transactionCreationUpdatingService;
        private IUnitOfWork _unitOfWork;
        private int _purchaseCompletionResultsCount;

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
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(2));
            Seeder.Seed(new PaymentCardsDataSet(UserIdWithActivePaymentCard));
            Seeder.Seed(new PaymentCardsDataSet(UserWithoutPaymentSystemId));

            TryAssignPaymentSystemToUser(UserIdWithActivePaymentCard, GetPaymentCardIdByUserId(UserIdWithActivePaymentCard));

            Mapper.Reset();
            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var services = TestServiceProviderFactory.Create();

            _purchaseManager = services.GetRequiredService<IPurchaseManager>();
            _unpaidPurchaseFinisher = services.GetRequiredService<IUnpaidPurchaseFinisher>();
            _operationTransactionManager = services.GetRequiredService<IOperationTransactionManager>();
            _transactionCreationUpdatingService = services.GetRequiredService<IPosOperationTransactionCreationUpdatingService>();
            _unitOfWork = services.GetRequiredService<IUnitOfWorkFactory>().MakeUnitOfWork();
            _purchaseCompletionResultsCount = 0;
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            FinishUnpaidPurchases_CheckItemNoActionAndPosOperationInThePastIsGivenAndUserPaymentSystemIsNotNull_ShouldReturnCorrectPurchaseCompletionResults(
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            var posOperation = PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                .MarkAsCompleted()
                .Build();

            Insert(posOperation);

            Insert(CreateCheckItemFromPosOperationIdAndLabeledGoodId(posOperation.Id, 1));

            CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 1,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>
                {
                    {1, 5M}
                }
            );

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 1, expectedPaymentAmount: 5M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 1);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 5M,
                fiscalizationType: FiscalizationType.Income
                );

        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_UserPaymentSystemIsNull_ShouldReturnEmpty(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperation =
                PosOperation.NewOfUserAndPosBuilder(UserWithoutPaymentSystemId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build();

            Insert(posOperation);

            Insert(CreateCheckItemFromPosOperationIdAndLabeledGoodId(posOperation.Id, 1));

            CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 0,
                expectedCheckTotalPriceByOperationIdDictionary: null);

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 0, expectedPaymentAmount: 0M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 0);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.Income
            );

        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_PosOperationIsGivenAndPosOperationTransactionIsNotGivenAndUserPaymentSystemIsNotNullAndPaymentCardHasIncorrectToken_ShouldReturnEmpty(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                .MarkAsCompleted()
                .Build();

            Insert(posOperation);

            var checkItems = new Collection<CheckItem>()
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1, CheckItemStatus.Deleted),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1, CheckItemStatus.Deleted),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1)

            };
            Insert(checkItems.ToArray());

            Insert(CreateCheckItemFromPosOperationIdAndLabeledGoodId(posOperation.Id, 1));

            var paymentCard = Context.PaymentCards.FirstOrDefault(pc => pc.Id == DefaultPaymentCardId);
            paymentCard.SetProperty("Token", Guid.NewGuid().ToString());

            Context.SaveChanges();

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: useNewPaymentSystem ? 0 : 1,
                expectedCheckTotalPriceByOperationIdDictionary: null);

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 0, expectedPaymentAmount: 0M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 0);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 0,
                expectedTransactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.Income
            );

        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_CheckItemNoActionAndRecentPosOperationIsGivenAndUserPaymentSystemIsNotNull_ShouldReturnEmpty(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperation =
                PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(0))
                    .Build();

            Insert(posOperation);

            Insert(CreateCheckItemFromPosOperationIdAndLabeledGoodId(posOperation.Id, 1));

            CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 0,
                expectedCheckTotalPriceByOperationIdDictionary: null);

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 0, expectedPaymentAmount: 0M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 0);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_PosOperationInThePastIsPaid_ShouldReturnEmpty(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                .MarkAsPendingPayment()
                .Build();

            var bankTransactionSummary = new BankTransactionSummary(
                GetPaymentCardIdByUserId(UserIdWithActivePaymentCard), DefaultBankTransactionId, 5M
            );
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(UserIdWithActivePaymentCard, bankTransactionSummary, 5M);
            posOperation.MarkAsPaid(operationPaymentInfo);

            Insert(posOperation);

            Insert(CreateCheckItemFromPosOperationIdAndLabeledGoodId(posOperation.Id, 1));

            CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 0,
                expectedCheckTotalPriceByOperationIdDictionary: null);

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 1, expectedPaymentAmount: 5M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 0);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_CheckItemDeleteAndPosOperationInThePastIsGivenUserPaymentSystemIsNotNull_ShouldReturnEmpty(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                .Build();

            Insert(posOperation);

            Insert(CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1, CheckItemStatus.Deleted));

            CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 0,
                expectedCheckTotalPriceByOperationIdDictionary: null);

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 0, expectedPaymentAmount: 0M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 0);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_CheckItemsAreGivenAndPaymenSystemIsNotNullAndPosOperationInThePastIsGiven_ShouldReturnPurchaseCompleteResultWithExpectedResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                .MarkAsCompleted()
                .Build();

            Insert(posOperation);

            var checkItems = new Collection<CheckItem>()
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1, CheckItemStatus.Deleted),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1, CheckItemStatus.Deleted),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1)

            };
            Insert(checkItems.ToArray());

            CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 1,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>()
                {
                    {1, 10M}
                });

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 1, expectedPaymentAmount: 10M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 1);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 10M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 10M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_CheckItemsAreGivenAndPaymenSystemIsNotNullAndPosOperationInThePastIsGivenAndPosOperationTransactionIsNotGiven_ShouldReturnPurchaseCompleteResultWithExpectedResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                .MarkAsCompleted()
                .Build();

            Insert(posOperation);

            var checkItems = new Collection<CheckItem>()
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1, CheckItemStatus.Deleted),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1, CheckItemStatus.Deleted),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1)

            };

            Insert(checkItems.ToArray());

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: useNewPaymentSystem ? 0 : 1,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>()
                {
                    {1, 10M}
                });

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: useNewPaymentSystem ? 0 : 1, expectedPaymentAmount: useNewPaymentSystem ? 0 : 10M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: useNewPaymentSystem ? 0 : 1);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 0,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_TheSecondUserPaymentSystemIsNullAndPosOperationsInThePastAreGiven_ShouldReturnCorrectPurchaseCompletionResults(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .MarkAsCompleted()
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserWithoutPaymentSystemId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .Build()
            };

            Insert(posOperations.ToArray());

            var checkItems = new Collection<CheckItem>()
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(2, 1)

            };
            Insert(checkItems.ToArray());

            foreach (var posOperation in posOperations)
                CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 1,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>()
                {
                    {1, 5M}
                });

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 1, expectedPaymentAmount: 5M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 1);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 5M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_TwoUsersWithPaymentSystemsAndRecentPosOperationsAreGiven_ShouldReturnNoPurchaseCompletionResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            TryAssignPaymentSystemToUser(UserWithoutPaymentSystemId, Context.PaymentCards.FirstOrDefault()?.Id);

            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(0))
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserWithoutPaymentSystemId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(0))
                    .Build()
            };
            Insert(posOperations.ToArray());

            var checkItems = new Collection<CheckItem>()
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(2, 1)

            };
            Insert(checkItems.ToArray());

            foreach (var posOperation in posOperations)
                CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 0,
                expectedCheckTotalPriceByOperationIdDictionary: null);

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 0, expectedPaymentAmount: 0M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 0);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.Unpaid,
                expectedBankTransactionsCount: 0,
                expectedBankTransactionsAmount: 0M,
                expectedFiscalizationCount: 0,
                expectedFiscalizationAmount: 0M,
                fiscalizationType: FiscalizationType.Income
            );

        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            FinishUnpaidPurchases_TwoUsersWithPaymentSystemsAndPosOperationsInThePastAreGiven_ShouldReturnCorrectPurchaseCompletionResults(
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            TryAssignPaymentSystemToUser(UserWithoutPaymentSystemId, GetPaymentCardIdByUserId(UserWithoutPaymentSystemId));
            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .MarkAsCompleted()
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserWithoutPaymentSystemId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .MarkAsCompleted()
                    .Build()
            };
            Insert(posOperations.ToArray());

            var checkItems = new Collection<CheckItem>()
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 2),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(2, 3)

            };
            Insert(checkItems.ToArray());

            foreach (var posOperation in posOperations)
                CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 2,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>()
                {
                    {2, 10M},
                    {1, 5M}
                });

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 2, expectedPaymentAmount: 15M, expectedFirstBankTransactionAmount: 10M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 2);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 2,
                expectedBankTransactionsAmount: 15M,
                expectedFiscalizationCount: 2,
                expectedFiscalizationAmount: 15M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FinishUnpaidPurchases_TwoUsersWithPaymentSystemsPosOperationsInThePastAreGivenAndTheFirstPosOperationIsPaid_ShouldReturnCorrectCompletionResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            TryAssignPaymentSystemToUser(UserWithoutPaymentSystemId, GetPaymentCardIdByUserId(UserWithoutPaymentSystemId));
            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .MarkAsPendingPayment()
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserWithoutPaymentSystemId, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .MarkAsCompleted()
                    .Build()
            };

            var bankTransactionSummary = new BankTransactionSummary(
                GetPaymentCardIdByUserId(UserIdWithActivePaymentCard), DefaultBankTransactionId, 5M
            );
            var operationPaymentInfo = OperationPaymentInfo.ForMixPayment(UserIdWithActivePaymentCard, bankTransactionSummary, 5M);
            posOperations.FirstOrDefault()?.MarkAsPaid(operationPaymentInfo);

            Insert(posOperations.ToArray());

            var checkItems = new Collection<CheckItem>
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 2),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(2, 3)

            };
            Insert(checkItems.ToArray());

            foreach (var posOperation in posOperations)
                CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 1,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>()
                {
                    {1, 5M}
                });

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 2, expectedPaymentAmount: 10M, expectedFirstBankTransactionAmount: 5M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 1);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 5M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            FinishUnpaidPurchases_UserWithPaymentSystemsAndCompletedPosOperationsInThePastAreGiven_ShouldReturnCorrectPurchaseCompletionResults(
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-2))
                    .MarkAsCompleted()
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .MarkAsCompleted()
                    .Build()
            };
            Insert(posOperations.ToArray());

            var checkItems = new Collection<CheckItem>()
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 2),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(2, 3)

            };
            Insert(checkItems.ToArray());

            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            foreach (var posOperation in savedPosOperations)
                CreateTransactionForPosOperation(posOperation);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 1,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>()
                {
                    {2, 5M},
                    {1, 10M}
                });

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 2, expectedPaymentAmount: 15M, expectedFirstBankTransactionAmount: 5M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 2);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 2,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 2,
                expectedBankTransactionsAmount: 15M,
                expectedFiscalizationCount: 2,
                expectedFiscalizationAmount: 15M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            FinishUnpaidPurchases_UsersWithPaymentSystemAndCompletedPosOperationsInThePastAreGiven_ShouldReturnCorrectPurchaseCompletionResults(
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            CreateDebtorUsers(DefaultDebtorsCount);

            var createdUsers = Context.Users.Select(us => us).ToList();

            createdUsers.ForEach(us =>
            {
                Seeder.Seed(new PaymentCardsDataSet(us.Id));
                TryAssignPaymentSystemToUser(us.Id, GetPaymentCardIdByUserId(us.Id));

                var posOperations = new Collection<PosOperation>
                {
                    PosOperation.NewOfUserAndPosBuilder(us.Id, DefaultPosId)
                        .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                        .MarkAsCompleted()
                        .Build()
                };
                Insert(posOperations.ToArray());

                var lastCreatedPosOperationId = Context.PosOperations.Last().Id;

                var checkItems = new Collection<CheckItem>()
                {
                    CreateCheckItemFromPosOperationIdAndLabeledGoodId(lastCreatedPosOperationId, 1),
                    CreateCheckItemFromPosOperationIdAndLabeledGoodId(lastCreatedPosOperationId, 2)
                };
                Insert(checkItems.ToArray());
            });

            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            foreach (var posOperation in savedPosOperations)
                CreateTransactionForPosOperation(posOperation);

            var serviceProvider = CreateServiceProviderWithMockPaymentService();

            var unpaidPurchaseFinisher = serviceProvider.GetRequiredService<IUnpaidPurchaseFinisher>();
            var purchaseManager = serviceProvider.GetRequiredService<IPurchaseManager>();

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 5,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>()
                {
                    {1, 10M},
                    {2, 10M},
                    {3, 10M},
                    {4, 10M},
                    {5, 10M}
                },
                unpaidPurchaseFinisher: unpaidPurchaseFinisher,
                purchaseManager: purchaseManager);

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 5, expectedPaymentAmount: 50M, expectedFirstBankTransactionAmount: 10M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 5);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 5,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 5,
                expectedBankTransactionsAmount: 50M,
                expectedFiscalizationCount: 5,
                expectedFiscalizationAmount: 50M,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            FinishUnpaidPurchases_UserWithPaymentSystemsAndCompletedPosOperationsInThePastAndMockedPaymentServiceAreGiven_ShouldReturnCorrectPurchaseCompletionResults(
                bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            var posOperations = new Collection<PosOperation>
            {
                PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-2))
                    .MarkAsCompleted()
                    .Build(),
                PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                    .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                    .MarkAsCompleted()
                    .Build()
            };
            Insert(posOperations.ToArray());

            var checkItems = new Collection<CheckItem>()
            {
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 1),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(1, 2),
                CreateCheckItemFromPosOperationIdAndLabeledGoodId(2, 3)

            };
            Insert(checkItems.ToArray());

            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            foreach (var posOperation in savedPosOperations)
                CreateTransactionForPosOperation(posOperation);

            var serviceProvider = CreateServiceProviderWithMockPaymentService(createErrorPaymentResponse: true);

            var unpaidPurchaseFinisher = serviceProvider.GetRequiredService<IUnpaidPurchaseFinisher>();
            var purchaseManager = serviceProvider.GetRequiredService<IPurchaseManager>();

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 1,
                expectedCheckTotalPriceByOperationIdDictionary: null,
                unpaidPurchaseFinisher: unpaidPurchaseFinisher,
                purchaseManager: purchaseManager);

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 1, expectedPaymentAmount: 0M, expectedFirstBankTransactionAmount: 0M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 0);
            EnsurePosOperationTransactionsHaveCount(2);
            EnsureBankTransactionInfosVersionTwoHaveCountAndAmount(1, 10M);
            EnsureFiscalizationInfosVersionTwoHaveCountAndAmount(0, 0M, FiscalizationType.Income);
        }

        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        [TestCase(true, true, true)]
        public void
            FinishUnpaidPurchases_PosOperationInThePastIsGivenAndUserPaymentSystemIsNotNullAndExtraTransactionIsAdded_ShouldReturnCorrectPurchaseCompletionResults(
                bool addAdditionTransaction,
                bool addRefundTransaction,
                bool addVerificationTransaction)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem: true);

            var posOperation = PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                .MarkAsCompleted()
                .Build();

            Insert(posOperation);

            var checkItem = CreateCheckItemFromPosOperationIdAndLabeledGoodId(posOperation.Id, 1);
            Insert(checkItem);

            CreateTransactionForPosOperation(posOperation);

            if (addAdditionTransaction)
                CreateExtraTransactionForPosOperation(posOperation: posOperation, checkItem: checkItem, transactionType: PosOperationTransactionType.Addition);

            if (addRefundTransaction)
                CreateExtraTransactionForPosOperation(posOperation: posOperation, checkItem: checkItem, transactionType: PosOperationTransactionType.Refund);

            if (addVerificationTransaction)
                CreateExtraTransactionForPosOperation(posOperation: posOperation, checkItem: checkItem, transactionType: PosOperationTransactionType.Verification);

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 1,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>
                {
                    {1, 5M}
                }
            );

            var isAddThreeDifferentTransactions = addAdditionTransaction && addRefundTransaction && addVerificationTransaction;
            var expectedFiscaliztionBankTransactionAndOperationTransactionCount = isAddThreeDifferentTransactions ? 4 : 2;
            var expectedAmount = isAddThreeDifferentTransactions ? 20M : 10M;

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: expectedFiscaliztionBankTransactionAndOperationTransactionCount, expectedPaymentAmount: expectedAmount, expectedFirstBankTransactionAmount: 5M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: expectedFiscaliztionBankTransactionAndOperationTransactionCount);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: expectedFiscaliztionBankTransactionAndOperationTransactionCount,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: expectedFiscaliztionBankTransactionAndOperationTransactionCount,
                expectedBankTransactionsAmount: expectedAmount,
                expectedFiscalizationCount: expectedFiscaliztionBankTransactionAndOperationTransactionCount,
                expectedFiscalizationAmount: expectedAmount,
                fiscalizationType: FiscalizationType.Income
            );
        }

        [Test]
        public void
            FinishUnpaidPurchases_CheckItemNoActionAndPosOperationInThePastIsGivenAndUserPaymentSystemIsNotNullAndAdditionTransactionIsGiven_ShouldReturnCorrectPurchaseCompletionResults()
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(true);
            var posOperation = PosOperation.NewOfUserAndPosBuilder(UserIdWithActivePaymentCard, DefaultPosId)
                .SetDateStarted(DateTime.UtcNow.AddDays(-1))
                .MarkAsCompleted()
                .Build();

            Insert(posOperation);

            Insert(CreateCheckItemFromPosOperationIdAndLabeledGoodId(posOperation.Id, 1));

            CreateTransactionForPosOperation(posOperation);

            var createdTransacitons = Context.PosOperationTransactions.ToImmutableList();

            foreach (var posOperationTransaction in createdTransacitons)
            {
                posOperationTransaction.SetProperty("Type", PosOperationTransactionType.Addition);
            }

            Context.SaveChanges();

            SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
                expectedDebtorsCount: 1,
                expectedCheckTotalPriceByOperationIdDictionary: new Dictionary<int, decimal>
                {
                    {1, 5M}
                }
            );

            Context.PosOperations.AsNoTracking().First().DatePaid.Should().NotBeNull();
            foreach (var checkItem in Context.CheckItems.AsNoTracking())
            {
                checkItem.Status.Should().Be(CheckItemStatus.Paid);
            }

            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(expectedCount: 1, expectedPaymentAmount: 5M);
            AssertExpectedFiscalizationCountInDatabase(expectedFiscalizationCount: 1);
            EnsurePosOperationTransactionHasCorrectBankTransactionsAndFiscalizationAmounts(
                expectedTransactionsCount: 1,
                expectedTransactionStatus: PosOperationTransactionStatus.PaidFiscalized,
                expectedBankTransactionsCount: 1,
                expectedBankTransactionsAmount: 5M,
                expectedFiscalizationCount: 1,
                expectedFiscalizationAmount: 5M,
                fiscalizationType: FiscalizationType.Income
            );
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

        private static IServiceProvider CreateServiceProviderWithMockPaymentService(bool createErrorPaymentResponse = false)
        {
            var services = TestServiceProviderFactory.CreateServiceCollection();

            var mockCheckPaymentService = new Mock<ICheckPaymentService>();

            if (createErrorPaymentResponse)
            {
                mockCheckPaymentService
                    .Setup(x => x.PayForCheckAsync(It.IsAny<IUnitOfWork>(), It.IsAny<int>(), It.IsAny<PaymentInfo>()))
                    .Returns(Task.FromResult(CheckPaymentResult.FailureWithBankRequisites(
                            Error.FromDescription("Some Error"),
                            123123,
                            1,
                            new CheckPaymentInfo(10M, 0M),
                            "Bank Error")));
            }
            else
            {
                mockCheckPaymentService
                    .Setup(x => x.PayForCheckAsync(It.IsAny<IUnitOfWork>(), It.IsAny<int>(), It.IsAny<PaymentInfo>()))
                    .Returns(Task.FromResult(CheckPaymentResult.Paid(123123, 3, new CheckPaymentInfo(10M, 0M))));
            }

            TestServiceProviderFactory.ExchangeService<ICheckPaymentService>(services, provider => mockCheckPaymentService.Object);

            return services.BuildServiceProvider();
        }

        private void CreateDebtorUsers(int debtorsCount)
        {
            var debtorUsersCollection = new List<ApplicationUser>();
            for (var i = 3; i <= debtorsCount; i++)
            {
                debtorUsersCollection.Add(new ApplicationUser
                {
                    Id = 0,
                    Email = $"user{i}@domain.com",
                    Birthdate = DateTime.UtcNow.AddYears(-22),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    AccessFailedCount = 0,
                    EmailConfirmed = false,
                    FullName = null,
                    Gender = Gender.Female,
                    LockoutEnabled = true,
                    LockoutEnd = null,
                    NormalizedEmail = $"USER{i}@domain.com",
                    NormalizedUserName = $"96777276646{i}",
                    PasswordHash = "AQAAAAEAACcQAAAAEOLVZHSpr5pXKDo22pI6sf/Tr4ZUwEbXybxQSqpsBXY/zLi/R9Yd9vqFJbmbzQBSpw==",
                    PhoneNumber = $"926210305{i}",
                    PhoneNumberConfirmed = true,
                    SecurityStamp = "0d6576d1-8d64-418e-87c1-008f64c061d2",
                    UserName = $"9677727664{i}"
                });
            }

            Seeder.Seed(debtorUsersCollection);
        }

        private void TryAssignPaymentSystemToUser(int id, int? paymentId)
        {
            if (!paymentId.HasValue) return;

            var user = Context.Users
                .Include(u => u.PaymentCards)
                .Single(u => u.Id == id);

            try
            {
                user.SetActivePaymentCard(paymentId.Value);
            }
            catch (InvalidOperationException ex)
            {
                //Перехватываем исключение установки кривой карты
                ex.Message.Should().Be($"Payment card with id = {paymentId.Value} does not exists.");
            }

            Context.SaveChanges();
        }

        private static CheckItem CreateCheckItemFromPosOperationIdAndLabeledGoodId(
            int posOperationId,
            int labeledGoodId,
            CheckItemStatus checkItemStatus = CheckItemStatus.Unpaid)
        {
            return CheckItem.NewBuilder(
                    DefaultPosId,
                    posOperationId,
                    1,
                    labeledGoodId,
                    1)
                .SetPrice(5M)
                .SetStatus(checkItemStatus)
                .Build();
        }

        private void SubscribeForPurchaseCompletedEventAndAssertExpectedResult(
            int expectedDebtorsCount,
            IDictionary<int, decimal> expectedCheckTotalPriceByOperationIdDictionary,
            IUnpaidPurchaseFinisher unpaidPurchaseFinisher = null,
            IPurchaseManager purchaseManager = null)
        {
            var debtorsIds = new HashSet<int>();

            _purchaseManager = purchaseManager ?? _purchaseManager;

            _purchaseManager.PurchaseCompleted += (sender, purchaseCompletionResult) =>
            {
                var userId = purchaseCompletionResult.User.Id;

                if (!debtorsIds.Contains(userId))
                {
                    debtorsIds.Add(userId);
                    Interlocked.Increment(ref _purchaseCompletionResultsCount);
                }

                if (purchaseCompletionResult.Operation != null)
                {
                    expectedCheckTotalPriceByOperationIdDictionary
                        .Should()
                        .ContainKey(purchaseCompletionResult.Operation.Id)
                        .WhichValue
                        .Should()
                        .Be(purchaseCompletionResult.Check.Summary.CostSummary.CostWithoutDiscount);
                }
            };

            _unpaidPurchaseFinisher = unpaidPurchaseFinisher ?? _unpaidPurchaseFinisher;

            _unpaidPurchaseFinisher.FinishUnpaidPurchasesAsync(TimeSpan.FromMinutes(30)).Wait();

            Interlocked.CompareExchange(ref _purchaseCompletionResultsCount, expectedDebtorsCount, expectedDebtorsCount)
                .Should()
                .Be(expectedDebtorsCount);
        }

        private void Insert<T>(params T[] entities) where T : class
        {
            Seeder.Seed(entities);
        }

        private int GetPaymentCardIdByUserId(int userId)
        {
            return Context.PaymentCards.First(pc => pc.UserId == userId)?.Id ?? 0;
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

        private void CreateTransactionForPosOperation(PosOperation posOperation)
        {
            _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, posOperation).GetAwaiter().GetResult();
        }

        private void
            AssertExpectedBankTransactionsNumberAndPaymentAmountInDatabase(
                int expectedCount,
                decimal? expectedPaymentAmount,
                BankTransactionInfoType bankTransactionInfoType = BankTransactionInfoType.Payment,
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

        private void EnsureBankTransactionInfosVersionTwoHaveCountAndAmount(int expectedCount, decimal expectedAmount)
        {
            var bankTransactionsVersionTwo = Context.BankTransactionInfosVersionTwo.ToImmutableList();

            bankTransactionsVersionTwo.Should().HaveCount(expectedCount);
            bankTransactionsVersionTwo.Sum(bti => bti.Amount).Should().Be(expectedAmount);
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


            EnsurePosOperationTransactionsHaveCount(expectedTransactionsCount);
            EnsureBankTransactionInfosVersionTwoHaveCountAndAmount(expectedBankTransactionsCount, bankTransactionAmount);
            EnsureFiscalizationInfosVersionTwoHaveCountAndAmount(expectedFiscalizationCount, fiscalizaionAmount, fiscalizationType);
        }

        private void AssertExpectedFiscalizationCountInDatabase(int expectedFiscalizationCount)
        {
            Context.FiscalizationInfos.AsNoTracking().Count().Should().Be(expectedFiscalizationCount);
        }
    }
}
