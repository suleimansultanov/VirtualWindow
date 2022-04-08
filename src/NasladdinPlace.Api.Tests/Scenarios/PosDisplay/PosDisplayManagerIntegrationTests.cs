using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.Pos.Display.Agents;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Utils.TasksAgent;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System.Linq;

namespace NasladdinPlace.Api.Tests.Scenarios.PosDisplay
{
    public class PosDisplayManagerIntegrationTests : TestsBase
    {
        private const int DefaultPosId = 1;

        private IPosDisplayManager _posDisplayManager;
        private ITasksAgent _tasksAgent;

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

            _posDisplayManager = serviceProvider.GetRequiredService<IPosDisplayManager>();
            _tasksAgent = serviceProvider.GetRequiredService<ITasksAgent>();
        }

        [Test]
        public void PosDisplay_StartWaitingForSwitchingToDisconnectedPage_ShouldAddSwitchingToDisconnectedPageJobToTaskAgent()
        {
            _posDisplayManager.StartWaitingForSwitchingToDisconnectedPage(DefaultPosId);

            var jobName = $"{nameof(PosDisplayAgent)}_Disconnect_{DefaultPosId}";

            _tasksAgent.GetSchedules().Any(s => s.Name.Contains(jobName)).Should().BeTrue();
        }

        [Test]
        public void PosDisplay_StopWaitingForSwitchingToDisconnectedPageExistingJob_ShouldStopSwitchingToDisconnectedPageJobToTaskAgent()
        {
            _posDisplayManager.StartWaitingForSwitchingToDisconnectedPage(DefaultPosId);
            var jobName = $"{nameof(PosDisplayAgent)}_Disconnect_{DefaultPosId}";

            _posDisplayManager.StopWaitingForSwitchingToDisconnectedPage(DefaultPosId);

            _tasksAgent.GetSchedules()
                       .Any(s => s.Name.Contains(jobName))
                       .Should().BeFalse();
        }

        [Test]
        public void PosDisplay_StartWaitingForSwitchingToDisconnectedPageForTwoDifferentPos_ShouldAddTwoDifferentSwitchingToDisconnectedPageJobToTaskAgent()
        {
            var secondPosId = 2;

            _posDisplayManager.StartWaitingForSwitchingToDisconnectedPage(DefaultPosId);
            _posDisplayManager.StartWaitingForSwitchingToDisconnectedPage(secondPosId);

            var firstJobName = $"{nameof(PosDisplayAgent)}_Disconnect_{DefaultPosId}";
            var secondJobName = $"{nameof(PosDisplayAgent)}_Disconnect_{secondPosId}";

            _tasksAgent.GetSchedules()
                .Any(s => s.Name.Contains(firstJobName))
                .Should().BeTrue();

            _tasksAgent.GetSchedules()
                .Any(s => s.Name.Contains(secondJobName))
                .Should().BeTrue();
        }

        [Test]
        public void PosDisplay_StartSwitchingForTwoDifferentPosAndStopForFirstPos_ShouldAddTwoDifferentJobsStopFirstPosJob()
        {
            var secondPosId = 2;

            _posDisplayManager.StartWaitingForSwitchingToDisconnectedPage(DefaultPosId);
            _posDisplayManager.StartWaitingForSwitchingToDisconnectedPage(secondPosId);

            var firstJobName = $"{nameof(PosDisplayAgent)}_Disconnect_{DefaultPosId}";
            var secondJobName = $"{nameof(PosDisplayAgent)}_Disconnect_{secondPosId}";

            _tasksAgent.GetSchedules()
                .Any(s => s.Name.Contains(firstJobName))
                .Should().BeTrue();

            _posDisplayManager.StopWaitingForSwitchingToDisconnectedPage(DefaultPosId);

            _tasksAgent.GetSchedules()
                .Any(s => s.Name.Contains(firstJobName))
                .Should().BeFalse();

            _tasksAgent.GetSchedules()
                .Any(s => s.Name.Contains(secondJobName))
                .Should().BeTrue();
        }
    }
}
