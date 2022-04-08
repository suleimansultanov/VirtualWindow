using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Tests.Controllers
{
    public class CurrentPurchaseControllerTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 1;
        private const int DefaultPosOperationId = 1;
        private const int DefaultGoodId = 1;
        private const int DefaultLabeledGoodId = 1;
        private const int DefaultCurrencyId = 1;
        private const decimal DefaultPrice = 15M;

        private CurrentPurchaseController _currentPurchaseController;
        private IServiceProvider _serviceProvider;

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
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));

            var checkItem = CheckItem.NewBuilder(DefaultPosId, DefaultPosOperationId, DefaultGoodId, DefaultLabeledGoodId, DefaultCurrencyId)
                .SetPrice(DefaultPrice)
                .SetStatus(CheckItemStatus.Unpaid)
                .Build();

            Context.CheckItems.Add(checkItem);

            Context.SaveChanges();

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            _serviceProvider = TestServiceProviderFactory.Create();

            var controllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);

            _currentPurchaseController = _serviceProvider.GetRequiredService<CurrentPurchaseController>();
            _currentPurchaseController.ControllerContext = controllerContext;
        }

        [Test]
        public void GetCurrentPurchaseCheck_CheckWithOneGoodIsGiven_ShouldReturnNotNullableCheckWithCorrectTotalPriceAndGoodsCount()
        {
            var result = _currentPurchaseController.GetCurrentPurchaseCheckAsync().GetAwaiter().GetResult();

            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var checkDto = objectResult?.Value as CheckDto;

            checkDto.Should().NotBeNull();

            EnsureCheckDtoCorrect(checkDto, expectedTotalPrice: DefaultPrice, expectedTotalQuantity: 1);
        }

        [Test]
        public void GetCurrentPurchaseCheck_CheckItemsWithDiscountAreGiven_ShouldReturnNotNullableCheckWithCorrectTotalPriceAndGoodsCount()
        {
            var checkItem = Context.CheckItems.FirstOrDefault();
            checkItem.AddDiscount(30M);
            Context.SaveChanges();

            var result = _currentPurchaseController.GetCurrentPurchaseCheckAsync().GetAwaiter().GetResult();

            result.Should().BeOfType<OkObjectResult>();

            var objectResult = result as OkObjectResult;

            var checkDto = objectResult?.Value as CheckDto;

            checkDto.Should().NotBeNull();

            EnsureCheckDtoCorrect(checkDto, expectedTotalPrice: 10M, expectedTotalQuantity: 1);
        }

        [Test]
        public void GetCurrentPurchaseCheck_NoUserIsGiven_ShouldReturnUnauthorizedResult()
        {
            _currentPurchaseController.ControllerContext.HttpContext.User = null;
            var result = _currentPurchaseController.GetCurrentPurchaseCheckAsync().GetAwaiter().GetResult();

            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Test]
        public void RecheckCurrentPurchase_PaidPosOperationIsGivenAndRecheckCalledThreeTimes_ShouldReturnCorrectResultAndTelegramMessagesCount()
        {
            var telegramMessagesCount = 0;

            var telegramMessageSender = _serviceProvider.GetRequiredService<Mock<ITelegramChannelMessageSender>>();

            telegramMessageSender
                .Setup(f => f.SendAsync(It.IsAny<string>()))
                .Returns(() =>
                {
                    Interlocked.Increment(ref telegramMessagesCount);
                    return Task.CompletedTask;
                });

            for (var i = 0; i < 3; i++)
                _currentPurchaseController.RecheckCurrentPurchaseAsync().GetAwaiter().GetResult();

            Interlocked.CompareExchange(ref telegramMessagesCount, 1, 1).Should().Be(1);
        }

        private static void EnsureCheckDtoCorrect(CheckDto checkDto, decimal expectedTotalPrice, int expectedTotalQuantity)
        {
            checkDto.Goods.Count.Should().Be(1);

            var checkPriceInfo = checkDto.PriceInfo;

            checkPriceInfo.TotalPrice.Should().Be(expectedTotalPrice);
            checkPriceInfo.Quantity.Should().Be(expectedTotalQuantity);
            checkPriceInfo.Currency.Should().NotBeNull();
            checkPriceInfo.Currency.Name.Should().NotBeNullOrWhiteSpace();
            checkDto.PurchaseDateTime.Should().NotBeNullOrWhiteSpace();
            checkDto.IsZero.Should().Be(expectedTotalPrice == 0M);

            foreach (var checkDtoGood in checkDto.Goods)
            {
                checkDtoGood.Name.Should().NotBeNullOrWhiteSpace();
                checkDtoGood.PriceInfo.Should().NotBeNull();
                checkDtoGood.PriceInfo.Currency.Should().NotBeNull();
                checkDtoGood.PriceInfo.Currency.Name.Should().NotBeNullOrEmpty();
                checkDtoGood.PriceInfo.Quantity.Should().BeGreaterOrEqualTo(0);
            }
        }
    }
}
