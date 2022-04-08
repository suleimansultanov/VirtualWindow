using FluentAssertions;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Tests.Repositories.PosOperationRepository.DataGenerators;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.DAL.Tests.Repositories.PosOperationRepository
{
    public class PosOperationRepositoryIntegrationTests : TestsBase
    {
        private const int DefaultPosId = 1;
        private const int DefaultUserId = 1;
        
        private DAL.Repositories.PosOperationRepository _posOperationRepository;
        
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
            
            _posOperationRepository = new DAL.Repositories.PosOperationRepository(Context);
        }

        [Test]
        public void GetLatestActiveOperationOfPosAsync_NoActiveOperationIsGiven_ShouldReturnNull()
        {
            var latestActiveOperation = _posOperationRepository.GetLatestActiveOfPosAsync(DefaultPosId).Result;
            latestActiveOperation.Should().BeNull();
        }

        [Test]
        public void GetLatestActiveOperationOfShopAsync_ActiveOperationIsGiven_ShouldReturnActiveOperation(
            [ValueSource(typeof(LastActivePosOperationDataGenerator), "Data")] Collection<PosOperation> PosOperations)
        {
            Insert(PosOperations);

            var latestActiveOperation = _posOperationRepository.GetLatestActiveOfPosAsync(
                LastActivePosOperationDataGenerator.ActiveOperationPosId).Result;
            latestActiveOperation.Should().NotBeNull();
            latestActiveOperation.PosId.Should().Be(LastActivePosOperationDataGenerator.ActiveOperationPosId);
            latestActiveOperation.DateCompleted.Should().BeNull();
        }
        
        [Test]
        public void GetLatestActiveOperationOfUserAsync_NoActiveOperationIsGiven_ShouldReturnNull()
        {
            var latestActiveOperation = _posOperationRepository.GetLatestUnpaidOfUserAsync(DefaultUserId).Result;
            latestActiveOperation.Should().BeNull();
        }

        [Test]
        public void GetLatestActiveOperationOfUserAsync_ActiveOperationIsGiven_ShouldReturnActiveOperation(
            [ValueSource(typeof(LastActiveUserOperationDataGenerator), "Data")] Collection<PosOperation> posOperations)
        {
            Insert(posOperations);
            
            var latestActiveOperation = _posOperationRepository.GetLatestUnpaidOfUserAsync(
                LastActiveUserOperationDataGenerator.ActiveUserId).Result;
            latestActiveOperation.Should().NotBeNull();
            latestActiveOperation.DateCompleted.Should().BeNull();
            latestActiveOperation.UserId.Should().Be(LastActiveUserOperationDataGenerator.ActiveUserId);
        }

        private void Insert<T>(IEnumerable<T> entities) where T : class
        {
            Seeder.Seed(entities);
        }
    }
}