using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.HardToDetectLabels;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.LabeledGoodBlocker
{
    public class LabeledGoodBlockerTests
    {
        private const string FirstLabeledGood = "E2 80 11 60 60 00 02 05 2A 98 4B A1";
        private const string SecondLabeledGood = "E2 80 11 60 60 00 02 05 2A 98 4B A2";

        private const int DefaultPosId = 1;

        private readonly Mock<ILabeledGoodRepository> _mockLabeledGoodRepository;
        private readonly LabeledGoodsBlocker _labeledGoodBlocker;
        private readonly ICollection<string> _labels;

        public LabeledGoodBlockerTests()
        {
            var mockUoW = new Mock<IUnitOfWork>();
            _labels = new Collection<string> { FirstLabeledGood, SecondLabeledGood };

            var mockPosRepository = new Mock<IPosRepository>();
            _mockLabeledGoodRepository = new Mock<ILabeledGoodRepository>();
            var mockUoWFactory = new Mock<IUnitOfWorkFactory>();

            mockUoW.SetupGet(u => u.PointsOfSale).Returns(mockPosRepository.Object);
            mockUoW.SetupGet(u => u.LabeledGoods).Returns(_mockLabeledGoodRepository.Object);
            mockUoWFactory.Setup(f => f.MakeUnitOfWork()).Returns(mockUoW.Object);
            var unitOfWork = mockUoW.Object;
            _labeledGoodBlocker = new LabeledGoodsBlocker(unitOfWork, _labels);
        }

        [Fact]
        public void BlockAsync_LabeledGoodIsGiven_ShouldReturnEmptyList()
        {
            _mockLabeledGoodRepository.Setup(r => r.GetEnabledByLabelsAsync(_labels))
                .Returns(Task.FromResult(new List<LabeledGood>()));

            BlockLabelsAndEnsureBlockedOnlyGivenNumberOfLabels(0);
        }

        [Fact]
        public void BlockAsync_LabeledGoodIsGiven_ShouldReturnListWithOneElement()
        {
            var labeledGoodsInDatabase = new List<LabeledGood> { LabeledGood.OfPos(DefaultPosId, FirstLabeledGood) };

            _mockLabeledGoodRepository.Setup(r => r.GetEnabledByLabelsAsync(_labels))
                .Returns(Task.FromResult(labeledGoodsInDatabase));

            BlockLabelsAndEnsureBlockedOnlyGivenNumberOfLabels(1);
        }

        [Fact]
        public void BlockAsync_LabeledGoodIsGiven_ShouldReturnListWithTwoElements()
        {
            var labeledGoodsInDatabase = new List<LabeledGood> {
                LabeledGood.OfPos(DefaultPosId, FirstLabeledGood),
                LabeledGood.OfPos(DefaultPosId, SecondLabeledGood)
            };
            _mockLabeledGoodRepository.Setup(r => r.GetEnabledByLabelsAsync(_labels))
                .Returns(Task.FromResult(labeledGoodsInDatabase));

            BlockLabelsAndEnsureBlockedOnlyGivenNumberOfLabels(2);
        }

        private void BlockLabelsAndEnsureBlockedOnlyGivenNumberOfLabels(int expectedCount)
        {
            _labeledGoodBlocker.BlockAsync().GetAwaiter().GetResult();
            _labeledGoodBlocker.BlockedLabeledGoods.Count().Should().Be(expectedCount);
        }
    }
}