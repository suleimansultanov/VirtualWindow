using System;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.PurchaseCompletionResult;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.Api.Dtos.SimpleCheck;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models.PaymentCards;
using NasladdinPlace.Core.Services.Payment.Card;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.TestUtils.Extensions;

namespace NasladdinPlace.Api.Tests.Controllers
{
    public class ChecksControllerTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 1;
        private const int DefaultPosOperationId = 1;
        private const int SecondPosOperationId = 2;
        private const int DefaultGoodId = 1;
        private const int DefaultLabeledGoodId = 1;
        private const int DefaultCurrencyId = 1;
        private const int DefaultBankTransactionId = 1;
        private const decimal DefaultPrice = 15M;
        private const string TimeForPaymentHasNotArrivedErrorMessage = "The time for payment has not yet arrived. Please try again later.";
        private const string NotPossibleToPayErrorMessage = "It’s not possible to pay a check, it may already have been paid.";
        private const string InternalServerErrorMessage = "Internal server error. Unexpected pos operation status (not equal pending payments).";
        private const string PaymentCardNotFoundErrorMessage = "Payment card not found.";
        private const string NotEnoughMoney = "Not enough money.";
        private const string IncorrectCheckFormat = "Return of goods because incorrect check format.";
        private const int DefaultVisaCardId = 1;
        private const int DefaultMasterCardId = 2;
        private const int DefaultAmount = 0;

        private ChecksController _checksController;
        private IOperationTransactionManager _operationTransactionManager;
        private IUnitOfWork _unitOfWork;

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
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();

            var controllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);

            _checksController = serviceProvider.GetRequiredService<ChecksController>();
            _checksController.ControllerContext = controllerContext;
            _operationTransactionManager = serviceProvider.GetRequiredService<IOperationTransactionManager>();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkFactory>().MakeUnitOfWork();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayForAllChecks_NoUnpaidPosOperationsAreGiven_ShouldReturnErrorPurchaseCompletionResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            SeedUnpaidPosOperation();

            var posOperation = Context.PosOperations.First(po => po.Id == DefaultPosOperationId);
            posOperation.MarkAsPendingPayment();
            posOperation.MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId));

            Context.SaveChanges();

            EnsurePurchaseCompletionResultsAreCorrectHaveExpectedResultsCountAndHaveExpectedResultsStatus(
                expectedResultsCount: 1, expectedResultsStatus: PurchaseCompletionStatus.UnpaidPurchaseNotFound);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayForAllChecks_TwoCompletedPosOperationsAreGivenAndActivePaymentSystemIsNull_ShouldReturnPaymentErrorPurchaseCompletionResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus();

            EnsurePurchaseCompletionResultsAreCorrectHaveExpectedResultsCountAndHaveExpectedResultsStatus(
                expectedResultsCount: 1, expectedResultsStatus: PurchaseCompletionStatus.PaymentError);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayForAllChecks_TwoUnpaidPosOperationsAreGivenAndActivePaymentSystemIsNotNull_ShouldReturnSuccessPurchaseCompletionResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus();

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);

            Context.SaveChanges();

            EnsurePurchaseCompletionResultsAreCorrectHaveExpectedResultsCountAndHaveExpectedResultsStatus(
                expectedResultsCount: 2, expectedResultsStatus: PurchaseCompletionStatus.Success);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayForAllChecks_TwoCompletedPosOperationsAreGivenAndActivePaymentSystemIsNullAndCheckItemsForSecondOperationHaveUnverifiedStatus_ShouldReturnPaymentErrorPurchaseCompletionResultForFirstOperation(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus(checkItemStatus: CheckItemStatus.Unverified);

            EnsurePurchaseCompletionResultsAreCorrectHaveExpectedResultsCountAndHaveExpectedResultsStatus(
                expectedResultsCount: 1, expectedResultsStatus: PurchaseCompletionStatus.PaymentError);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayForAllChecks_TwoCompletedPosOperationsAreGivenAndActivePaymentSystemIsNotNullAndCheckItemsForSecondOperationHaveUnverifiedStatus_ShouldReturnSuccessPurchaseCompletionResultForFirstOperation(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus(checkItemStatus: CheckItemStatus.Unverified);

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);

            EnsurePurchaseCompletionResultsAreCorrectHaveExpectedResultsCountAndHaveExpectedResultsStatus(
                expectedResultsCount: 1, expectedResultsStatus: PurchaseCompletionStatus.Success);
        }

        [TestCase(false, 2, 1)]
        [TestCase(true, 2, 1)]
        [TestCase(false, 1, 2)]
        [TestCase(true, 1, 2)]
        public void PayCheck_ThreeUnpaidPosOperationsAreGivenAndAnotherPaymentCard_ShouldReturnSuccessUnpaidPurchaseCompletionResult(
            bool useNewPaymentSystem, int posOperationId, int nextCheckId)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus();

            SeedUnpaidPosOperation(false);

            Seeder.Seed(new Collection<PaymentCard>
            {
                CreatePaymentCard(DefaultUserId)
            });

            SetActivePaymentCardForDefaultUser(DefaultMasterCardId);

            EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
                posOperationId: posOperationId, nextCheckId: nextCheckId,
                isSucceeded: true, paymentCardId: DefaultVisaCardId);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GetFirstUnpaidCheck_UserHaveUnpaidPosOperations_ShouldReturnFirstUnpaidCheckResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus();

            EnsureFirstUnpaidCheckIsCorrectHaveExpectedResultAndHaveExpectedCheck();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GetFirstUnpaidCheck_UserHasNotUnpaidPosOperations_ShouldReturnFirstUnpaidCheckResult(bool useNewPaymentSystem)
        {
            EnsureFirstUnpaidCheckReturnedNotFoundResult();
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_TwoUnpaidPosOperationsAreGivenAndActivePaymentSystemIsNotNull_ShouldReturnSuccessUnpaidPurchaseCompletionResultWithSecondCheck(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            
            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus();

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);

            EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
                posOperationId: DefaultPosOperationId, nextCheckId: SecondPosOperationId,
                isSucceeded: true);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_TwoUnpaidPosOperationsAreGivenAndAnotherPaymentCard_ShouldReturnSuccessUnpaidPurchaseCompletionResultWithSecondCheck(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus();

            Seeder.Seed(new Collection<PaymentCard>
            {
                CreatePaymentCard(DefaultUserId)
            });

            SetActivePaymentCardForDefaultUser(DefaultMasterCardId);

            EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
                posOperationId: DefaultPosOperationId, nextCheckId: SecondPosOperationId,
                isSucceeded: true, paymentCardId: DefaultVisaCardId);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_TwoUnpaidPosOperationsAreGivenAndActivePaymentSystemIsNotNull_ShouldReturnSuccessUnpaidPurchaseCompletionResultWithFirstCheck(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            
            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus();

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);

            EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
                posOperationId: SecondPosOperationId, nextCheckId: DefaultPosOperationId,
                isSucceeded: true);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_TwoUnpaidPosOperationsAreGivenAndAnotherPaymentCard_ShouldReturnSuccessUnpaidPurchaseCompletionResultWithFirstCheck(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SeedUnpaidPosOperation();

            CreateCompletedPosOperationAndCheckItemsWithStatus();

            Seeder.Seed(new Collection<PaymentCard>
            {
                CreatePaymentCard(DefaultUserId)
            });

            SetActivePaymentCardForDefaultUser(DefaultMasterCardId);

            EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
                posOperationId: SecondPosOperationId, nextCheckId: DefaultPosOperationId,
                isSucceeded: true, paymentCardId: DefaultVisaCardId);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_OneUnpaidPosOperationIsGivenAndActivePaymentSystemIsNotNull_ShouldReturnSuccessUnpaidPurchaseCompletionResultWithoutNextCheck(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            
            SeedUnpaidPosOperation();

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);
            
            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            savedPosOperations.ForEach(po => _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, po).GetAwaiter().GetResult());
            
            Context.SaveChanges();
            
            EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
                posOperationId: DefaultPosOperationId, isSucceeded: true);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_NoUnpaidPosOperationIsGivenAndActivePaymentSystemIsNotNull_ShouldReturnBadRequestObjectResult(bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            
            SeedUnpaidPosOperation();

            var posOperation = Context.PosOperations.First(po => po.Id == DefaultPosOperationId);
            posOperation.MarkAsPendingPayment();
            posOperation.MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId));

            Context.SaveChanges();
            EnsurePurchaseCompletionResultIsIncorrectAndHaveExpectedBadRequestResult(
                posOperationId: DefaultPosOperationId, errorMessage: $"Pos operation with id = {DefaultPosOperationId} not found.");
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_OneUnpaidPosOperationWithCurrentDateCreatedBankTransactionIsGivenAndActivePaymentSystemIsNotNull_ShouldReturnBadRequestObjectResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            
            SeedUnpaidPosOperation();

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);

            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            savedPosOperations.ForEach(po => _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, po).GetAwaiter().GetResult());

            var amount = savedPosOperations.Sum(po => po.CheckItems.Sum(ci => ci.Price));

            var posOperation = Context.PosOperations.First(po => po.Id == DefaultPosOperationId);
            posOperation.AddBankTransaction(BankTransactionInfo.ForError(Context.PaymentCards.First().Id,
                DefaultBankTransactionId, amount, ""));

            Context.SaveChanges();

            EnsurePurchaseCompletionResultIsIncorrectAndHaveExpectedBadRequestObjectResult(
                posOperationId: DefaultPosOperationId, errorMessage: TimeForPaymentHasNotArrivedErrorMessage);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_OneUnpaidPosOperationWithCorrectDateCreatedBankTransactionIsGivenAndActivePaymentSystemIsNotNull_ShouldReturnSuccessResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            
            SeedUnpaidPosOperation();

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);

            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            savedPosOperations.ForEach(po => _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, po).GetAwaiter().GetResult());

            var amount = savedPosOperations.Sum(po => po.CheckItems.Sum(ci => ci.Price));

            var posOperation = Context.PosOperations.First(po => po.Id == DefaultPosOperationId);
            posOperation.AddBankTransaction(
                BankTransactionInfo.ForError(Context.PaymentCards.First().Id, DefaultBankTransactionId, amount, ""));

            posOperation.BankTransactionInfos.FirstOrDefault().SetProperty(nameof(BankTransactionInfo.DateCreated),
                posOperation.BankTransactionInfos.FirstOrDefault().DateCreated.AddHours(-1));
            
            Context.SaveChanges();

            EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
                posOperationId: DefaultPosOperationId, isSucceeded: true);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_OneUnpaidPosOperationWithPendingPaymentStatusIsGivenAndActivePaymentSystemIsNotNull_ShouldReturnBadRequestObjectResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            
            SeedUnpaidPosOperation();

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);

            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            savedPosOperations.ForEach(po => _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, po).GetAwaiter().GetResult());
           
            var amount = savedPosOperations.Sum(po => po.CheckItems.Sum(ci => ci.Price));
            
            var posOperation = Context.PosOperations.First(po => po.Id == DefaultPosOperationId);

            posOperation.AddBankTransaction(
                BankTransactionInfo.ForError(Context.PaymentCards.First().Id, DefaultBankTransactionId, amount, ""));
            
            posOperation.BankTransactionInfos.FirstOrDefault().SetProperty(nameof(BankTransactionInfo.DateCreated),
                posOperation.BankTransactionInfos.FirstOrDefault().DateCreated.AddHours(-1));
            
            posOperation.MarkAsPendingPayment();

            Context.SaveChanges();

            EnsurePurchaseCompletionResultIsIncorrectAndHaveExpectedBadRequestObjectResult(
                posOperationId: DefaultPosOperationId, errorMessage: NotPossibleToPayErrorMessage);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_OneUnpaidPosOperationWithIncorrectStatusIsGivenAndActivePaymentSystemIsNotNull_ShouldReturnBadRequestObjectResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);
            
            SeedUnpaidPosOperation();

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);

            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            savedPosOperations.ForEach(po => _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, po).GetAwaiter().GetResult());

            var amount = savedPosOperations.Sum(po => po.CheckItems.Sum(ci => ci.Price));

            var posOperation = Context.PosOperations.First(po => po.Id == DefaultPosOperationId);

            posOperation.AddBankTransaction(
                BankTransactionInfo.ForError(Context.PaymentCards.First().Id, DefaultBankTransactionId, amount, ""));

            posOperation.BankTransactionInfos.FirstOrDefault().SetProperty(nameof(BankTransactionInfo.DateCreated),
                posOperation.BankTransactionInfos.FirstOrDefault().DateCreated.AddHours(-1));
            
            posOperation.SetProperty(nameof(PosOperation.Status), PosOperationStatus.Opened);

            Context.SaveChanges();

            EnsurePurchaseCompletionResultIsIncorrectAndHaveExpectedBadRequestObjectResult(
                posOperationId: DefaultPosOperationId, errorMessage: InternalServerErrorMessage);
        }

        [Test]
        public void PayCheck_OneUnpaidPosOperationIsGivenAndPaymentCardThatDoesNotBelongToTheUser_ShouldReturnBadRequestObjectResult()
        {
            SeedUnpaidPosOperation();

            SetActivePaymentCardForDefaultUser(DefaultVisaCardId);

            EnsurePurchaseCompletionResultIsIncorrectAndHaveExpectedBadRequestResult(
                posOperationId: DefaultPosOperationId, errorMessage: PaymentCardNotFoundErrorMessage, paymentCardId: DefaultMasterCardId);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PayCheck_OneUnpaidPosOperationIsGivenAndPaymentCard_ShouldReturnOkObjectResult(
            bool useNewPaymentSystem)
        {
            MarkPosToUseNewPaymentSystemIfItIsNeeded(useNewPaymentSystem);

            SeedUnpaidPosOperation();
            Seeder.Seed(new Collection<PaymentCard>
            {
                CreatePaymentCard(DefaultUserId)
            });

            SetActivePaymentCardForDefaultUser(DefaultMasterCardId);

            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            savedPosOperations.ForEach(po => _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, po).GetAwaiter().GetResult());

            var amount = savedPosOperations.Sum(po => po.CheckItems.Sum(ci => ci.Price));

            var posOperation = Context.PosOperations.First(po => po.Id == DefaultPosOperationId);
            posOperation.AddBankTransaction(
                BankTransactionInfo.ForError(Context.PaymentCards.First().Id, DefaultBankTransactionId, amount, ""));

            posOperation.BankTransactionInfos.FirstOrDefault().SetProperty(nameof(BankTransactionInfo.DateCreated),
                posOperation.BankTransactionInfos.FirstOrDefault().DateCreated.AddHours(-1));

            Context.SaveChanges();

            EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
                posOperationId: DefaultPosOperationId, isSucceeded: true, paymentCardId: DefaultVisaCardId);
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        public void GetAllAsync_UserHasPosOperations_ShouldReturnListOfSimpleChecks(int checksWithPaymentErrorCount,
            int allChecksCount)
        {
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            var errorMessages = new List<string> { TimeForPaymentHasNotArrivedErrorMessage, IncorrectCheckFormat, NotEnoughMoney };
            for (int i = 0; i < checksWithPaymentErrorCount; i++)
            {
                Seeder.Seed(
                    new IncompletePosOperationsDataSet(posId: DefaultPosId, userId: DefaultUserId)
                    .Select(po =>
                    {
                        po.MarkAsPendingCompletion();
                        po.MarkAsPendingCheckCreation();
                        po.MarkAsCompletedAndRememberDate();
                        if (!string.IsNullOrEmpty(errorMessages.ElementAt(i)))
                        {
                            po.AddBankTransaction(
                                BankTransactionInfo.ForError(DefaultVisaCardId, DefaultBankTransactionId, DefaultAmount,
                                    errorMessages.ElementAt(i)));
                        }
                        return po;
                    })
                    .ToArray()
                );

                var bankTransactionInfo = Context.BankTransactionInfos.FirstOrDefault(po =>
                   po.Comment.Equals(errorMessages.ElementAt(i)));

                bankTransactionInfo.Should().NotBeNull();

                SeedUnpaidCheckItems(bankTransactionInfo.PosOperationId);
            }
            
            SeedPaidPosOperation();
            Seeder.Seed(new Collection<PaymentCard>
            {
                CreatePaymentCard(DefaultUserId)
            });

            EnsureGetAllAsyncHaveExpectedResultAndHaveExpectedCheck(checksWithPaymentErrorCount,
                allChecksCount,
                errorMessages);
        }

        private void EnsureGetAllAsyncHaveExpectedResultAndHaveExpectedCheck(int checksWithPaymentErrorCount, 
            int allChecksCount,
            List<string> paymentErrors)
        {
            var result = _checksController.GetAllAsync().GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var checksResult = objectResult?.Value as ImmutableList<SimpleCheckDto>;

            checksResult.Should().NotBeNull();

            checksResult.Count.Should().Be(allChecksCount);

            var unpaidChecks = checksResult.Where(c => c.DatePaid == null).OrderBy(c => c.Id);
            unpaidChecks.Count().Should().Be(checksWithPaymentErrorCount);

            unpaidChecks.Should().NotBeNull();

            for (int i = 0; i < checksWithPaymentErrorCount; i++)
            {
                unpaidChecks.ElementAt(i).PaymentError.Should().NotBeNull();
                unpaidChecks.ElementAt(i).PaymentError.Message.Should().Be(paymentErrors.ElementAt(i));
            }
        }

        private void CreateCompletedPosOperationAndCheckItemsWithStatus(int posOperationId = SecondPosOperationId, 
            CheckItemStatus checkItemStatus = CheckItemStatus.Unpaid)
        {
            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsCompleted()
                .Build();

            Context.PosOperations.Add(posOperation);
            Context.SaveChanges();

            Context.CheckItems.AddRange(

               CheckItem.NewBuilder(DefaultPosId, posOperationId, DefaultGoodId, DefaultLabeledGoodId, DefaultCurrencyId)
                    .SetPrice(DefaultPrice)
                    .SetStatus(checkItemStatus)
                    .Build(),

               CheckItem.NewBuilder(DefaultPosId, posOperationId, DefaultGoodId, DefaultLabeledGoodId, DefaultCurrencyId)
                   .SetPrice(DefaultPrice)
                   .SetStatus(checkItemStatus)
                   .Build()
            );

            Context.SaveChanges();

            var savedPosOperations = Context.PosOperations.Select(po => po).ToImmutableList();
            savedPosOperations.ForEach(po => _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, po).GetAwaiter().GetResult());
        }

        private void EnsurePurchaseCompletionResultsAreCorrectHaveExpectedResultsCountAndHaveExpectedResultsStatus(
            int expectedResultsCount, PurchaseCompletionStatus expectedResultsStatus)
        {
            var result = _checksController.PayForAllChecksAsync().GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var purchaseCompletionResults = objectResult?.Value as List<PurchaseCompletionResultDto>;

            purchaseCompletionResults.Count.Should().Be(expectedResultsCount);

            purchaseCompletionResults.ForEach(pcr => pcr.Status.Should().Be(expectedResultsStatus));
        }

        private void EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
            int posOperationId, int nextCheckId, bool isSucceeded, int? paymentCardId = null)
        {
            var purchaseCompletionResult = PayCheckAndGetUnpaidPurchaseCompletionResult(posOperationId, paymentCardId);

            purchaseCompletionResult.Should().NotBeNull();

            purchaseCompletionResult.PaymentError.Should().BeNull();
            purchaseCompletionResult.Success.Should().Be(isSucceeded);
            purchaseCompletionResult.NextCheck.Should().NotBeNull();
            purchaseCompletionResult.NextCheck.Id.Should().Be(nextCheckId);
        }

        private void EnsurePurchaseCompletionResultIsCorrectAndHaveExpectedResult(
            int posOperationId, bool isSucceeded, int? paymentCardId = null)
        {
            var purchaseCompletionResult = PayCheckAndGetUnpaidPurchaseCompletionResult(posOperationId, paymentCardId);

            purchaseCompletionResult.Should().NotBeNull();

            purchaseCompletionResult.PaymentError.Should().BeNull();
            purchaseCompletionResult.Success.Should().Be(isSucceeded);
            purchaseCompletionResult.NextCheck.Should().BeNull();
        }

        private UnpaidPurchaseCompletionResult PayCheckAndGetUnpaidPurchaseCompletionResult(int posOperationId, int? paymentCardId = null)
        {
            var unpaidCheck = new UnpaidCheckInfo
            {
                PosOperationId = posOperationId,
                PaymentCardId = paymentCardId
            };

            var result = _checksController.PayCheckAsync(unpaidCheck).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            return objectResult?.Value as UnpaidPurchaseCompletionResult;
        }

        private void EnsurePurchaseCompletionResultIsIncorrectAndHaveExpectedBadRequestResult(
            int posOperationId, string errorMessage, int? paymentCardId = null)
        {
            var unpaidCheck = new UnpaidCheckInfo
            {
                PosOperationId = posOperationId,
                PaymentCardId = paymentCardId
            };
            var result = _checksController.PayCheckAsync(unpaidCheck).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            var purchaseCompletionResult = objectResult?.Value as string;

            purchaseCompletionResult.Should().NotBeNull();
            purchaseCompletionResult.Should().Be(errorMessage);
        }

        private void EnsurePurchaseCompletionResultIsIncorrectAndHaveExpectedBadRequestObjectResult(
            int posOperationId, string errorMessage)
        {
            var unpaidCheck = new UnpaidCheckInfo {PosOperationId = posOperationId};
            var result = _checksController.PayCheckAsync(unpaidCheck).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            var purchaseCompletionResult = objectResult?.Value as UnpaidPurchaseCompletionResult;

            purchaseCompletionResult.Should().NotBeNull();
            purchaseCompletionResult.Success.Should().BeFalse();
            purchaseCompletionResult.PaymentError.Should().NotBeNull();
            purchaseCompletionResult.PaymentError.Message.Should().Be(errorMessage);
            //TODO: Нурсултан, исправь тесты, допиши тесты на возврат чеков с ошибками оплаты
            //purchaseCompletionResult.PaymentError.PaymentDate.Should().NotBeNull();
        }

        

        private void EnsureFirstUnpaidCheckIsCorrectHaveExpectedResultAndHaveExpectedCheck()
        {
            var result = _checksController.GetFirstUnpaidByUserAsync().GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var firstUnpaidCheckResult = objectResult?.Value as SimpleCheckDto;

            firstUnpaidCheckResult.Should().NotBeNull();

            firstUnpaidCheckResult.Id.Should().Be(DefaultPosOperationId);
        }

        private void EnsureFirstUnpaidCheckReturnedNotFoundResult()
        {
            var result = _checksController.GetFirstUnpaidByUserAsync().GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        private void SetActivePaymentCardForDefaultUser(int paymentCardId)
        {
            var firstUser = Context.Users
                .Include(u => u.PaymentCards)
                .Include(u => u.BonusPoints)
                .Single(u => u.Id == DefaultUserId);
            firstUser.SetActivePaymentCard(Context.PaymentCards.First(pc => pc.Id == paymentCardId).Id);

            Context.SaveChanges();
        }

        private void SeedUnpaidPosOperation(bool isNeedSeedLabeledGood = true)
        {
            Seeder.Seed(new IncompletePosOperationsDataSet(posId: DefaultPosId, userId: DefaultUserId)
                .Select(po =>
                {
                    po.MarkAsPendingCompletion();
                    po.MarkAsPendingCheckCreation();
                    po.MarkAsCompletedAndRememberDate();
                    return po;
                })
                .ToArray()
            );

            if (isNeedSeedLabeledGood)
                Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            SeedUnpaidCheckItems();
        }

        private void SeedUnpaidCheckItems(int posOperationId = DefaultPosOperationId)
        {
            var checkItem = CheckItem.NewBuilder(DefaultPosId, posOperationId, DefaultGoodId, DefaultLabeledGoodId, DefaultCurrencyId)
                .SetPrice(DefaultPrice)
                .SetStatus(CheckItemStatus.Unpaid)
                .Build();

            Context.CheckItems.Add(checkItem);

            Context.SaveChanges();
        }

        private void SeedPaidPosOperation()
        {
            var posOperation = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForNoPayment(DefaultUserId))
                .Build();

            Seeder.Seed(new Collection<PosOperation>
            {
                posOperation
            });

            var checkItem = CheckItem.NewBuilder(DefaultPosId, posOperation.Id, DefaultGoodId, DefaultLabeledGoodId, DefaultCurrencyId)
                .SetPrice(DefaultPrice)
                .SetStatus(CheckItemStatus.Paid)
                .Build();

            Seeder.Seed(new Collection<CheckItem>
            {
                checkItem
            });
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

        private PaymentCard CreatePaymentCard(int userId)
        {
            var paymentCard = new PaymentCard(
                userId,
                new ExtendedPaymentCardInfo(
                    new PaymentCardNumber("555555", "4444"),
                    DateTime.UtcNow,
                    "2F725BBD1F405A1ED0336ABAF85DDFEB6902A9984A76FD877C3B5CC3B5085A82"
                )
            );

            paymentCard.MarkAsAbleToMakePayment();

            return paymentCard;
        }
    }
}
