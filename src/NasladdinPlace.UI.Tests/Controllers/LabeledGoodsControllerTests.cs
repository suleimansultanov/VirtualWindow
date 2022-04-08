using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.UI.Controllers.Api;
using NasladdinPlace.UI.Tests.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;

namespace NasladdinPlace.UI.Tests.Controllers
{
    public class LabeledGoodsControllerTests : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultGoodId = 1;
        private const int DefaultCurrencyId = 1;
        private const string DefaultLabel = "E2 00 00 16 18 0B 01 66 15 20 7E EA";
        private const string DefaultUserId = "1";
        private const int IncorrectLabeledGoodId = 1000000;

        private IServiceProvider _serviceProvider;

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

            _serviceProvider = new ServiceProviderFactory().CreateServiceProvider(Context);

            _labeledGoodsController = _serviceProvider.GetRequiredService<LabeledGoodsController>();

            _labeledGoodsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, DefaultUserId)
                            },
                            "sameAuthTypeName"
                        )
                    )
                }
            };
        }

        [Test]
        public void UntiedLabelFromGood_NonExistentLabeledGood_ShouldReturnNotFoundResult()
        {
            var result = _labeledGoodsController.UntieLabelFromGoodAsync(IncorrectLabeledGoodId).Result;
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public void UntiedLabelFromGood_CorrectLabeledGoodWithGoodIdAndPriceAreGiven_ShouldReturnOkResultAndGoodIdAndPriceOfLabeledGoodChangeToNullInDatabase()
        {
            var labeledGood = LabeledGood.NewOfPosBuilder(DefaultPosId, DefaultLabel)
                .TieToGood(DefaultGoodId, new LabeledGoodPrice(20M, DefaultCurrencyId), new ExpirationPeriod())
                .Build();

            Insert(new Collection<LabeledGood>
            {
                labeledGood
            });

            Context.SaveChanges();

            var labeledGoodInDatabase = Context.LabeledGoods.AsNoTracking().First(lg => lg.Label == DefaultLabel);
            labeledGoodInDatabase.Should().NotBeNull();
            labeledGoodInDatabase.CurrencyId.Should().NotBeNull();
            labeledGoodInDatabase.GoodId.Should().NotBeNull();
            labeledGoodInDatabase.Price.Should().NotBeNull();

            var result = _labeledGoodsController.UntieLabelFromGoodAsync(labeledGoodInDatabase.Id).Result;
            result.Should().BeOfType<OkResult>();

            var labeledGoodResult = Context.LabeledGoods.AsNoTracking().First(lg => lg.Label == DefaultLabel);
            labeledGoodResult.Should().NotBeNull();
            labeledGoodResult.CurrencyId.Should().BeNull();
            labeledGoodResult.GoodId.Should().BeNull();
            labeledGoodResult.Price.Should().BeNull();
        }

        private void Insert<T>(IEnumerable<T> entities) where T : class
        {
            Seeder.Seed(entities);
        }
    }
}
