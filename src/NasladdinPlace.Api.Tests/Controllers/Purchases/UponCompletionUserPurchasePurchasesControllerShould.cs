using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.PurchaseCompletionResult;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Purchase.Completion.Models;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Pos.PosOperationTransactions.Contracts;

namespace NasladdinPlace.Api.Tests.Controllers.Purchases
{
    public class UponCompletionUserPurchasePurchasesControllerShould : TestsBase
    {
        private int? _defaultPosId;
        private int? _defaultGoodId;
        private int? _defaultLabeledGoodId;
        private int? _defaultCurrencyId;
        private PosOperation _latestPosOperation;
        private ApplicationUser _defaultUser;

        private IOperationTransactionManager _operationTransactionManager;
        private IUnitOfWork _unitOfWork;

        private int DefaultPosId
        {
            get
            {
                if (_defaultPosId == null)
                    _defaultPosId = Context.PointsOfSale.OrderBy(pos => pos.Id).First().Id;
                return _defaultPosId.Value;
            }
        }

        private ApplicationUser DefaultUser
        {
            get
            {
                if (_defaultUser == null)
                    _defaultUser = Context.Users.OrderBy(u => u.Id).First();
                return _defaultUser;
            }
        }

        private int DefaultUserId => DefaultUser.Id;

        private PosOperation LatestPosOperation
        {
            get
            {
                if (_latestPosOperation == null)
                    _latestPosOperation = Context.PosOperations
                        .Include(po => po.PosOperationTransactions)
                        .OrderBy(po => po.DateStarted)
                        .First();
                return _latestPosOperation;
            }
        }

        private int DefaultGoodId
        {
            get
            {
                if (_defaultGoodId == null)
                    _defaultGoodId = Context.Goods.OrderBy(g => g.Id).First().Id;
                return _defaultGoodId.Value;
            }
        }

        private int DefaultLabeledGoodId
        {
            get
            {
                if (_defaultLabeledGoodId == null)
                    _defaultLabeledGoodId = Context.LabeledGoods.OrderBy(lg => lg.Id).First().Id;
                return _defaultLabeledGoodId.Value;
            }
        }

        private int DefaultCurrencyId
        {
            get
            {
                if (_defaultCurrencyId == null)
                    _defaultCurrencyId = Context.Currencies.OrderBy(c => c.Id).First().Id;
                return _defaultCurrencyId.Value;
            }
        }

        private IServiceCollection DiServices { get; set; }

        private PurchasesController _purchasesController;

        public override void SetUp()
        {
            base.SetUp();

            _defaultPosId = null;
            _defaultGoodId = null;
            _defaultLabeledGoodId = null;
            _defaultCurrencyId = null;
            _latestPosOperation = null;
            _defaultUser = null;

            Mapper.Reset();
            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(posId: DefaultPosId, userId: DefaultUserId));
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            AssignDefaultUserAPaymentCard();

            DiServices =  TestServiceProviderFactory.CreateServiceCollection();
            var serviceProvider = DiServices.BuildServiceProvider();

            _purchasesController = serviceProvider.GetRequiredService<PurchasesController>();

            _purchasesController.ControllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);

            _operationTransactionManager = serviceProvider.GetRequiredService<IOperationTransactionManager>();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkFactory>().MakeUnitOfWork();
        }

        [Test]
        public void ReturnUnpaidPurchaseNotGivenStatusWhenCompletedUnpaidPosOperationContainsNoCheckItems()
        {
            MarkLatestPosOperationAsCompleted();
            var purchaseCompletionResult = CompleteUserPurchase();
            EnsurePurchaseCompletionActionResultContainsPurchaseCompletionResultDtoWithExpectedStatus(
                purchaseCompletionResult,
                PurchaseCompletionStatus.UnpaidPurchaseNotFound
            );
        }

        [Test]
        public void ReturnUnpaidPurchaseNotGivenStatusWhenPosOperationNotCompleted()
        {
            var purchaseCompletionResult = CompleteUserPurchase();
            EnsurePurchaseCompletionActionResultContainsPurchaseCompletionResultDtoWithExpectedStatus(
                purchaseCompletionResult,
                PurchaseCompletionStatus.UnpaidPurchaseNotFound
            );
        }

        [Test]
        public void ReturnUnpaidPurchaseNotGivenStatusWhenPosOperationAlreadyPaid()
        {
            AddCheckItemToLatestPosOperation(10M, CheckItemStatus.Paid);
            MarkLatestPosOperationAsCompleted();
            MarkLatestPosOperationAsPaid();
            var purchaseCompletionResult = CompleteUserPurchase();
            EnsurePurchaseCompletionActionResultContainsPurchaseCompletionResultDtoWithExpectedStatus(
                purchaseCompletionResult,
                PurchaseCompletionStatus.UnpaidPurchaseNotFound
            );
        }

        [Test]
        public void ReturnAlreadyPendingPaymentStatusWhenPosOperationHasAlreadyBeenInPendingPaymentState()
        {
            AddCheckItemToLatestPosOperation(10M, CheckItemStatus.Unpaid);
            MarkLatestPosOperationAsCompleted();
            MarkLatestPosOperationAsPendingPayment();
            var purchaseCompletionResult = CompleteUserPurchase();
            EnsurePurchaseCompletionActionResultContainsPurchaseCompletionResultDtoWithExpectedStatus(
                purchaseCompletionResult,
                PurchaseCompletionStatus.ProcessingPayment
            );
        }

        [Test]
        public void ReturnPaymentFailureStatusWhenUnableToConnectToPaymentSystem()
        {
            AddCheckItemToLatestPosOperation(10M, CheckItemStatus.Unpaid);
            CreateTransactionForLatestPosOperation();
            MarkLatestPosOperationAsCompleted();
            SimulatePaymentSystemNetworkErrorOnRecurrentPaymentRequest();
            var purchaseCompletionResult = CompleteUserPurchase();
            EnsurePurchaseCompletionActionResultContainsPurchaseCompletionResultDtoWithExpectedStatus(
                purchaseCompletionResult,
                PurchaseCompletionStatus.PaymentError
            );
        }

        [Test]
        public void ReturnPaymentFailureStatusWhenPaymentSystemUnableToProcessPayment()
        {
            AddCheckItemToLatestPosOperation(10M, CheckItemStatus.Unpaid);
            CreateTransactionForLatestPosOperation();
            MarkLatestPosOperationAsCompleted();
            SimulatePaymentSystemNotPaidResponseOnRecurrentPaymentRequest();
            var purchaseCompletionResult = CompleteUserPurchase();
            EnsurePurchaseCompletionActionResultContainsPurchaseCompletionResultDtoWithExpectedStatus(
                purchaseCompletionResult,
                PurchaseCompletionStatus.PaymentError
            );
        }

        [Test]
        public void ReturnSuccessStatusWhenPosOperationPaidSuccessfully()
        {
            AddCheckItemToLatestPosOperation(10M, CheckItemStatus.Unpaid);
            CreateTransactionForLatestPosOperation();
            MarkLatestPosOperationAsCompleted();
            var purchaseCompletionResult = CompleteUserPurchase();
            EnsurePurchaseCompletionActionResultContainsPurchaseCompletionResultDtoWithExpectedStatus(
                purchaseCompletionResult,
                PurchaseCompletionStatus.Success
            );
        }

        private void AddCheckItemToLatestPosOperation(decimal price, CheckItemStatus status)
        {
            var checkItem = CheckItem.NewBuilder(DefaultPosId, LatestPosOperation.Id, DefaultGoodId, DefaultLabeledGoodId, DefaultCurrencyId)
                .SetPrice(price)
                .SetStatus(status)
                .Build();

            Context.CheckItems.Add(checkItem);

            Context.SaveChanges();
        }

        private IActionResult CompleteUserPurchase()
        {
            return _purchasesController.CompleteUserPurchaseAsync().GetAwaiter().GetResult();
        }

        private void MarkLatestPosOperationAsCompleted()
        {
            LatestPosOperation.MarkAsPendingCompletion();
            LatestPosOperation.MarkAsPendingCheckCreation();
            LatestPosOperation.MarkAsCompletedAndRememberDate();
            Context.SaveChanges();
        }

        private void MarkLatestPosOperationAsPaid()
        {
            LatestPosOperation.MarkAsPendingPayment();
            LatestPosOperation.MarkAsPaid(OperationPaymentInfo.ForPaymentViaBonuses(DefaultUserId, 10M));
            Context.SaveChanges();
        }

        private void MarkLatestPosOperationAsPendingPayment()
        {
            LatestPosOperation.MarkAsPendingPayment();
            Context.SaveChanges();
        }

        private void CreateTransactionForLatestPosOperation()
        {
            _operationTransactionManager.CreateOperationTransactionAsync(_unitOfWork, LatestPosOperation).GetAwaiter().GetResult();
        }

        private void SimulatePaymentSystemNetworkErrorOnRecurrentPaymentRequest()
        {
            var networkErrorPaymentResponse = Response<PaymentResult>.Failure("Network error");

            ConfigureReturningPaymentServiceRecurrentPaymentRequestResponse(networkErrorPaymentResponse);
        }

        private void SimulatePaymentSystemNotPaidResponseOnRecurrentPaymentRequest()
        {
            var notPaidPaymentResult = PaymentResult.NotPaid(
                transactionId: 0,
                reason: "Payment error",
                localizedReason: "Ошибка с платежом."
            );
            var notPaidPaymentResponse = Response<PaymentResult>.Success(notPaidPaymentResult);

            ConfigureReturningPaymentServiceRecurrentPaymentRequestResponse(notPaidPaymentResponse);
        }

        private void ConfigureReturningPaymentServiceRecurrentPaymentRequestResponse(Response<PaymentResult> paymentResponse)
        {
            var mockPaymentService = new Mock<IPaymentService>();

            mockPaymentService.Setup(ps => ps.MakeRecurrentPaymentAsync(It.IsAny<RecurrentPaymentRequest>()))
                .Returns(Task.FromResult(paymentResponse));

            DiServices.AddTransient(sp => mockPaymentService.Object);

            var serviceProvider = DiServices.BuildServiceProvider();

            _purchasesController = serviceProvider.GetRequiredService<PurchasesController>();

            _purchasesController.ControllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);
        }

        private static void EnsurePurchaseCompletionActionResultContainsPurchaseCompletionResultDtoWithExpectedStatus(
            IActionResult purchaseCompletionActionResult, PurchaseCompletionStatus expectedStatus)
        {
            purchaseCompletionActionResult.Should().NotBeNull();
            var purchaseCompletionObjectResult = purchaseCompletionActionResult as ObjectResult;
            purchaseCompletionObjectResult.Should().NotBeNull();

            var purchaseCompletionResultDto = purchaseCompletionObjectResult.Value as PurchaseCompletionResultDto;
            purchaseCompletionResultDto.Should().NotBeNull();
            purchaseCompletionResultDto.Status.Should().Be(expectedStatus);
        }

        private void AssignDefaultUserAPaymentCard()
        {
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            var paymentCard = Context.PaymentCards.First(pc => pc.Id == DefaultUserId);
            DefaultUser.SetActivePaymentCard(paymentCard.Id);
            Context.SaveChanges();
        }
    }
}