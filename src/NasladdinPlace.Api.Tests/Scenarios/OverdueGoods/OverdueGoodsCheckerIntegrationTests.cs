using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Api.Tests.Scenarios.OverdueGoods.DataGenerators;
using NasladdinPlace.Api.Tests.Scenarios.OverdueGoods.Models;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.OverdueGoods.Checker;
using NasladdinPlace.Core.Services.OverdueGoods.Models;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System.Collections.Generic;
using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;

namespace NasladdinPlace.Api.Tests.Scenarios.OverdueGoods
{
    public class OverdueGoodsCheckerIntegrationTests : TestsBase
    {
        private IOverdueGoodsChecker _overdueGoodsChecker;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            var serviceProvider = TestServiceProviderFactory.Create();

            _overdueGoodsChecker = serviceProvider.GetRequiredService<IOverdueGoodsChecker>();
        }

        [TestCaseSource(typeof(OverdueLabeledGoodsDataGenerator))]
        public void
            CheckAsync_LabeledGoodsWithDifferentExpirationPeriodAreGiven_ShouldReturnExpectedCountOfEachOverdueType(
                IEnumerable<LabeledGood> initLabeledGoods,
                Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance> expectedOverdueGoodsInfoInstances)
        {
            Seeder.Seed(initLabeledGoods);
            AddEventBindingForOverdueGoodsChecker(expectedOverdueGoodsInfoInstances);

            _overdueGoodsChecker.CheckAsync().Wait();
        }

        private void AddEventBindingForOverdueGoodsChecker(Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance> expectedOverdueGoodsInfoInstances)
        {
            _overdueGoodsChecker.OnFoundOverdueGoods += (sender, groupedOverdueGoodsInfo) =>
            {
                AssertOverdueLabeledGoodsEqualsExpectedCountOfEachOverdueType(groupedOverdueGoodsInfo,
                    expectedOverdueGoodsInfoInstances);
            };
        }

        private static void AssertOverdueLabeledGoodsEqualsExpectedCountOfEachOverdueType(
            Dictionary<OverdueType, IEnumerable<PosGoodInstances>> actualOverdueGoodsInfoInstances,
            Dictionary<OverdueType, ExpectedOverdueGoodsInfoInstance> expectedOverdueGoodsInfoInstances)
        {
            actualOverdueGoodsInfoInstances.Should().NotBeNull();
            foreach (var expectedOverdueGoodsInfoInfoInstance in expectedOverdueGoodsInfoInstances)
            {
                actualOverdueGoodsInfoInstances.TryGetValue(expectedOverdueGoodsInfoInfoInstance.Key,
                    out var posGoodInstances).Should().BeTrue();

                posGoodInstances.Should()
                    .HaveCount(expectedOverdueGoodsInfoInfoInstance.Value.GroupedCount);
                AssertOverdueLabeledGoodsInstanceEqualsExpectedCount(
                    posGoodInstances,
                    expectedOverdueGoodsInfoInfoInstance.Value.InstancesCountByPos);
            }
        }

        private static void AssertOverdueLabeledGoodsInstanceEqualsExpectedCount(
            IEnumerable<PosGoodInstances> actualPosGoodInstances, IReadOnlyDictionary<int, int> expectedInstancesByPos)
        {
            foreach (var instances in actualPosGoodInstances)
            {
                expectedInstancesByPos.TryGetValue(instances.PosId, out var instancesCountByPos);
                instances.OverdueGoods.Should().HaveCount(instancesCountByPos);
            }
        }
    }
}