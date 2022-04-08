using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NUnit.Framework;
using System.Linq;

namespace NasladdinPlace.Api.Tests.Controllers
{
    public class LabeledGoodsControllerTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPointOfSaleId = 1;
        private const int IncorrectPointOfSaleId = 10;
        private const int IncorrectLabeledGoodId = 1000000;
        private const string LabeledGoodWithGoodLabel = "E2 00 00 16 18 0B 01 66 15 20 7E EA";
        private const string IncorrectLabel = "1234";

        private LabeledGoodsController _labeledGoodsController;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPointOfSaleId));

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();

            _labeledGoodsController = serviceProvider.GetRequiredService<LabeledGoodsController>();

            _labeledGoodsController.ControllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);
        }

        [Test]
        public void BlockLabel_CorrectLabelsAreGiven_ShouldReturnOkResultAndDisableLabeledGood()
        {
            var labelsForBlocking = new LabelsDto { Labels = { LabeledGoodWithGoodLabel } };

            var result = _labeledGoodsController.BlockLabels(DefaultPointOfSaleId, labelsForBlocking).Result;

            result.Should().BeOfType<OkResult>();

            Context.LabeledGoods.Where(lg => lg.IsDisabled && lg.Label == LabeledGoodWithGoodLabel).Should().HaveCount(1);
        }

        [Test]
        public void UntieLabelFromGood_CorrectLabeledGoodIdAndPointOSaleIdAreGiven_ShouldReturnOkObjectResultWithIdAndGoodIdAndPriceChangeToNullInDatabase()
        {
            var labeledGoodWithGood = Context.LabeledGoods.First(lg => lg.Label == LabeledGoodWithGoodLabel);

            var result = _labeledGoodsController.UntieLabelFromGood(labeledGoodWithGood.Id, DefaultPointOfSaleId).Result;

            result.Should().BeOfType<OkObjectResult>();

            Context.LabeledGoods.Where(lg => lg.GoodId == null).Should().HaveCount(1);
        }

        [Test]
        public void BlockLabel_IncorrectLabelIsGiven_ShouldReturnOkResultButWillBeNoChangesInDatabase()
        {
            var incorrectLabels = new LabelsDto { Labels = { IncorrectLabel } };

            var result = _labeledGoodsController.BlockLabels(DefaultPointOfSaleId, incorrectLabels).Result;

            result.Should().BeOfType<OkResult>();

            Context.LabeledGoods.Where(lg => lg.IsDisabled && lg.Label == LabeledGoodWithGoodLabel).Should().BeEmpty();
        }

        [Test]
        public void UntieLabelFromGood_NonExistentLabeledGood_ShouldReturnNotFoundResult()
        {
            var result = _labeledGoodsController.UntieLabelFromGood(IncorrectLabeledGoodId, IncorrectPointOfSaleId).Result;

            result.Should().BeOfType<NotFoundResult>();
        }

    }
}
