using System.Linq;
using FluentAssertions;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;

namespace NasladdinPlace.DAL.Tests.Repositories.PosRepository
{
    public class PosRepositoryIntegrationTests : TestsBase
    {
        private IPosRepository _posRepository;

        public override void SetUp()
        {
            base.SetUp();
            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());

            _posRepository = new DAL.Repositories.PosRepository(Context);
        }

        [Test]
        public void GetDisabledNotificationsAsync_PosEnabledNotificationsAreGiven_ShouldReturnNull()
        {
            var pointsOfSaleWithDisabledNotifications =_posRepository.GetActiveWithDisabledNotificationsAsync().Result;
            pointsOfSaleWithDisabledNotifications.Should().BeEmpty();
        }

        [Test]
        public void GetDisabledNotificationsAsync_PosDisabledNotificationsIsGiven_ShouldReturnExpectedResult()
        {
            var pointOfSale = Context.PointsOfSale.FirstOrDefault();
            pointOfSale?.DisableNotifications();

            Context.SaveChanges();

            var pointsOfSaleWithDisabledNotifications = _posRepository.GetActiveWithDisabledNotificationsAsync().Result;
            pointsOfSaleWithDisabledNotifications.Should().NotBeEmpty();
            pointsOfSaleWithDisabledNotifications.Should().HaveCount(1);
        }
    }
}