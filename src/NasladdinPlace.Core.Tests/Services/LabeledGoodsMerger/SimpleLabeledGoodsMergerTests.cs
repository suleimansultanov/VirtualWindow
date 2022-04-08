using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.LabeledGoodsMerger;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.LabeledGoodsMerger
{
    public class SimpleLabeledGoodsMergerTests
    {
        private const int PosId = 1;
        private const int ActiveOperationId = 1;
        private const string Label = "1";

        private readonly SimpleLabeledGoodsMerger _simpleLabeledGoodsMerger;
        private readonly Mock<ILabeledGoodRepository> _mockLabeledGoodsRepository;
        private readonly Mock<IPosOperationRepository> _mockPosOperationRepository;
        private readonly Mock<IUnitOfWork> _mockUoW;

        public SimpleLabeledGoodsMergerTests()
        {
            _mockUoW = new Mock<IUnitOfWork>();
            _mockLabeledGoodsRepository = new Mock<ILabeledGoodRepository>();

            _mockLabeledGoodsRepository
                .Setup(r => r.GetByLabelsAsync(It.IsAny<IEnumerable<string>>()))
                .Returns(Task.FromResult(new List<LabeledGood>()));
            
            _mockPosOperationRepository = new Mock<IPosOperationRepository>();
            _mockUoW.SetupGet(u => u.LabeledGoods).Returns(_mockLabeledGoodsRepository.Object);
            _mockUoW.SetupGet(u => u.PosOperations).Returns(_mockPosOperationRepository.Object);
            _mockUoW.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));
            _simpleLabeledGoodsMerger = new SimpleLabeledGoodsMerger();
        }

        [Fact]
        public async Task MergeAsync_NoPutOrTakenLabeledGoodsLabelsAreGiven_ShouldReturnSuccess()
        {
            var putLabeledGoodsLabels = new Collection<string>();
            var takenLabeledGoodsLabels = new Collection<string>();

            var result = await _simpleLabeledGoodsMerger.MergeAsync(
                _mockUoW.Object,
                PosId,
                putLabeledGoodsLabels,
                takenLabeledGoodsLabels
            );

            result.Should().BeTrue();
        }

        [Fact]
        public async Task MergeAsync_PutLabeledGoodLabelIsGivenAndPresentInRepositoryAndDoesNotBelongToPos_ShouldReturnSuccessAndLabeledGoodShouldBelongToPos()
        {
            var labeledGood1 = LabeledGood.OfPos(PosId, Label);
            _mockLabeledGoodsRepository.Setup(lg => lg.GetByLabelsAsync(new[] {Label}))
                .Returns(Task.FromResult(new[] {labeledGood1}.ToList()));
            var putLabeledGoodsLabels = new[] { Label };
            var takenLabeledGoodsLabels = new Collection<string>();

            var result = await _simpleLabeledGoodsMerger.MergeAsync(
                _mockUoW.Object,
                PosId,
                putLabeledGoodsLabels,
                takenLabeledGoodsLabels
            );

            result.Should().BeTrue();
            labeledGood1.PosId.Should().Be(PosId);
        }

        [Fact]
        public async Task MergeAsync_PutLabeledGoodLabelIsGivenAndPresentsInRepositoryAndBelongToPlantAndHasTransactionId_ShouldReturnSuccessAndLabeledGoodTransactionIdShouldBeNull()
        {
            var labeledGood1 = LabeledGood.OfPos(PosId, Label);
            labeledGood1.MarkAsUsedInPosOperation(ActiveOperationId);
            _mockLabeledGoodsRepository.Setup(lg => lg.GetByLabelsAsync(new[] { Label }))
                .Returns(Task.FromResult(new[] { labeledGood1 }.ToList()));
            var putLabeledGoodsLabels = new[] { Label };
            var takenLabeledGoodsLabels = new Collection<string>();

            var result = await _simpleLabeledGoodsMerger.MergeAsync(
                _mockUoW.Object,
                PosId,
                putLabeledGoodsLabels,
                takenLabeledGoodsLabels
            );

            result.Should().BeTrue();
            labeledGood1.PosOperationId.Should().BeNull();
        }

        [Fact]
        public async Task MergeAsync_PutLabeledGoodLabelIsGivenAndPresentsInRepositoryAndBelongToPlantAndDoesNotHaveTransactionId_ShouldReturnSuccessAndLabeledGoodTransactionIdShouldBeNull()
        {
            var labeledGood1 = LabeledGood.OfPos(PosId, Label);
            _mockLabeledGoodsRepository.Setup(lg => lg.GetByLabelsAsync(new[] { Label }))
                .Returns(Task.FromResult(new[] { labeledGood1 }.ToList()));
            var putLabeledGoodsLabels = new[] { Label };
            var takenLabeledGoodsLabels = new Collection<string>();

            var result = await _simpleLabeledGoodsMerger.MergeAsync(
                _mockUoW.Object,
                PosId,
                putLabeledGoodsLabels,
                takenLabeledGoodsLabels
            );

            result.Should().BeTrue();
            labeledGood1.PosOperationId.Should().BeNull();
        }

        [Fact]
        public async Task MergeAsync_TakenLabeledGoodLabelIsGivenAndPresentsInRepositoryAndDoesNotBelongToPos_ShouldReturnFailure()
        {
            var labeledGood1 = LabeledGood.OfPos(PosId, Label);
            labeledGood1.MarkAsNotBelongingToUserOrPos();
            _mockLabeledGoodsRepository.Setup(lg => lg.GetByLabelsAsync(new[] { Label }))
                .Returns(Task.FromResult(new[] { labeledGood1 }.ToList()));
            var takenLabeledGoodsLabels = new[] {Label };
            var putLabeledGoodsLabels = new Collection<string>();

            var result = await _simpleLabeledGoodsMerger.MergeAsync(
                _mockUoW.Object,
                PosId,
                putLabeledGoodsLabels,
                takenLabeledGoodsLabels
            );

            result.Should().BeFalse();
        }

        [Fact]
        public async Task MergeAsync_TakenLabeledGoodLabelIsGivenAndPresentsInRepositoryAndBelongsToPos_ShouldReturnSuccessAndLabeledGoodTransactionIdShouldBeEqualToActiveTransactionId()
        {
            var labeledGood1 = LabeledGood.OfPos(PosId, Label);
            _mockLabeledGoodsRepository.Setup(lg => lg.GetByLabelsAsync(new[] { Label }))
                .Returns(Task.FromResult(new[] { labeledGood1 }.ToList()));
            _mockPosOperationRepository.Setup(upt => upt.GetLatestActiveOfPosAsync(PosId))
                .Returns(Task.FromResult(PosOperation.NewOfUserAndPosBuilder(1, 1).SetId(ActiveOperationId).Build()));
            var takenLabeledGoodsLabels = new[] { Label };
            var putLabeledGoodsLabels = new Collection<string>();

            var result = await _simpleLabeledGoodsMerger.MergeAsync(
                _mockUoW.Object,
                PosId,
                putLabeledGoodsLabels,
                takenLabeledGoodsLabels
            );

            result.Should().BeTrue();
            labeledGood1.PosOperationId.Should().Be(ActiveOperationId);
        }

        [Fact]
        public async Task MergeAsync_TakenLabeledGoodLabelIsGivenAndPresentsInRepositoryAndBelongsToPlantAndActiveTransactionDoesNotExist_ShouldReturnFailure()
        {
            var labeledGood1 = LabeledGood.OfPos(PosId, Label);
            _mockLabeledGoodsRepository.Setup(lg => lg.GetByLabelsAsync(new[] { Label }))
                .Returns(Task.FromResult(new[] { labeledGood1 }.ToList()));
            var takenLabeledGoodsLabels = new[] { Label };
            var putLabeledGoodsLabels = new Collection<string>();

            var result = await _simpleLabeledGoodsMerger.MergeAsync(
                _mockUoW.Object,
                PosId,
                putLabeledGoodsLabels,
                takenLabeledGoodsLabels
            );

            result.Should().BeTrue();
            labeledGood1.PosId.Should().BeNull();
        }
        
        [Fact]
        public async Task MergeAsync_SixteenLabelsArePutAndShopContainsSeventeenLabels_ShouldReturnSuccess()
        {
            var putLabels = new[]
            {
                "74 72 61 63 65 00 02 05 7e fb 7c d4 cb 2e e2 80 11 00 20 00 7b 44 df 6f 08 af",
                "74 72 61 63 65 00 02 05 7e fb 7c ed 6c 54 e2 80 11 00 20 00 7b 8d df 6f 08 af",
                "74 72 61 63 65 00 02 05 7e fb 7d dc 79 17 e2 80 11 00 20 00 7b 5c df 6f 08 af",
                "74 72 61 63 65 00 02 05 7e fb 7d de 59 55 e2 80 11 00 20 00 7b 5e df 6f 08 af",
                "74 72 61 63 65 00 02 05 7e fb 85 e1 04 81 e2 80 11 00 20 00 7b 91 df 70 08 af",
                "74 72 61 63 65 00 02 05 7e fb 85 e3 24 c3 e2 80 11 00 20 00 7b 93 df 70 08 af",
                "e2 00 10 63 10 0a 01 17 21 90 30 fd",
                "e2 00 10 63 10 0a 01 19 02 30 f1 b5",
                "e2 00 10 63 10 0a 01 19 02 60 f1 b8",
                "e2 00 10 63 10 0a 01 19 02 70 ef 7d",
                "e2 00 10 63 10 0a 01 19 05 70 db 33",
                "e2 00 10 63 10 0a 01 19 05 80 db 34",
                "e2 00 10 63 10 0a 01 19 05 90 d7 cd",
                "e2 80 11 60 60 00 02 07 14 c5 53 1f e2 80 11 60 20 00 73 1f 98 aa 08 e2",
                "e2 80 11 60 60 00 02 07 14 c5 62 11 e2 80 11 60 20 00 72 11 98 ac 08 e2",
                "e2 80 11 60 60 00 02 07 14 c5 62 1b e2 80 11 60 20 00 72 1b 98 ac 08 e2"
            };
            var takenLabels = new[]
            {
                "e2 80 11 60 60 00 02 07 14 c5 53 1d e2 80 11 60 20 00 63 1d 98 aa 08 e2"
            };
            var counter = 0;
            var posContentLabeledGoods = putLabels
                .Union(takenLabels)
                .Select(l => LabeledGood.NewOfPosBuilder(PosId, l).SetId(++counter).Build())
                .ToList();
            var putLabeledGoods = posContentLabeledGoods
                .Where(lg => putLabels.Contains(lg.Label))
                .ToList();
            var takenLabeledGoods = posContentLabeledGoods
                .Where(lg => takenLabels.Contains(lg.Label))
                .ToList();

            _mockLabeledGoodsRepository.Setup(r => r.GetByLabelsAsync(putLabels))
                .ReturnsAsync(putLabeledGoods);
            _mockLabeledGoodsRepository.Setup(r => r.GetByLabelsAsync(takenLabels))
                .ReturnsAsync(takenLabeledGoods);

            var result = await _simpleLabeledGoodsMerger.MergeAsync(_mockUoW.Object, PosId, putLabels, takenLabels);

            result.Should().BeTrue();
            takenLabeledGoods.ForEach(lg => lg.PosId.Should().BeNull());
            putLabeledGoods.ForEach(lg => lg.PosId.Should().Be(1));
        }
    }
}