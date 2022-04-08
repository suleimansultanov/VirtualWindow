using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos.PosDisplay;
using NasladdinPlace.Api.Services.Pos.Display.Agents.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.Display;

namespace NasladdinPlace.Api.Services.Pos.Display.Managers
{
    public class PosDisplayCommandsManager : IPosDisplayCommandsManager
    {
        private readonly IPosDisplayRemoteController _posDisplayRemoteController;
        private readonly IPosDisplayCommandsQueueManager _posDisplayCommandsQueueManager;
        private Dictionary<PosDisplayContentType, TimeSpan> _retryCommandExecutionTimeDictionary;
        
        public PosDisplayCommandsManager(IPosDisplayRemoteController posDisplayRemoteController,
                                         IPosDisplaySettingsManager posDisplaySettingsManager,
                                         IPosDisplayCommandsQueueManager posDisplayCommandsQueueManager)
        {
            _posDisplayRemoteController = posDisplayRemoteController;
            _posDisplayCommandsQueueManager = posDisplayCommandsQueueManager;

            var posDisplaySettings = posDisplaySettingsManager.GetPosDisplaySettings();
            InitCommandsSettings(posDisplaySettings);
        }

        private void InitCommandsSettings(PosDisplaySettings settings)
        {
            _retryCommandExecutionTimeDictionary = new Dictionary<PosDisplayContentType, TimeSpan>
            {
                { PosDisplayContentType.QrCode, TimeSpan.FromSeconds(settings.RetryShowQrCodeAfterInSeconds) },
                { PosDisplayContentType.ActivePurchase, TimeSpan.FromSeconds(settings.RetryHideQrCodeAfterInSeconds) },
                { PosDisplayContentType.Inventory, TimeSpan.FromSeconds(settings.RetryShowTimerAfterInSeconds) }
            };
        }

        public Task GenerateAndShowQrCodeAsync(int posId)
        {
            return ExecuteCommandWithDeliveryConfirmation(posId, PosDisplayContentType.QrCode);
        }

        public Task HideQrCodeAndShowActivePurchaseTimerAsync(int posId)
        {
            return ExecuteCommandWithDeliveryConfirmation(posId, PosDisplayContentType.ActivePurchase);
        }

        public Task ShowInventoryTimerAsync(int posId)
        {
            return ExecuteCommandWithDeliveryConfirmation(posId, PosDisplayContentType.Inventory);
        }
        
        public void ConfirmPosDisplayCommandDelivered(PosDisplayCommandDeliveryDto deliveredCommand)
        {
            if (!deliveredCommand.PosId.HasValue)
                return;

            _posDisplayCommandsQueueManager.RemoveCommandById(deliveredCommand.CommandId);
        }

        public void RetryExecutePosDisplayCommand(PosDisplayCommand command)
        {
            PerformCommandAsync(command.PosId, command.CommandContentType, command.CommandId);
        }

        private Task ExecuteCommandWithDeliveryConfirmation(int posId, PosDisplayContentType commandType)
        {
            var commandId = Guid.NewGuid();
            var commandTask = PerformCommandAsync(posId, commandType, commandId);

            if (!_retryCommandExecutionTimeDictionary.ContainsKey(commandType))
                return commandTask;

            var executeCommandEvery = _retryCommandExecutionTimeDictionary[commandType];
            var command = new PosDisplayCommand(posId, commandType, executeCommandEvery, commandId);

            _posDisplayCommandsQueueManager.AddCommand(command);

            return commandTask;
        }

        private Task PerformCommandAsync(int posId, PosDisplayContentType contentType, Guid commandId)
        {
            switch (contentType)
            {
                case PosDisplayContentType.QrCode:
                    return _posDisplayRemoteController.GenerateAndShowQrCodeAsync(posId, commandId);
                case PosDisplayContentType.ActivePurchase:
                    return _posDisplayRemoteController.HideQrCodeAsync(posId, commandId);
                case PosDisplayContentType.Check:
                    break;
                case PosDisplayContentType.Inventory:
                    return _posDisplayRemoteController.ShowTimerAsync(posId, commandId);
                case PosDisplayContentType.Disconnect:
                    break;
                case PosDisplayContentType.Refresh:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(contentType), 
                        contentType,
                        $"Content type {contentType} has not been supported yet."
                    );
            }

            return Task.CompletedTask;
        }
    }
}
