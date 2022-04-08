using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Services.Catalog.Models;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Dtos.Catalog;

namespace NasladdinPlace.Api.Tests.Controllers
{
    public class CatalogControllerTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPointOfSaleId = 1;
        private const int DefaultCategoryId = 0;
        private const int IncorrectPageNumber = 0;
        private const string ImagePath = "images/image.jpg";

        private CatalogController _catalogController;

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
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPointOfSaleId));
            
            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();

            SetAllPosesInactive();
            _catalogController = serviceProvider.GetRequiredService<CatalogController>();

            _catalogController.ControllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);
        }

        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(3, null)]
        public void GetPointsOfSaleAsync_PageIsGivenAndAllPosesAreActive_ShouldReturnListOfPointsOfSale(byte page, int? expectedPosCount)
        {
            ActivatePointsOfSale();

            EnsureGettingPointsOfSaleResultIsCorrectAndHaveExpectedResultCount(page, expectedPosCount);
        }

        [TestCase(1, 1)]
        [TestCase(2, null)]
        public void GetPointsOfSaleAsync_PageIsGivenAndOnlyOnePosActive_ShouldReturnListOfPointsOfSale(byte page, int? expectedPosCount)
        {
            ActivateFirstPointOfSale();

            EnsureGettingPointsOfSaleResultIsCorrectAndHaveExpectedResultCount(page, expectedPosCount);
        }

        [TestCase(1, 2, 1)]
        [TestCase(2, 1, 2)]
        [TestCase(3, null, 3)]
        public void GetPointsOfSaleAsync_PageIsGivenAndUserHasLastVisitedPosAndAllPosesAreActive_ShouldReturnListOfPointsOfSaleWithLastVisitedPos(
            byte page,
            int? expectedPosCount,
            int lastVisitedPosId)
        {
            ActivatePointsOfSale();
            Seeder.Seed(new IncompletePosOperationsDataSet(posId: lastVisitedPosId, userId: DefaultUserId)
                .Select(po =>
                {
                    po.MarkAsPendingCompletion();
                    po.MarkAsPendingCheckCreation();
                    po.MarkAsCompletedAndRememberDate();
                    return po;
                }));

            EnsureGettingPointsOfSaleResultIsCorrectAndHaveExpectedResultCount(page, expectedPosCount, lastVisitedPosId);
        }

        [Test]
        public void GetPointsOfSaleAsync_IncorrectPageIsGiven_ShouldReturnBadRequestResult()
        {
            EnsureGettingPointsOfSaleResultIsBadRequestResult(IncorrectPageNumber);
        }

        [TestCase(1, 1, 1, 2)]
        [TestCase(1, 2, null, null)]
        public void GetPosContent_PosIdAndPageAreGiven_ShouldReturnPosContent(int posId, byte page,
            int? expectedPosContentCount, int? expectedGoodsCount)
        {
            var goodCategory = Context.GoodCategories.FirstOrDefault();
            if (goodCategory != null)
            {
                goodCategory.SetImagePath(ImagePath);
                Context.SaveChanges();
            }

            EnsureGettingPosContentResultIsCorrectAndHaveExpectedResultCount(posId, page, expectedPosContentCount,
                expectedGoodsCount);
        }

        [TestCase(1, 2)]
        [TestCase(2, 0)]
        [TestCase(3, null)]
        public void GetCategoryItems_CategoryItemsDtoIsGiven_ShouldReturnGetCategoryItems(byte page, int? expectedGoodsCount)
        {
            var goodCategory = Context.GoodCategories.FirstOrDefault();
            if (goodCategory != null)
            {
                goodCategory.SetImagePath(ImagePath);
                Context.SaveChanges();
            }

            EnsureGettingCategoryItemsResultIsCorrectAndHaveExpectedResultCount(page, expectedGoodsCount);
        }

        private void EnsureGettingPointsOfSaleResultIsCorrectAndHaveExpectedResultCount(byte page, int? expectedPosCount, 
            int? lastVisitedPosId = null)
        {
            var result = _catalogController.GetPointsOfSaleAsync(page).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var getPointsOfSale = objectResult?.Value as CatalogPointOfSale;

            getPointsOfSale.Should().NotBeNull();

            if (lastVisitedPosId.HasValue)
            {
                getPointsOfSale.LastVisited.Should().NotBeNull();
                getPointsOfSale.LastVisited.Id.Should().Be(lastVisitedPosId);
            }

            if (expectedPosCount.HasValue)
                getPointsOfSale.Items.Count.Should().Be(expectedPosCount);
            else
                getPointsOfSale.Items.Should().BeNull();
        }

        private void EnsureGettingPointsOfSaleResultIsBadRequestResult(byte page)
        {
            var result = _catalogController.GetPointsOfSaleAsync(page).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            var getPointsOfSale = objectResult?.Value as string;

            getPointsOfSale.Should().Be($"{nameof(page)} must be greater than 0.");
        }

        private void EnsureGettingPosContentResultIsCorrectAndHaveExpectedResultCount(int posId, byte page,
            int? expectedPosContentCount, int? expectedGoodsCount)
        {
            var result = _catalogController.GetPosContent(posId, page).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            objectResult.Should().NotBeNull();
            var posContents = objectResult?.Value as List<PosContent>;

            if (expectedPosContentCount.HasValue)
            {
                posContents.Count.Should().Be(expectedPosContentCount);
                var posContent = posContents.FirstOrDefault();
                posContent.Goods.Should().NotBeNull();
                posContent.Goods.Count.Should().Be(expectedGoodsCount);
            }
            else
                posContents.Should().BeEmpty();
        }

        private void EnsureGettingCategoryItemsResultIsCorrectAndHaveExpectedResultCount(byte page, int? expectedGoodsCount)
        {
            var categoryItemsDto = new CategoryItemsDto
            {
                Page = page,
                CategoryId = DefaultCategoryId,
                PosId = DefaultPointOfSaleId
            };

            var result = _catalogController.GetCategoryItems(categoryItemsDto).GetAwaiter().GetResult();

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            objectResult.Should().NotBeNull();
            var categoryItems = objectResult?.Value as List<LabeledGoodWithImageDto>;

            categoryItems.Should().NotBeNull();
            if (expectedGoodsCount.HasValue)
                categoryItems.Count.Should().Be(expectedGoodsCount);
            else
                categoryItems.Should().BeEmpty();
        }

        private void ActivatePointsOfSale()
        {
            var poses = Context.PointsOfSale;
            foreach (var pos in poses)
            {
                pos.ChangeActivityStatus(PosActivityStatus.Active);
            }

            Context.SaveChanges();
        }

        private void ActivateFirstPointOfSale()
        {
            var pos = Context.PointsOfSale.First();
            pos.ChangeActivityStatus(PosActivityStatus.Active);

            Context.SaveChanges();
        }

        private void SetAllPosesInactive()
        {
            foreach (var pos in Context.PointsOfSale)
            {
                pos.ChangeActivityStatus(PosActivityStatus.Inactive);
            }
        }
    }
}
