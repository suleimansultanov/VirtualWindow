using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.PosLabeledGoodBlocker.DataGenerator;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.HardToDetectLabels;
using NasladdinPlace.Core.Services.Shared.Models;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NasladdinPlace.Api.Tests.Scenarios.PosLabeledGoodBlocker
{
    public class PosLabeledGoodBlockerIntegrationTests : TestsBase
    {
        private const int DefaultPosId = 1;

        private IUnitOfWork _unitOfWork;
        private IPosLabeledGoodsBlocker _posLabeledGoodsBlocker;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new PaymentCardsDataSet(1));
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(new IncompletePosOperationsDataSet(posId: DefaultPosId, userId: 1));

            var serviceProvider = TestServiceProviderFactory.Create();
            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _posLabeledGoodsBlocker = serviceProvider.GetRequiredService<IPosLabeledGoodsBlocker>();
            _unitOfWork = unitOfWorkFactory.MakeUnitOfWork();
        }

        [TestCaseSource(typeof(LabeledGoodsDataGenerator))]
        public void ReturnExpectedBlockedContentWhenDifferentPosContentLabelsAreGiven(PosContent posContent,
            int expectedBlockedLabels)
        {
            foreach (var label in posContent.Labels)
            {
                Seeder.Seed(LabeledGoodsBlockedDataSet.FromPosIdWithLabels(DefaultPosId, label));
            }

            _posLabeledGoodsBlocker.BlockAsync(_unitOfWork, posContent).GetAwaiter().GetResult();

            EnsureExistsOnlyGivenDisabledLabeledGoodsNumber(expectedBlockedLabels);
        }

        private void EnsureExistsOnlyGivenDisabledLabeledGoodsNumber(int expectedDisabledLabeledGoodsCount)
        {
            var disabledLabeledGoods = Context.LabeledGoods.AsNoTracking();
            disabledLabeledGoods.Select(x => x.IsDisabled).Count().Should().Be(expectedDisabledLabeledGoodsCount);
        }
    }
}
