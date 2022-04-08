using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Extensions;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NasladdinPlace.Utilities.DateTimeConverter.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NasladdinPlace.Api.Tests.Controllers
{
    public class LabeledGoodsPartnerControllerTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPointOfSaleId = 1;
        private const int DefaultGoodId = 1;
        private const int DefaultCurrencyId = 1;
        private const int DefaultPrice = 1;
        private const int DefaultLabeledGoodId = 1;
        private const int DefaultLabeledGoodsExpectedCount = 1;
        private const int WrongLabeledGoodId = 1024;
        private const int CorrectResultListCount = 1;
        private const string CorrectExistingLabel = "E2 00 00 16 18 0B 01 66 15 20 7E EA";
        private const string NotExistingLabel = "E2 00 00 16 18 0B 01 66 15 20 7E AE";
        private const string GoodIsNotPublishedErrorMessage = "Good with Id = 1 isn't published. Publish good and try again.";
        private const string LabeledGoodWithIdNotFoundErrorMessage = "LabeledGood with id {0} does not exist.";
        private const string LabeledGoodWithLabelNotExistsErrorMessage = "Labeled good with label {0} exists.";
        private LabeledGoodsPartnerController _controller;

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
            _controller = serviceProvider.GetRequiredService<LabeledGoodsPartnerController>();
            _controller.ControllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);
        }

        [Test]
        public void AddAsync_CorrectLabelToAddingIsGiven_ShouldTieLabelToGoodAndReturnResultWithLabeledGood()
        {
            PublishGoodById(DefaultGoodId);

            var labeledGoods = new List<LabeledGoodPartnerDto>()
            {
                CreateLabeledGoodDto(NotExistingLabel)
            };

            var result = GetAddingResult(labeledGoods);

            result.Should().BeOfType<ObjectResult>();

            var objectResult = result as ObjectResult;

            var labeledGoodsResult = objectResult?.Value as List<LabeledGood>;

            labeledGoodsResult.Should().NotBeNull();

            labeledGoodsResult.First().Label.Should().Be(NotExistingLabel);

            labeledGoodsResult.Count.Should().Be(DefaultLabeledGoodsExpectedCount);
        }

        [Test]
        public void AddAsync_CorrectLabelWithNotPublishedGoodIdToAddingAreGiven_ShouldNotTieLabelToGoodAndReturnBadRequestWithErrorMessage()
        {
            var labeledGoods = new List<LabeledGoodPartnerDto>()
            {
                CreateLabeledGoodDto(NotExistingLabel)
            };

            var result = GetAddingResult(labeledGoods);

            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            var labeledGoodsResult = objectResult?.Value as string;

            labeledGoodsResult.Should().NotBeNullOrEmpty();

            labeledGoodsResult.Should().Be(GoodIsNotPublishedErrorMessage);
        }

        [Test]
        public void AddAsync_WrongLabelIsGiven_ShouldNotTieLabelToGoodAndReturnBadRequestWithErrorMessage()
        {
            var labeledGoods = new List<LabeledGoodPartnerDto>()
            {
                CreateLabeledGoodDto(CorrectExistingLabel)
            };

            var result = GetAddingResult(labeledGoods);

            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            var labeledGoodsResult = objectResult?.Value as string;

            labeledGoodsResult.Should().NotBeNullOrEmpty();

            var errorMessage = string.Format(LabeledGoodWithLabelNotExistsErrorMessage, CorrectExistingLabel);

            labeledGoodsResult.Should().Be(errorMessage);
        }

        [Test]
        public void UpdateAsync_CorrectLabelToUpdateIsGiven_ShouldTieLabelToGoodAndReturnResultWithLabeledGood()
        {
            PublishGoodById(DefaultGoodId);

            var labeledGoodDto = CreateLabeledGoodDto(CorrectExistingLabel);

            labeledGoodDto.Id = GetAddedLabeledGoodId(labeledGoodDto);

            var result = GetUpdatingResult(new List<LabeledGoodPartnerDto>() { labeledGoodDto });

            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var labeledGoodsResult = objectResult?.Value as List<int>;

            labeledGoodsResult.Should().NotBeNull();

            labeledGoodsResult.First().Should().Be(labeledGoodDto.Id);

            labeledGoodsResult.Count.Should().Be(DefaultLabeledGoodsExpectedCount);
        }

        [Test]
        public void UpdateAsync_CorrectLabelWithNotPublishedGoodIdToUpdateAreGiven_ShouldNotTieLabelToGoodAndReturnBadRequestWithErrorMessage()
        {
            var labeledGoodDto = CreateLabeledGoodDto(NotExistingLabel);

            labeledGoodDto.Id = GetAddedLabeledGoodId(labeledGoodDto);

            var result = GetUpdatingResult(new List<LabeledGoodPartnerDto>() { labeledGoodDto });

            result.Should().BeOfType<BadRequestObjectResult>();

            var objectResult = result as BadRequestObjectResult;

            var labeledGoodsResult = objectResult?.Value as string;

            labeledGoodsResult.Should().NotBeNullOrEmpty();

            labeledGoodsResult.Should().Be(GoodIsNotPublishedErrorMessage);
        }

        [Test]
        public void UpdateAsync_WrongIdToUpdateAreGiven_ShouldNotTieLabelToGoodAndReturnNotFoundWithErrorMessage()
        {
            var labeledGoodDto = CreateLabeledGoodDto(CorrectExistingLabel);

            var result = GetUpdatingResult(new List<LabeledGoodPartnerDto>() { labeledGoodDto });

            result.Should().BeOfType<NotFoundObjectResult>();

            var objectResult = result as NotFoundObjectResult;

            var labeledGoodsResult = objectResult?.Value as string;

            labeledGoodsResult.Should().NotBeNullOrEmpty();

            var errorMessage = string.Format(LabeledGoodWithIdNotFoundErrorMessage, labeledGoodDto.Id);

            labeledGoodsResult.Should().Be(errorMessage);
        }

        [Test]
        public void GetByLabels_WithCorrectLabel_ShouldReturnCorrectLabelWithAllNecessaryProps()
        {
            var result = _controller.GetAsync(new List<string> { CorrectExistingLabel }).GetAwaiter().GetResult();
            var dto = ConvertToResultAndAssert(result);

            var strValue = JsonConvert.SerializeObject(dto);
            var expectedJsonResponse = JsonConvert.DeserializeObject<JsonLabeledGoodResponse>(strValue);
            expectedJsonResponse.Should().BeEquivalentTo(dto);
        }

        [Test]
        public void GetByIds_WithCorrectId_ShouldReturnCorrectLabelWithAllNecessaryProps()
        {
            var result = _controller.GetAsync(new List<int> { DefaultLabeledGoodId }).GetAwaiter().GetResult();
            var dto = ConvertToResultAndAssert(result);

            var strValue = JsonConvert.SerializeObject(dto);
            var expectedJsonResponse = JsonConvert.DeserializeObject<JsonLabeledGoodResponse>(strValue);
            expectedJsonResponse.Should().BeEquivalentTo(dto);
        }

        [Test]
        public void GetByIds_WithCorrectId_ShroudThrowExceptionIfOutputDtoHasWrongStructure()
        {
            var result = _controller.GetAsync(new List<string> { CorrectExistingLabel }).GetAwaiter().GetResult();
            var dto = ConvertToResultAndAssert(result);
            var strValue = JsonConvert.SerializeObject(dto);
            var wrongObject = JsonConvert.DeserializeObject<WrongJsonLabeledGoodResponse>(strValue);
            Action equivalentAssertion = () => dto.Should().BeEquivalentTo(wrongObject);
            equivalentAssertion.Should().Throw<AssertionException>();
        }

        [Test]
        public void GetByLabels_WithWrongLabel_ShouldReturnNotFoundResult()
        {
            var result = _controller.GetAsync(new List<string> { NotExistingLabel }).GetAwaiter().GetResult();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public void GetByLabels_WithEmptyList_ShouldReturnNotFoundResult()
        {
            var result = _controller.GetAsync(new List<string>()).GetAwaiter().GetResult();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public void GetByIds_WithWrongId_ShouldReturnNotFoundResult()
        {
            var result = _controller.GetAsync(new List<int> { WrongLabeledGoodId }).GetAwaiter().GetResult();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public void GetByIds_WithEmptyList_ShouldReturnNotFoundResult()
        {
            var result = _controller.GetAsync(new List<int>()).GetAwaiter().GetResult();
            result.Should().BeOfType<NotFoundResult>();
        }

        private LabeledGoodPartnerDto ConvertToResultAndAssert(IActionResult result)
        {
            var objectResult = result as OkObjectResult;
            objectResult.Should().NotBeNull();
            var labeledGoodsList = objectResult.Value as IEnumerable<LabeledGoodPartnerDto>;
            labeledGoodsList.Should().NotBeNull();
            labeledGoodsList.Count().Should().Be(CorrectResultListCount);
            return labeledGoodsList.First();
        }

        private void PublishGoodById(int goodId)
        {
            var good = Context.Goods.FirstOrDefault(g => g.Id == goodId);

            if (good == null)
                throw new ArgumentNullException(nameof(goodId));

            good.SetProperty(nameof(good.PublishingStatus), GoodPublishingStatus.Published);

            Context.SaveChanges();
        }

        private IActionResult GetAddingResult(List<LabeledGoodPartnerDto> labeledGoods)
        {
            var result = _controller.AddAsync(labeledGoods).GetAwaiter().GetResult();

            result.Should().NotBeNull();

            return result;
        }


        private IActionResult GetUpdatingResult(List<LabeledGoodPartnerDto> labeledGoods)
        {
            var result = _controller.UpdateAsync(labeledGoods).GetAwaiter().GetResult();

            result.Should().NotBeNull();

            return result;
        }

        private LabeledGoodPartnerDto CreateLabeledGoodDto(string label)
        {
            var expirationDate = DateTime.UtcNow.AddDays(1).ToMillisecondsSince1970();
            return new LabeledGoodPartnerDto()
            {
                Label = label,
                GoodId = DefaultGoodId,
                CurrencyId = DefaultCurrencyId,
                ExpirationDate = expirationDate,
                Price = DefaultPrice
            };
        }

        private int GetAddedLabeledGoodId(LabeledGoodPartnerDto labeledGoodDto)
        {
            _controller.AddAsync(new List<LabeledGoodPartnerDto>() { labeledGoodDto }).GetAwaiter().GetResult();

            var labeledGood = Context.LabeledGoods.FirstOrDefault(lg => lg.Label == CorrectExistingLabel);

            if (labeledGood == null)
                throw new NullReferenceException("LabeledGood is null.");

            return labeledGood.Id;
        }
    }
}
