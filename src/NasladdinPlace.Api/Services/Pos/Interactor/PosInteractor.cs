using System;
using System.Threading.Tasks;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Pos.Doors.Contracts;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.Pos.Interactor.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Pos.RemoteController;
using ILogger = NasladdinPlace.Logging.ILogger;

namespace NasladdinPlace.Api.Services.Pos.Interactor
{
    public class PosInteractor : IPosInteractor
    {
        private readonly IOperationsManager _operationsManager;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPosRemoteControllerFactory _posRemoteControllerFactory;
        private readonly ITelegramChannelMessageSender _telegramChannelMessageSender;
        private readonly IPosDisplayCommandsManager _posDisplayCommandsManager;
        private readonly IPosDoorsStateTracker _posDoorsStateTracker;
        private readonly ILogger _logger;

        public PosInteractor(
            IOperationsManager operationsManager,
            IUnitOfWorkFactory unitOfWorkFactory,
            IPosRemoteControllerFactory posRemoteControllerFactory,
            ITelegramChannelMessageSender telegramChannelMessageSender,
            IPosDisplayCommandsManager posDisplayCommandsManager,
            IPosDoorsStateTracker posDoorsStateTracker,
            ILogger logger)
        {
            if (operationsManager == null)
                throw new ArgumentNullException(nameof(operationsManager));
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (posRemoteControllerFactory == null)
                throw new ArgumentNullException(nameof(posRemoteControllerFactory));
            if (telegramChannelMessageSender == null)
                throw new ArgumentNullException(nameof(telegramChannelMessageSender));
            if (posDisplayCommandsManager == null)
                throw new ArgumentNullException(nameof(posDisplayCommandsManager));
            if (posDoorsStateTracker == null)
                throw new ArgumentNullException(nameof(posDoorsStateTracker));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            
            _operationsManager = operationsManager;
            _unitOfWorkFactory = unitOfWorkFactory;
            _posRemoteControllerFactory = posRemoteControllerFactory;
            _telegramChannelMessageSender = telegramChannelMessageSender;
            _posDisplayCommandsManager = posDisplayCommandsManager;
            _posDoorsStateTracker = posDoorsStateTracker;
            _logger = logger;
        }
        
        public Task<PosInteractionResult> InitiatePosOperationAsync(PosOperationInitiationParams posOperationInitiationParams)
        {
            if (posOperationInitiationParams == null)
                throw new ArgumentNullException(nameof(posOperationInitiationParams));

            return InitiatePosOperationAuxAsync(posOperationInitiationParams);
        }
        
        public async Task<PosInteractionResult> ContinueOperationAsync(int userId)
        {   
            PosOperation latestOperation;
            
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                latestOperation = await GetLatestActiveOperationOfUserAndPos(unitOfWork, userId);
            }

            if (latestOperation == null)
                return PosInteractionResult.Failure(
                    PosInteractionStatus.NoActiveOperationWithUser,
                    $"POS door opening failed because there is no active operation with the user {userId}."
                );

            var posRemoteController = CreatePosRemoteController(latestOperation.PosId);
            await posRemoteController.ContinueOperationAsync();
            
            return PosInteractionResult.Success(latestOperation);
        }

        public Task<PosInteractionResult> TryCompleteOperationAsync(int userId)
        {
            return TryCompleteOperationAuxAsync(userId, completionAction: posOperation => {});
        }

        public Task<PosInteractionResult> TryCompleteOperationAndShowTimerOnDisplayAsync(int userId)
        {
            return TryCompleteOperationAuxAsync(userId, completionAction: async posOperation =>
            {
                var posId = posOperation.PosId;
                await _posDisplayCommandsManager.ShowInventoryTimerAsync(posId);
            });
        }

        public async Task SendOperationCompletionRequestAsync(int posId)
        {
            var posRemoteController = CreatePosRemoteController(posId);

            await _posDisplayCommandsManager.ShowInventoryTimerAsync(posId);
            await posRemoteController.CompleteOperationAsync();
            await _posDoorsStateTracker.NotifyPosDoorsClosedAsync(posId);
        }

        public async Task RequestAccountingBalancesAsync(int posId)
        {
            var posRemoteController = CreatePosRemoteController(posId);
            await posRemoteController.RequestAccountingBalancesAsync();
        }

        public async Task RequestDoorsStateAsync(int posId)
        {
            var posRemoteController = CreatePosRemoteController(posId);
            await posRemoteController.RequestDoorsStateAsync();
        }

        public async Task RequestLogsAsync(int posId, PosLogType logType)
        {
            var posRemoteController = CreatePosRemoteController(posId);
            await posRemoteController.RequestLogsAsync(logType);
        }
        
        public async Task<PosInteractionResult> TryCompleteOperationAuxAsync(int userId, Action<PosOperation> completionAction)
        {
            PosOperation lastPosOperation;
            
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                lastPosOperation = await GetLatestActiveOperationOfUserAndPos(unitOfWork, userId);

                if (lastPosOperation == null)
                {
                    return PosInteractionResult.Failure(
                        PosInteractionStatus.NoActiveOperationWithUser,
                        $"POS door closing failed because there is no active operation with the user {userId}."
                    );
                }

                try
                {
                    lastPosOperation.MarkAsPendingCompletion();
                    await unitOfWork.CompleteAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        $"Unable to mark pos operation {lastPosOperation.Id} as sent for verification because: {ex}."
                    );
                }
            }

            var posId = lastPosOperation.PosId;

            completionAction(lastPosOperation);
            await SendOperationCompletionRequestAsync(posId);

            return PosInteractionResult.Success(lastPosOperation);
        }

        private async Task<PosInteractionResult> InitiatePosOperationAuxAsync(PosOperationInitiationParams posOperationInitiationParams)
        {
            var userId = posOperationInitiationParams.UserId;
            var posId = posOperationInitiationParams.PosId;
            var mode = posOperationInitiationParams.PosMode;
            var brand = posOperationInitiationParams.Brand;
            var doorPosition = posOperationInitiationParams.DoorPosition;

            var operationCreationParams = new OperationCreationParams(userId, posId)
            {
                Brand = brand,
                PosMode = mode
            };
            var operationCreationResult =
                await _operationsManager.TryCreateOperationAsync(operationCreationParams);

            var posRemoteController = CreatePosRemoteController(posId);

            if (!operationCreationResult.Succeeded)
            {
                switch (operationCreationResult.FailureType)
                {
                    case OperationsManagerFailureType.Undefined:
                        return PosInteractionResult.Failure(PosInteractionStatus.UnknownError);
                    case OperationsManagerFailureType.LastPosOperationBelongsToOtherUserOrMode:
                        await ReportOperationCreationFailureDueToPosIncompleteOperationAsync(userId, posId);
                        await posRemoteController.RequestAccountingBalancesAsync();
                        return PosInteractionResult.Failure(PosInteractionStatus.LastPosOperationIncomplete);
                    case OperationsManagerFailureType.LastPosOperationPendingCompletion:
                    case OperationsManagerFailureType.PosModeNotAllowed:    
                        return PosInteractionResult.Failure(PosInteractionStatus.PurchaseNotAllowed);
                    default:
                        return PosInteractionResult.Failure(PosInteractionStatus.UnknownError);
                }
            }

            await _posDisplayCommandsManager.HideQrCodeAndShowActivePurchaseTimerAsync(posId);
            await _posDoorsStateTracker.NotifyPosDoorsOpenedAsync(posId, doorPosition, operationCreationResult.PosOperation.Id);
            await posRemoteController.StartOperationInModeAsync(mode, doorPosition);

            return PosInteractionResult.Success(operationCreationResult.PosOperation);
        }

        private static async Task<PosOperation> GetLatestActiveOperationOfUserAndPos(IUnitOfWork unitOfWork, int userId)
        {
            var latestUserOperation = 
                await unitOfWork.PosOperations.GetLatestUnpaidOfUserAsync(userId);

            if (latestUserOperation == null)
                return null;
            
            var lastPosOperation =
                await unitOfWork.PosOperations.GetLatestActiveOfPosAsync(latestUserOperation.PosId);

            return latestUserOperation.Id != lastPosOperation?.Id 
                ? null 
                : lastPosOperation;
        }

        private IPosRemoteController CreatePosRemoteController(int posId)
        {
            return _posRemoteControllerFactory.Create(posId);
        }

        private async Task ReportOperationCreationFailureDueToPosIncompleteOperationAsync(int userId, int posId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var user = unitOfWork.Users.GetById(userId);
                var pos = await unitOfWork.PointsOfSale.GetByIdAsync(posId);
                var telegramMessage = $"Не удалось создать операцию витрины {pos.AbbreviatedName} " +
                                      $"и пользователя {user.UserName}, т.к. предыдущий пользователь еще не закончил покупку.";

                if (!pos.AreNotificationsEnabled)
                    return;

                await _telegramChannelMessageSender.SendAsync(telegramMessage);
            }
        }
    }
}
