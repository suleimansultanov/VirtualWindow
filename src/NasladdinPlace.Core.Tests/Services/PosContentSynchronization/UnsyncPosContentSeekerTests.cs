using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Pos.ContentSynchronization;
using NasladdinPlace.Core.Services.Shared.Models;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.PosContentSynchronization
{
    public class UnsyncPosContentSeekerTests
    {
        private const int DefaultPosId = 1;
        private const string Label1 = "74 72 61 63 65 00 02 05 7e fb 7c d4 cb 2e e2 80 11 00 20 00 7b 44 df 6f 08 af";
        private const string Label2 = "74 72 61 63 65 00 02 05 7e fb 7c ed 6c 54 e2 80 11 00 20 00 7b 8d df 6f 08 af";
        
        private readonly UnsyncPosContentSeeker _unsyncPosContentSeeker;
        private readonly Mock<ILabeledGoodRepository> _mockRepository;
        
        public UnsyncPosContentSeekerTests()
        {   
            _mockRepository = new Mock<ILabeledGoodRepository>();
            _mockRepository.Setup(r => r.GetEnabledInPosAsync(DefaultPosId))
                .Returns(Task.FromResult(new List<LabeledGood>()));
            
            var mockUoW = new Mock<IUnitOfWork>();
            mockUoW.SetupGet(u => u.LabeledGoods).Returns(_mockRepository.Object);
            
            _unsyncPosContentSeeker = new UnsyncPosContentSeeker(mockUoW.Object);
        }
        
        [Fact]
        public void Seek_LabelsInPosAndNoLabelsDatabaseAreNotGiven_ShouldReturnSyncResult()
        {
            _mockRepository.Setup(r => r.GetInPosIncludingGoodAndCurrencyAsync(DefaultPosId))
                .Returns(Task.FromResult(new List<LabeledGood>()));

            var posContent = new PosContent(DefaultPosId, Enumerable.Empty<string>());
            var result = _unsyncPosContentSeeker.SeekAsync(posContent).Result;

            result.IsSync.Should().BeTrue();
            result.LabelsInPos.Should().BeEmpty();
            result.LabelsOfPosInDatabase.Should().BeEmpty();
            result.PutLabels.Should().BeEmpty();
            result.TakenLabels.Should().BeEmpty();
        }

        [Fact]
        public void Seek_NoLabelsInPosAndLabelInDatabaseAreGiven_ShouldReturnUnsyncResultWithOneTakenLabel()
        {
            _mockRepository.Setup(r => r.GetInPosIncludingGoodAndCurrencyAsync(DefaultPosId))
                .Returns(Task.FromResult(new List<LabeledGood>
                    {
                        LabeledGood.OfPos(DefaultPosId, Label1)
                    })
                );

            var posContent = new PosContent(DefaultPosId, Enumerable.Empty<string>());
            var result = _unsyncPosContentSeeker.SeekAsync(posContent).Result;

            result.IsSync.Should().BeFalse();
            result.TakenLabels.Should().HaveCount(1);
            result.TakenLabels.First().Should().Be(Label1);
            result.PutLabels.Should().BeEmpty();
            result.LabelsInPos.Should().BeEmpty();
            result.LabelsOfPosInDatabase.Should().HaveCount(1);
            result.LabelsOfPosInDatabase.First().Should().Be(Label1);
        }
        [Fact]
        public void Seek_NoLabelsInPosAndSomeDisabledLabelInDatabaseAreGiven_ShouldReturnUnsyncResultWithOneTakenLabel()
        {
            var labeledGoods = new List<LabeledGood>
            {
                LabeledGood.OfPos(DefaultPosId, Label1)
            }.Select(lg =>
            {
                lg.Disable();
                return lg;
            }).ToList();
            _mockRepository.Setup(r => r.GetInPosIncludingGoodAndCurrencyAsync(DefaultPosId))
                .Returns(Task.FromResult(labeledGoods));

            var posContent = new PosContent(DefaultPosId, Enumerable.Empty<string>());
            var result = _unsyncPosContentSeeker.SeekAsync(posContent).Result;

            result.IsSync.Should().BeFalse();
            result.TakenLabels.Should().HaveCount(1);
            result.TakenLabels.First().Should().Be(Label1);
            result.PutLabels.Should().BeEmpty();
            result.LabelsInPos.Should().BeEmpty();
            result.LabelsOfPosInDatabase.Should().HaveCount(1);
            result.LabelsOfPosInDatabase.First().Should().Be(Label1);
        }

        [Fact]
        public void Seek_LabelInPosAndNoLabelInDatabaseAreGiven_ShouldReturnUnsyncResultWithOnePutLabel()
        {
            _mockRepository.Setup(r => r.GetInPosIncludingGoodAndCurrencyAsync(DefaultPosId))
                .Returns(Task.FromResult(new List<LabeledGood>()));

            var posContent = new PosContent(DefaultPosId, new List<string> { Label1 });
            var result = _unsyncPosContentSeeker.SeekAsync(posContent).Result;

            result.IsSync.Should().BeFalse();
            result.PutLabels.Should().HaveCount(1);
            result.PutLabels.First().Should().Be(Label1);
            result.TakenLabels.Should().BeEmpty();
            result.LabelsOfPosInDatabase.Should().BeEmpty();
            result.LabelsInPos.Should().HaveCount(1);
            result.LabelsInPos.First().Should().Be(Label1);
        }

        [Fact]
        public void Seek_SameLabelInPosAndLabelInDatabaseAreGiven_ShouldReturnSyncResult()
        {
            _mockRepository.Setup(r => r.GetInPosIncludingGoodAndCurrencyAsync(DefaultPosId))
                .Returns(Task.FromResult(new List<LabeledGood>
                    {
                        LabeledGood.OfPos(DefaultPosId, Label1)
                    })
                );

            var posContent = new PosContent(DefaultPosId, new List<string> { Label1 });
            var result = _unsyncPosContentSeeker.SeekAsync(posContent).Result;

            result.IsSync.Should().BeTrue();
            result.TakenLabels.Should().BeEmpty();
            result.PutLabels.Should().BeEmpty();
            result.LabelsInPos.Should().HaveCount(1);
            result.LabelsInPos.First().Should().Be(Label1);
            result.LabelsOfPosInDatabase.Should().HaveCount(1);
            result.LabelsOfPosInDatabase.First().Should().Be(Label1);
        }

        [Fact]
        public void Seek_LabelInPosAndDisabledLabelInDatabaseAreGiven_ShouldReturnSyncResult()
        {
            var labeledGoods = new List<LabeledGood>
            {
                LabeledGood.OfPos(DefaultPosId, Label1)
            }.Select(lg =>
            {
                lg.Disable();
                return lg;
            }).ToList();
            _mockRepository.Setup(r => r.GetInPosIncludingGoodAndCurrencyAsync(DefaultPosId))
                .Returns(Task.FromResult(labeledGoods));

            var posContent = new PosContent(DefaultPosId, new List<string> { Label1 });
            var result = _unsyncPosContentSeeker.SeekAsync(posContent).Result;

            result.IsSync.Should().BeTrue();
            result.TakenLabels.Should().BeEmpty();
            result.PutLabels.Should().BeEmpty();
            result.LabelsInPos.Should().HaveCount(1);
            result.LabelsInPos.First().Should().Be(Label1);
            result.LabelsOfPosInDatabase.Should().HaveCount(1);
            result.LabelsOfPosInDatabase.First().Should().Be(Label1);
        }

        [Fact]
        public void Seek_DifferentLabelInPosAndLabelInDatabaseAreGiven_ShouldReturnSyncResult()
        {
            _mockRepository.Setup(r => r.GetInPosIncludingGoodAndCurrencyAsync(DefaultPosId))
                .Returns(Task.FromResult(new List<LabeledGood>
                    {
                        LabeledGood.OfPos(DefaultPosId, Label1)
                    })
                );

            var posContent = new PosContent(DefaultPosId, new List<string> { Label2 });
            var result = _unsyncPosContentSeeker.SeekAsync(posContent).Result;

            result.IsSync.Should().BeFalse();
            result.TakenLabels.Should().HaveCount(1);
            result.TakenLabels.First().Should().Be(Label1);
            result.PutLabels.Should().HaveCount(1);
            result.PutLabels.First().Should().Be(Label2);
            result.LabelsInPos.Should().HaveCount(1);
            result.LabelsInPos.First().Should().Be(Label2);
            result.LabelsOfPosInDatabase.Should().HaveCount(1);
            result.LabelsOfPosInDatabase.First().Should().Be(Label1);
        }

        [Fact]
        public void Seek_NoLabelsInPosAndSomeLabelsInDatabaseAreGiven_ShouldReturnSyncResultWithTakenLabels()
        {
            _mockRepository.Setup(r => r.GetInPosIncludingGoodAndCurrencyAsync(DefaultPosId))
                .Returns(Task.FromResult(new List<LabeledGood>
                    {
                        LabeledGood.OfPos(DefaultPosId, Label1),
                        LabeledGood.OfPos(DefaultPosId, Label2)
                    })
                );

            var posContent = new PosContent(DefaultPosId, Enumerable.Empty<string>());
            var result = _unsyncPosContentSeeker.SeekAsync(posContent).Result;

            result.IsSync.Should().BeFalse();
            result.TakenLabels.Should().HaveCount(2);
            result.PutLabels.Should().BeEmpty();
            result.LabelsInPos.Should().BeEmpty();
            result.LabelsOfPosInDatabase.Should().HaveCount(2);
        }

        [Fact]
        public void Seek_NoLabelInPosAndDisabledLabelsInDatabaseAreGiven_ShouldReturnSyncResult()
        {
            var labeledGoods = new List<LabeledGood>
                {
                    LabeledGood.OfPos(DefaultPosId, Label1),
                    LabeledGood.OfPos(DefaultPosId, Label2)
                }
                .Select(lg =>
                {
                    lg.Disable();
                    return lg;
                })
                .ToList();
            _mockRepository.Setup(r => r.GetInPosIncludingGoodAndCurrencyAsync(DefaultPosId))
                .Returns(Task.FromResult(labeledGoods));

            var posContent = new PosContent(DefaultPosId, Enumerable.Empty<string>());
            var result = _unsyncPosContentSeeker.SeekAsync(posContent).Result;

            result.IsSync.Should().BeFalse();
            result.TakenLabels.Should().HaveCount(2);
            result.PutLabels.Should().BeEmpty();
            result.LabelsInPos.Should().BeEmpty();
            result.LabelsOfPosInDatabase.Should().HaveCount(2);
        }
    }
}