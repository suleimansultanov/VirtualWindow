using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;
using NasladdinPlace.Dtos.Purchase;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NUnit.Framework;
using System.Linq;
using Moq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;

namespace NasladdinPlace.Api.Tests.Controllers.Purchases
{
    public class OnPurchaseInitiationPurchasesControllerShould : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;

        private PurchasesController _purchasesController;
        private IStatelessPosTokenManager _statelessPosTokenManager;

        public override void SetUp()
        {
            base.SetUp();

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

            var serviceProvider = TestServiceProviderFactory.Create();

            _purchasesController = serviceProvider.GetRequiredService<PurchasesController>();

            _statelessPosTokenManager = serviceProvider.GetRequiredService<IStatelessPosTokenManager>();

            _purchasesController.ControllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);
            var mockPosRealTimeInfo = serviceProvider.GetRequiredService<Mock<IPosRealTimeInfoDataStore>>();

            mockPosRealTimeInfo.Setup(p => p.GetOrAddById(DefaultPosId))
                .Returns(new PosRealTimeInfo(DefaultPosId));
        }

        [Test]
        public void
            InitiatePurchaseAsync_PosOperationInProcessOfCheckCreationIsGiven_ShouldReturnPurchaseNotAllowedResult()
        {
            MarkPosOperationAsPendingCompletion();
            InitiatePurchaseAndEnsurePurchaseInitiationResultHasStatus(PurchaseInitiationStatus.PurchaseNotAllowed);
        }

        [Test]
        public void
            InitiatePurchaseAsync_ActivePosOperationIsGiven_ShouldReturnSuccessResult()
        {
            InitiatePurchaseAndEnsurePurchaseInitiationResultHasStatus(PurchaseInitiationStatus.Success);
        }

        private void MarkPosOperationAsPendingCompletion()
        {
            var posOperation = Context.PosOperations.First();
            posOperation.MarkAsPendingCompletion();
            Context.SaveChanges();
        }

        private string GetDefaultQrCode()
        {
            return _statelessPosTokenManager.GeneratePosToken(DefaultPosId);
        }

        private void InitiatePurchaseAndEnsurePurchaseInitiationResultHasStatus(PurchaseInitiationStatus purchaseInitiationStatus)
        {
            var purchaseInitiationActionResult = _purchasesController.InitiatePurchaseAsync(new PurchaseInitiationRequestDto
            {
                QrCode = GetDefaultQrCode()
            }).Result;

            var purchaseInitiationObjectResult = purchaseInitiationActionResult as OkObjectResult;
            purchaseInitiationObjectResult.Should().NotBeNull();

            var purchaseInitiationResult = purchaseInitiationObjectResult.Value as PurchaseInitiationResultDto;
            purchaseInitiationResult.Should().NotBeNull();

            purchaseInitiationResult.Status.Should().Be((int)purchaseInitiationStatus);
        }
    }
}