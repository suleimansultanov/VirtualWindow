using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos.PosDisplay;
using NasladdinPlace.Api.Services.Pos.Display.Agents;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;

namespace NasladdinPlace.Api.Tests.Scenarios.PosDisplay
{
    public class PosDisplayCommandsManagerIntegrationTests : TestsBase
    {
        private const int DefaultPosId = 1;
        private int _commandExecuteCounter = 0;

        private IPosDisplayCommandsManager _posDisplayCommandsManager;
        private IPosDisplayCommandsQueueManager _posDisplayCommandsQueueManager;
        private IPosDisplayAgent _posDisplayAgent;

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

            _posDisplayCommandsManager = serviceProvider.GetRequiredService<IPosDisplayCommandsManager>();
            _posDisplayCommandsQueueManager = serviceProvider.GetRequiredService<IPosDisplayCommandsQueueManager>();

            _posDisplayAgent = serviceProvider.GetRequiredService<IPosDisplayAgent>();
            _posDisplayAgent.OnPosDisplayActionExecuted += (sender, commands) =>
            {
                _commandExecuteCounter++;
            };
            _posDisplayAgent.StartCheckCommandsForRetrySend(TimeSpan.FromSeconds(5));
        }

        [Test]
        public async Task PosDisplay_PerformShowQrCodeCommand_ShouldAddQrCodeCommandToQueue()
        {
            await _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(DefaultPosId);

            var commandsInQueue = _posDisplayCommandsQueueManager.GetAllCommands();
            commandsInQueue.Should().HaveCount(1);

            commandsInQueue.First().CommandContentType.Should().Be(PosDisplayContentType.QrCode);
        }

        [Test]
        public async Task PosDisplay_PerformHideQrCodeCommand_ShouldAddActivePurchaseCommandToQueue()
        {
            await _posDisplayCommandsManager.HideQrCodeAndShowActivePurchaseTimerAsync(DefaultPosId);

            var commandsInQueue = _posDisplayCommandsQueueManager.GetAllCommands();
            commandsInQueue.Should().HaveCount(1);

            commandsInQueue.First().CommandContentType.Should().Be(PosDisplayContentType.ActivePurchase);
        }

        [Test]
        public async Task PosDisplay_PerformShowTimerCommand_ShouldAddInventoryCommandToQueue()
        {
            await _posDisplayCommandsManager.ShowInventoryTimerAsync(DefaultPosId);

            var commandsInQueue = _posDisplayCommandsQueueManager.GetAllCommands();
            commandsInQueue.Should().HaveCount(1);

            commandsInQueue.First().CommandContentType.Should().Be(PosDisplayContentType.Inventory);
        }

        [Test]
        public async Task PosDisplay_PerformShowQrCodeCommandConfirmCommandDelivery_ShouldRemoveQrCodeCommandFromQueue()
        {
            await _posDisplayCommandsManager.ShowInventoryTimerAsync(DefaultPosId);

            var commandsInQueue = _posDisplayCommandsQueueManager.GetAllCommands();
            commandsInQueue.Should().HaveCount(1);

            var command = commandsInQueue.First();
            var deliveredCommand = new PosDisplayCommandDeliveryDto
            {
                PosId = command.PosId,
                CommandId = command.CommandId
            };

            _posDisplayCommandsManager.ConfirmPosDisplayCommandDelivered(deliveredCommand);

            _posDisplayCommandsQueueManager.GetAllCommands()
                                           .Should()
                                           .HaveCount(0);
        }

        [Test]
        public async Task PosDisplay_PerformShowQrCodeCommandForTwoDifferentPos_ShouldAddTwoDifferentQrCodeCommandToQueue()
        {
            var secondPosId = 2;

            await _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(DefaultPosId);
            await _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(posId: secondPosId);

            var commandsInQueue = _posDisplayCommandsQueueManager.GetAllCommands();
            commandsInQueue.Should().HaveCount(2);
        }

        [Test]
        public async Task PosDisplay_PerformCommandsForTwoPosConfirmCommandDeliveryForFirst_ShouldAddTwoDifferentCommandToQueueFirstShouldRemoved()
        {
            var secondPosId = 2;

            await _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(DefaultPosId);
            await _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(posId: secondPosId);

            var commandsInQueue = _posDisplayCommandsQueueManager.GetAllCommands();
            commandsInQueue.Should().HaveCount(2);

            var firstCommand = commandsInQueue.First();

            var deliveredCommand = new PosDisplayCommandDeliveryDto
            {
                CommandId = firstCommand.CommandId,
                PosId = firstCommand.PosId
            };

            _posDisplayCommandsManager.ConfirmPosDisplayCommandDelivered(deliveredCommand);

            _posDisplayCommandsQueueManager.GetAllCommands()
                                           .Should()
                                           .HaveCount(1);
        }

        [Test]
        public async Task PosDisplay_PerformShowQrCodeCommandNoCommandDeliveryConfirmation_CommandShouldExecutedMultipleTimes()
        {
            _commandExecuteCounter = 0;

            var waitHandle = new AutoResetEvent(false);

            await _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(DefaultPosId);

            waitHandle.WaitOne(TimeSpan.FromSeconds(30));

            _commandExecuteCounter.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task PosDisplay_PerformShowQrCodeCommandMultipleTimesAndConfirmCommandDelivery_CommandShouldExecutedMultipleTimesAndRemoveQrCodeCommandFromQueue()
        {
            var waitHandle = new AutoResetEvent(false);

            _commandExecuteCounter = 0;
            await _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(DefaultPosId);

            waitHandle.WaitOne(TimeSpan.FromSeconds(30));

            _commandExecuteCounter.Should().BeGreaterThan(0);
            
            var commandsInQueue = _posDisplayCommandsQueueManager.GetAllCommands();
            commandsInQueue.Should().HaveCount(1);

            var command = commandsInQueue.First();
            
            var deliveredCommand = new PosDisplayCommandDeliveryDto
            {
                CommandId = command.CommandId,
                PosId = command.PosId
            };

            _posDisplayCommandsManager.ConfirmPosDisplayCommandDelivered(deliveredCommand);

            _posDisplayCommandsQueueManager.GetAllCommands()
                .Should()
                .HaveCount(0);
        }

        [Test]
        public async Task PosDisplay_PerformHideQrCodeCommandAndThenShowQrCodeCommand_OnlyLastPosCommandShouldBeInQueue()
        {
            var waitHandle = new AutoResetEvent(false);

            _commandExecuteCounter = 0;
            await _posDisplayCommandsManager.HideQrCodeAndShowActivePurchaseTimerAsync(DefaultPosId);

            waitHandle.WaitOne(TimeSpan.FromSeconds(30));

            _commandExecuteCounter.Should().BeGreaterThan(0);

            await _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(DefaultPosId);

            var commandsInQueue = _posDisplayCommandsQueueManager.GetAllCommands();
            commandsInQueue.Should().HaveCount(1);

            var command = commandsInQueue.First();
            command.CommandContentType.Should().Be(PosDisplayContentType.QrCode);
        }
    }
}
