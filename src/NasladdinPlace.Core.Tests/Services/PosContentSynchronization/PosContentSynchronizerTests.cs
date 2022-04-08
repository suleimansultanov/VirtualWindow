using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.LabeledGoodsMerger.Contracts;
using NasladdinPlace.Core.Services.Pos.ContentSynchronization;
using NasladdinPlace.Core.Services.Shared.Models;
using Xunit;
using ILogger = NasladdinPlace.Logging.ILogger;

namespace NasladdinPlace.Core.Tests.Services.PosContentSynchronization
{
    public class PosContentSynchronizerTests
    {
        private const int DefaultPosId = 1;
        
        private readonly PosContentSynchronizer _posContentSynchronizer;
        private readonly Mock<IUnsyncPosContentSeeker> _mockUnsyncPosContentSeeker;
        private readonly Mock<ILabeledGoodsMerger> _mockLabeledGoodsMerger;
        private readonly Mock<IUnitOfWork> _mockUoW;

        public PosContentSynchronizerTests()
        {
            _mockUoW = new Mock<IUnitOfWork>();
            _mockUoW.Setup(p => p.PosRealTimeInfos.GetById(DefaultPosId))
                .Returns(new PosRealTimeInfo(DefaultPosId));

            _mockUnsyncPosContentSeeker = new Mock<IUnsyncPosContentSeeker>();
            var mockUnsyncPosContnetSeekerFactory = new Mock<UnsyncPosContentSeekerFactory>();
            var mockLogger = new Mock<ILogger>();

            mockUnsyncPosContnetSeekerFactory
                .Setup(f => f.Create(It.IsAny<IUnitOfWork>()))
                .Returns(_mockUnsyncPosContentSeeker.Object);

            _mockLabeledGoodsMerger = new Mock<ILabeledGoodsMerger>();

            _posContentSynchronizer = new PosContentSynchronizer(
                _mockLabeledGoodsMerger.Object,
                mockUnsyncPosContnetSeekerFactory.Object,
                mockLogger.Object
            );
        }

        [Fact]
        public void Sync_NoLabelInPosAndNoLabelGoodsInStorageAreGiven_ShouldReturnSyncResult()
        {
            var emptyStringList = Enumerable.Empty<string>().ToList();


            _mockUnsyncPosContentSeeker.Setup(s => s.SeekAsync(It.IsAny<PosContent>()))
                .Returns(Task.FromResult(new SyncResult(
                    emptyStringList, 
                    emptyStringList, 
                    emptyStringList, 
                    emptyStringList))
                );

            _mockLabeledGoodsMerger
                .Setup(m => m.MergeAsync(
                    It.IsAny<IUnitOfWork>(), 
                    DefaultPosId, 
                    It.IsAny<ICollection<string>>(), 
                    It.IsAny<ICollection<string>>())
                )
                .Returns(Task.FromResult(true));

            var unitOfWork = _mockUoW.Object;

            var posContent = new PosContent(DefaultPosId, emptyStringList);
            var result = _posContentSynchronizer.SyncAsync(unitOfWork, posContent).Result;
            result.IsSync.Should().BeTrue();
            result.LabelsInPos.Should().BeEmpty();
            result.LabelsOfPosInDatabase.Should().BeEmpty();
            result.PutLabels.Should().BeEmpty();
            result.TakenLabels.Should().BeEmpty();
        }
    }
}