using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Pos.LabeledGoodsCreator;
using NasladdinPlace.Core.Services.Shared.Models;
using Xunit;
using ILogger = NasladdinPlace.Logging.ILogger;

namespace NasladdinPlace.Core.Tests.Services.PosLabeledGoodsCreator
{
    public class PosLabeledGoodsFromLabelsCreatorTests
    {
        private const string LabelInDatabase = "LabelInDatabase";
        private const string LabelOutsideDatabase = "LabelOutsideDatabase";
        
        private const int DefaultPosId = 1;
        
        private readonly Mock<ILabeledGoodRepository> _mockLabeledGoodRepository;
        private readonly PosLabeledGoodsFromLabelsCreator _posLabeledGoodsFromLabelsCreator;
        private readonly IUnitOfWork _unitOfWork;
        
        public PosLabeledGoodsFromLabelsCreatorTests()
        {
            _mockLabeledGoodRepository = new Mock<ILabeledGoodRepository>();
            var mockUoW = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger>();

            mockUoW.SetupGet(u => u.LabeledGoods).Returns(_mockLabeledGoodRepository.Object);
            _unitOfWork = mockUoW.Object;
            _posLabeledGoodsFromLabelsCreator = new PosLabeledGoodsFromLabelsCreator(mockLogger.Object);
        }

        [Fact]
        public async Task Create_NoLabelIsGiven_ShouldReturnEmptyCollection()
        {
            var labels = new Collection<string>();
            _mockLabeledGoodRepository.Setup(r => r.GetByLabelsAsync(labels))
                .Returns(Task.FromResult(new List<LabeledGood>()));

            var posContent = new PosContent(DefaultPosId, labels);
            var newLabeledGoods = await _posLabeledGoodsFromLabelsCreator.CreateAsync(_unitOfWork, posContent);
            newLabeledGoods.Should().BeEmpty();
        }

        [Fact]
        public async Task Create_ExistingLabelIsGiven_ShouldReturnEmptyCollection()
        {
            var labels = new Collection<string> { LabelInDatabase };
            var labeledGoodsInDatabase = new List<LabeledGood> { LabeledGood.OfPos(DefaultPosId, LabelInDatabase) };
            _mockLabeledGoodRepository.Setup(r => r.GetByLabelsAsync(labels))
                .Returns(Task.FromResult(labeledGoodsInDatabase));
            
            var posContent = new PosContent(DefaultPosId, labels);
            var newLabeledGoods = await _posLabeledGoodsFromLabelsCreator.CreateAsync(_unitOfWork, posContent);
            newLabeledGoods.Should().BeEmpty();
        }

        [Fact]
        public async Task Create_ExistingAndNewLabelsAreGiven_ShouldReturnCollectionOfLabeledGoodWithNewLabel()
        {
            var labels = new Collection<string> { LabelInDatabase, LabelOutsideDatabase };
            var labeledGoodsInDatabase = new List<LabeledGood>
            {
                LabeledGood.OfPos(0, LabelInDatabase)
            };
            _mockLabeledGoodRepository.Setup(r => r.GetByLabelsAsync(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult(labeledGoodsInDatabase));
            
            var posContent = new PosContent(DefaultPosId, labels);
            var newLabeledGoods = await _posLabeledGoodsFromLabelsCreator.CreateAsync(_unitOfWork, posContent);
            newLabeledGoods.Should().HaveCount(1);
            newLabeledGoods.First().Label.Should().Be(LabelOutsideDatabase);
        }
    }
}