using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement;
using NasladdinPlace.Dtos.Purchase;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;

namespace NasladdinPlace.Api.Tests.Scenarios.Shopping
{
    public class PurchaseInitiationScenario : TestsBase
    {
        private PointsOfSaleController _pointsOfSaleController;

        private Mock<INasladdinWebSocketMessageSender> _mockWebSocketMessageSender;
        private Mock<ITelegramChannelMessageSender> _mockTelegramChannelMessageSender;

        private IServiceProvider _serviceProvider;

        private IStatelessPosTokenManager _statelessPosTokenManager;

        [SetUp]
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

            var pointsOfSales = Context.PointsOfSale.ToList();
            pointsOfSales.ForEach(p => p.UpdateAllowedModes(new List<PosMode> { PosMode.Purchase }));
            Context.SaveChanges();

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            _serviceProvider = TestServiceProviderFactory.Create();

            _mockWebSocketMessageSender = _serviceProvider.GetRequiredService<Mock<INasladdinWebSocketMessageSender>>();

            _mockTelegramChannelMessageSender =
                _serviceProvider.GetRequiredService<Mock<ITelegramChannelMessageSender>>();

            _mockTelegramChannelMessageSender
                .Setup(s => s.SendAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _statelessPosTokenManager = _serviceProvider.GetRequiredService<IStatelessPosTokenManager>();

            _pointsOfSaleController = _serviceProvider.GetRequiredService<PointsOfSaleController>();

            var mockPosRealTimeInfo = _serviceProvider.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>();
            var posIds = new[] { GetFirstPosId(), GetSecondPosId() };

            foreach (var posId in posIds)
            {
                mockPosRealTimeInfo.Setup(p => p.GetOrAddById(posId))
                    .Returns(new PosRealTimeInfo(posId));
            }

            AuthorizeUser(GetFirstUserId());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void InitiatePurchase_IncorrectQrCodeIsGiven_ShouldNotCreateOperationAndSendOpenDoorRequest(bool isLeftDoor)
        {
            var isOpenDoorRequestSent = false;

            _mockWebSocketMessageSender
                .Setup(s => s.RequestPosOperationInitiationAsync(It.IsAny<int>(), It.IsAny<PosMode>(), PosDoorPosition.Left))
                .Returns(() =>
                {
                    isOpenDoorRequestSent = isLeftDoor;
                    return Task.CompletedTask;
                });

            _mockWebSocketMessageSender
                .Setup(s => s.RequestPosOperationInitiationAsync(It.IsAny<int>(), It.IsAny<PosMode>(), PosDoorPosition.Right))
                .Returns(() =>
                {
                    isOpenDoorRequestSent = !isLeftDoor;
                    return Task.CompletedTask;
                });

            var result = _pointsOfSaleController.InitiatePurchaseAsync(new PurchaseInitiationRequestDto
            {
                QrCode = "INCORRECT_QR_CODE"
            }).Result;
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));

            Context.PosOperations.ToList().Should().HaveCount(0);
            isOpenDoorRequestSent.Should().BeFalse();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void InitiatePurchaseAndRequestCompletion_CorrectQrCodeIsGiven_ShouldCreateOperationAndSendOpenDoorRequestThenSendCloseDoorsRequest(bool isLeftDoor)
        {
            AuthorizeUser(GetFirstUserId());

            InitiatePurchase_CorrectQrCodeIsGiven_ShouldCreateOperationAndSendOpenDoorRequest();
            InitiatePurchaseCompletionAndCheckPurchaseCompletionRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: GetFirstPosId(),
                shouldPurchaseCompletionRequestBeSent: true
            );
        }

        [Test]
        public void
            InitiateThenContinueThenRequestCompletionOfPurchase_CorrectQrCodeIsGiven_ShouldCreateOperationAndSendOpenDoorRequestThenSendOpenAnotherDoorRequestThenSendCloseDoorsRequest()
        {
            AuthorizeUser(GetFirstUserId());

            InitiatePurchase_CorrectQrCodeIsGiven_ShouldCreateOperationAndSendOpenDoorRequest();
            ContinuePurchaseAndCheckPurchaseContinuationRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: GetFirstPosId(),
                shouldPurchaseContinuationRequestBeDelivered: true
            );
            InitiatePurchaseCompletionAndCheckPurchaseCompletionRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: GetFirstPosId(),
                shouldPurchaseCompletionRequestBeSent: true
            );
        }

        [Test]
        public void ContinuePurchase_SuitableOperationIsNotGiven_ShouldReturnOkAndNotSendDoorOpeningRequest()
        {
            AuthorizeUser(GetFirstUserId());

            ContinuePurchaseAndCheckPurchaseContinuationRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: GetFirstPosId(),
                shouldPurchaseContinuationRequestBeDelivered: false
            );
        }

        [Test]
        public void InitiatePurchaseCompletion_SuitableOperationIsNotGiven_ShouldReturnOkAndNotSendDoorClosingRequest()
        {
            AuthorizeUser(GetFirstUserId());

            InitiatePurchaseCompletionAndCheckPurchaseCompletionRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: GetFirstPosId(),
                shouldPurchaseCompletionRequestBeSent: false
            );
        }

        [Test]
        public void
            InitiatePurchaseAndRequestCompletionInParallel_CorrectQrCodeIsGiven_ShouldReturnOkAndSendDoorClosingRequest()
        {
            AuthorizeUser(GetFirstUserId());

            InitiatePurchase_CorrectQrCodeIsGiven_ShouldCreateOperationAndSendOpenDoorRequest();

            var isCloseDoorsRequestSent = false;

            _mockWebSocketMessageSender
                .Setup(s => s.RequestPosOperationCompletionAsync(It.IsAny<int>()))
                .Returns(() =>
                {
                    isCloseDoorsRequestSent = true;
                    return Task.CompletedTask;
                });

            for (var threadNumber = 0; threadNumber < 5; ++threadNumber)
            {
                Task.Run(() =>
                {
                    var result = _pointsOfSaleController.InitiatePurchaseCompletion();
                    result.Should().BeOfType<OkResult>();
                });
            }

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            isCloseDoorsRequestSent.Should().BeTrue();

            Context.PosOperations.AsNoTracking().First().CompletionInitiationDate.Should().NotBeNull();
        }

        [Test]
        public void InitiatePurchase_IncompletePosOperationWithOtherUserIsGiven_ShouldReturnBadRequest()
        {
            AuthorizeUser(GetSecondUserId());

            CreateIncompletePosOperationForUserAndPos(GetFirstUserId(), GetFirstPosId());

            var doorOpeningResult = InitiatePurchaseUsingQrCode(GetFirstPosQrCode());

            doorOpeningResult.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public void InitiatePurchase_IncompletePosOperationWithOtherUserIsGiven_ShouldSendAlertToTelegram()
        {
            AuthorizeUser(GetSecondUserId());

            CreateIncompletePosOperationForUserAndPos(GetFirstUserId(), GetFirstPosId());

            _mockWebSocketMessageSender
                .Setup(s => s.RequestPosAccountingBalancesAsync(GetFirstPosId()))
                .Returns(Task.CompletedTask);

            var wasNotificationSentToTelegram = false;

            _mockTelegramChannelMessageSender
                .Setup(s => s.SendAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Callback(() => wasNotificationSentToTelegram = true);

            InitiatePurchaseUsingQrCode(GetFirstPosQrCode());

            wasNotificationSentToTelegram.Should().BeTrue();
        }

        [Test]
        public void InitiatePurchase_IncompletePosOperationWithOtherUserIsGiven_ShouldRequestPosContent()
        {
            AuthorizeUser(GetSecondUserId());

            CreateIncompletePosOperationForUserAndPos(GetFirstUserId(), GetFirstPosId());

            var wasPosContentRequested = false;

            _mockWebSocketMessageSender
                .Setup(s => s.RequestPosAccountingBalancesAsync(GetFirstPosId()))
                .Returns(Task.CompletedTask)
                .Callback(() => wasPosContentRequested = true);

            InitiatePurchaseUsingQrCode(GetFirstPosQrCode());

            wasPosContentRequested.Should().BeTrue();
        }

        [Test]
        public void InitiatePurchase_IncompletePosOperationWithSameUserIsGiven_ShouldReturnOk()
        {
            AuthorizeUser(GetFirstUserId());

            CreateIncompletePosOperationForUserAndPos(GetFirstUserId(), GetFirstPosId());

            var openingDoorResult = InitiatePurchaseUsingQrCode(GetFirstPosQrCode());

            openingDoorResult.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void InitiatePurchase_IncompletePosOperationWithSameUserIsGiven_ShouldNotRequestPosContent()
        {
            AuthorizeUser(GetFirstUserId());

            CreateIncompletePosOperationForUserAndPos(GetFirstUserId(), GetFirstPosId());

            var wasPosContentRequested = false;

            _mockWebSocketMessageSender
                .Setup(s => s.RequestPosAccountingBalancesAsync(GetFirstPosId()))
                .Returns(Task.CompletedTask)
                .Callback(() => wasPosContentRequested = true);

            InitiatePurchaseUsingQrCode(GetFirstPosQrCode());

            wasPosContentRequested.Should().BeFalse();
        }

        [Test]
        public void InitiatePurchase_IncompletePosOperationWithSameUserIsGiven_ShouldNotSendAlertToTelegram()
        {
            AuthorizeUser(GetFirstUserId());

            CreateIncompletePosOperationForUserAndPos(GetFirstUserId(), GetFirstPosId());

            var wasNotificationSentToTelegram = false;

            _mockTelegramChannelMessageSender
                .Setup(s => s.SendAsync(It.IsAny<string>()))
                .Callback(() => wasNotificationSentToTelegram = true);

            InitiatePurchaseUsingQrCode(GetFirstPosQrCode());

            wasNotificationSentToTelegram.Should().BeFalse();
        }

        [Test]
        public void InitiatePurchase_IncompletePosOperationWithSameUserIsGiven_ShouldNotCreateExtraOperation()
        {
            AuthorizeUser(GetFirstUserId());
            CreateIncompletePosOperationForUserAndPos(GetFirstUserId(), GetFirstPosId());

            InitiatePurchaseUsingQrCode(GetFirstPosQrCode());

            Context.PosOperations.ToList().Should().HaveCount(1);
        }

        [Test]
        public void InitiatePurchase_TwoUsersAtDifferentPointsOfSaleAreGiven_ShouldCreateTwoOperations()
        {
            var purchaseInitiationData = new List<(int UserId, string QrCode)>
            {
                (GetFirstUserId(), GetFirstPosQrCode()),
                (GetSecondUserId(), GetSecondPosQrCode())
            };

            var purchaseInitiationTasks = new List<Task>();
            foreach (var (userId, qrCode) in purchaseInitiationData)
            {
                var purchaseInitiationTask = Task.Run(() =>
                {
                    var firstPointsOfSaleController = _serviceProvider.GetRequiredService<PointsOfSaleController>();
                    AuthorizeUser(firstPointsOfSaleController, userId);
                    InitiatePurchaseUsingQrCode(qrCode);
                });
                purchaseInitiationTasks.Add(purchaseInitiationTask);
            }

            Task.WhenAll(purchaseInitiationTasks).Wait();

            Context.PosOperations.AsNoTracking().Count().Should().Be(2);
        }

        [Test]
        public void
            ContinuePurchase_IncompletePosOperationsForTwoUsersAtDifferentPointsOfSaleAreGiven_ShouldSendPurchaseContinuationRequestTwice()
        {
            var purchaseContinuationData = new List<(int UserId, int PosId)>
            {
                (GetFirstUserId(), GetFirstPosId()),
                (GetSecondUserId(), GetSecondPosId())
            };

            foreach (var (userId, posId) in purchaseContinuationData)
            {
                CreateIncompletePosOperationForUserAndPos(userId, posId);
            }

            var purchaseContinuationTasks = new List<Task>();

            foreach (var (userId, posId) in purchaseContinuationData)
            {
                var purchaseContinuationTask = Task.Run(() =>
                {
                    var pointsOfSaleController = _serviceProvider.GetRequiredService<PointsOfSaleController>();
                    AuthorizeUser(pointsOfSaleController, userId);
                    ContinuePurchaseAndCheckPurchaseContinuationRequestDeliveryStatus(
                        pointsOfSaleController: pointsOfSaleController,
                        givenPosId: posId,
                        shouldPurchaseContinuationRequestBeDelivered: true
                    );
                });
                purchaseContinuationTasks.Add(purchaseContinuationTask);
            }

            Task.WhenAll(purchaseContinuationTasks).Wait();
        }

        [Test]
        public void
            InitiatePurchaseCompletion_IncompletePosOperationsForTwoUsersAtDifferentPointsOfSaleAreGiven_ShouldSendPurchaseCompletionRequestTwice()
        {
            var purchaseInitiationCompletionData = new List<(int UserId, int PosId)>
            {
                (GetFirstUserId(), GetFirstPosId()),
                (GetSecondUserId(), GetSecondPosId())
            };

            foreach (var (userId, posId) in purchaseInitiationCompletionData)
            {
                CreateIncompletePosOperationForUserAndPos(userId, posId);
            }

            var purchaseInitiationCompletionTasks = new List<Task>();
            foreach (var (userId, posId) in purchaseInitiationCompletionData)
            {
                var purchaseInitiationCompletionTask = Task.Run(() =>
                {
                    var pointsOfSaleController = _serviceProvider.GetRequiredService<PointsOfSaleController>();
                    AuthorizeUser(pointsOfSaleController, userId);
                    InitiatePurchaseCompletionAndCheckPurchaseCompletionRequestDeliveryStatus(
                        pointsOfSaleController,
                        posId,
                        shouldPurchaseCompletionRequestBeSent: true
                    );
                });
                purchaseInitiationCompletionTasks.Add(purchaseInitiationCompletionTask);
            }

            Task.WhenAll(purchaseInitiationCompletionTasks).Wait();

            foreach (var (userId, posId) in purchaseInitiationCompletionData)
            {
                EnsurePosOperationDateSentForVerificationIsSingleAndRecent(posId, userId);
            }
        }

        [Test]
        public void
            InitiatePurchaseCompletion_IncompletePosOperationIsGivenAndUserIsTryingToInitiatePurchaseCompletionAgainAfterMinWaitInterval_ShouldSendPurchaseCompletionRequest()
        {
            var userId = GetFirstUserId();
            var posId = GetFirstPosId();

            CreateIncompletePosOperationForUserAndPos(userId, posId);

            InitiatePurchaseCompletionAndCheckPurchaseCompletionRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: posId,
                shouldPurchaseCompletionRequestBeSent: true
            );

            Task.Delay(TimeSpan.FromSeconds(10)).Wait();

            InitiatePurchaseCompletionAndCheckPurchaseCompletionRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: posId,
                shouldPurchaseCompletionRequestBeSent: true
            );
        }

        [Test]
        public void
            InitiatePurchaseCompletion_IncompletePosOperationIsGivenAndUserIsTryingToInitiatePurchaseCompletionAgainBeforeMinWaitInterval_ShouldNotSendPurchaseCompletionRequest()
        {
            var userId = GetFirstUserId();
            var posId = GetFirstPosId();

            CreateIncompletePosOperationForUserAndPos(userId, posId);

            InitiatePurchaseCompletionAndCheckPurchaseCompletionRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: posId,
                shouldPurchaseCompletionRequestBeSent: true
            );

            InitiatePurchaseCompletionAndCheckPurchaseCompletionRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: posId,
                shouldPurchaseCompletionRequestBeSent: false
            );
        }

        [Test]
        public void
            ContinuePurchase_IncompletePosOperationIsGivenAndUserIsTryingToContinuePurchaseAgainAfterMinWaitInterval_ShouldSendPurchaseContinuationRequest()
        {
            CreateIncompletePosOperationForUserAndPos(GetFirstUserId(), GetFirstPosId());
            ContinuePurchaseAndCheckPurchaseContinuationRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: GetFirstPosId(),
                shouldPurchaseContinuationRequestBeDelivered: true
            );

            Task.Delay(TimeSpan.FromSeconds(5)).Wait();

            ContinuePurchaseAndCheckPurchaseContinuationRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: GetFirstPosId(),
                shouldPurchaseContinuationRequestBeDelivered: true
            );
        }

        [Test]
        public void
            ContinuePurchase_IncompletePosOperationIsGivenAndUserIsTryingToContinuePurchaseAgainBeforeMinWaitInterval_ShouldNotSendPurchaseContinuationRequest()
        {
            var firstUserId = GetFirstUserId();
            var firstPosId = GetFirstPosId();

            CreateIncompletePosOperationForUserAndPos(firstUserId, firstPosId);

            ContinuePurchaseAndCheckPurchaseContinuationRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: firstPosId,
                shouldPurchaseContinuationRequestBeDelivered: true
            );

            ContinuePurchaseAndCheckPurchaseContinuationRequestDeliveryStatus(
                pointsOfSaleController: _pointsOfSaleController,
                givenPosId: firstPosId,
                shouldPurchaseContinuationRequestBeDelivered: false
            );
        }

        private void InitiatePurchase_CorrectQrCodeIsGiven_ShouldCreateOperationAndSendOpenDoorRequest()
        {
            var isOpenDoorRequestSent = false;

            _mockWebSocketMessageSender
                .Setup(s => s.RequestPosOperationInitiationAsync(
                    It.IsAny<int>(),
                    It.IsAny<PosMode>(),
                    It.Is<PosDoorPosition>(value => value == PosDoorPosition.Left))
                )
                .Returns(() =>
                {
                    isOpenDoorRequestSent = true;
                    return Task.CompletedTask;
                });

            var correctPosQrCode = GetFirstPosQrCode();

            var result = _pointsOfSaleController.InitiatePurchaseAsync(new PurchaseInitiationRequestDto
            {
                QrCode = correctPosQrCode
            }).Result;
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var posOperations = Context.PosOperations
                .Include(po => po.Pos)
                .ToList();
            posOperations.Should().HaveCount(1);
            var posOperation = posOperations.First();

            posOperation.DateStarted.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            posOperation.DateCompleted.Should().BeNull();
            posOperation.UserId.Should().Be(GetFirstUserId());
            posOperation.CompletionInitiationDate.Should().BeNull();
            isOpenDoorRequestSent.Should().BeTrue();
        }

        private void InitiatePurchaseCompletionAndCheckPurchaseCompletionRequestDeliveryStatus(
            PointsOfSaleController pointsOfSaleController,
            int givenPosId,
            bool shouldPurchaseCompletionRequestBeSent)
        {
            var closeDoorsRequestWaitHandle = new AutoResetEvent(false);

            _mockWebSocketMessageSender
                .Setup(s => s.RequestPosOperationCompletionAsync(givenPosId))
                .Returns(() =>
                {
                    closeDoorsRequestWaitHandle.Set();
                    return Task.CompletedTask;
                });

            var result = pointsOfSaleController.InitiatePurchaseCompletion();
            result.Should().BeOfType<OkResult>();

            closeDoorsRequestWaitHandle
                .WaitOne(TimeSpan.FromSeconds(5))
                .Should()
                .Be(shouldPurchaseCompletionRequestBeSent);
        }

        private void ContinuePurchaseAndCheckPurchaseContinuationRequestDeliveryStatus(
            PointsOfSaleController pointsOfSaleController,
            int givenPosId,
            bool shouldPurchaseContinuationRequestBeDelivered)
        {
            var purchaseContinuationRequestWaitHandle = new AutoResetEvent(false);

            _mockWebSocketMessageSender
                .Setup(s => s.RequestPosOperationContinuationAsync(givenPosId))
                .Returns(() =>
                {
                    purchaseContinuationRequestWaitHandle.Set();
                    return Task.CompletedTask;
                });

            var result = pointsOfSaleController.ContinuePurchase();
            result.Should().BeOfType<OkResult>();

            purchaseContinuationRequestWaitHandle
                .WaitOne(TimeSpan.FromSeconds(5))
                .Should()
                .Be(shouldPurchaseContinuationRequestBeDelivered);
        }

        private void EnsurePosOperationDateSentForVerificationIsSingleAndRecent(int posId, int userId)
        {
            var posOperations = Context.PosOperations
                .AsNoTracking()
                .Where(po => po.UserId == userId && po.PosId == posId)
                .ToImmutableList();

            posOperations.Count.Should().Be(1);

            posOperations.First().CompletionInitiationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }

        private IActionResult InitiatePurchaseUsingQrCode(string qrCode)
        {
            var posDoorOpeningRequest = new PurchaseInitiationRequestDto
            {
                QrCode = qrCode
            };

            return _pointsOfSaleController.InitiatePurchaseAsync(posDoorOpeningRequest).Result;
        }

        private void CreateIncompletePosOperationForUserAndPos(int userId, int posId)
        {
            var posOperation = PosOperation.NewOfUserAndPosBuilder(userId, posId).Build();
            Context.PosOperations.Add(posOperation);
            Context.SaveChanges();
        }

        private int GetFirstUserId()
        {
            return GetFirstUser().Id;
        }

        private int GetSecondUserId()
        {
            return GetUsersSortedById().Skip(1).First().Id;
        }

        private ApplicationUser GetFirstUser()
        {
            return GetUsersSortedById().First();
        }

        private IQueryable<ApplicationUser> GetUsersSortedById()
        {
            return Context.Users.OrderBy(u => u.Id);
        }

        private string GetFirstPosQrCode()
        {
            return _statelessPosTokenManager.GeneratePosToken(GetFirstPosId());
        }

        private string GetSecondPosQrCode()
        {
            return _statelessPosTokenManager.GeneratePosToken(GetSecondPosId());
        }

        private int GetFirstPosId()
        {
            return GetFirstPos().Id;
        }

        private Pos GetFirstPos()
        {
            return GetPointsOfSaleSortedById().First();
        }

        private Pos GetSecondPos()
        {
            return GetPointsOfSaleSortedById().Skip(1).First();
        }

        private int GetSecondPosId()
        {
            return GetSecondPos().Id;
        }

        private IQueryable<Pos> GetPointsOfSaleSortedById()
        {
            return Context.PointsOfSale.OrderBy(pos => pos.Id);
        }

        private void AuthorizeUser(int userId)
        {
            AuthorizeUser(_pointsOfSaleController, userId);
        }

        private static void AuthorizeUser(ControllerBase mvcController, int userId)
        {
            mvcController.ControllerContext = ControllerContextFactory.MakeForUserWithId(userId);
        }
    }
}