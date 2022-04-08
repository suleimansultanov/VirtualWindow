using System.Collections.ObjectModel;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.Tests.Extensions;
using NasladdinPlace.DAL.Tests.Repositories.PosOperationRepository.DataGenerators;
using NUnit.Framework;

namespace NasladdinPlace.DAL.Tests.Repositories.PosOperationRepository
{
    [TestFixture]
    public class PosOperationRepositoryTests
    {
        private const int DefaultPosId = 1;
        
        private DAL.Repositories.PosOperationRepository _posOperationRepository;
        private Mock<DbSet<PosOperation>> _mockPosOperationDbSet;

        [SetUp]
        public void SetUp()
        {
            _mockPosOperationDbSet = new Mock<DbSet<PosOperation>>();
            
            var mockDbContext = new Mock<IApplicationDbContext>();
            mockDbContext.Setup(c => c.PosOperations).Returns(() => _mockPosOperationDbSet.Object);
            
            _posOperationRepository = new DAL.Repositories.PosOperationRepository(mockDbContext.Object);
        }

        [Test]
        public void GetLatestActiveOperationOfPosAsync_NoActiveOperationIsGiven_ShouldReturnNull()
        {
            _mockPosOperationDbSet.SetSource(new Collection<PosOperation>());
            var latestActiveOperation = _posOperationRepository.GetLatestActiveOfPosAsync(DefaultPosId).Result;
            latestActiveOperation.Should().BeNull();
        }

        [Test]
        public void GetLatestActiveOperationOfPosAsync_ActiveOperationIsGiven_ShouldReturnActiveOperation(
            [ValueSource(typeof(LastActivePosOperationDataGenerator), "Data")] Collection<PosOperation> posOperations)
        {
            _mockPosOperationDbSet.SetSource(posOperations);
            var latestActiveOperation = _posOperationRepository.GetLatestActiveOfPosAsync(
                LastActivePosOperationDataGenerator.ActiveOperationPosId).Result;
            latestActiveOperation.Should().NotBeNull();
            latestActiveOperation.DateCompleted.Should().BeNull();
            latestActiveOperation.PosId.Should().Be(LastActivePosOperationDataGenerator.ActiveOperationPosId);
        }
    }
}