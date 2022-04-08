using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos.AccountingBalances;
using NasladdinPlace.Api.Dtos.LabeledGood;
using NasladdinPlace.Api.Dtos.PurchaseCompletionResult;
using NasladdinPlace.Api.Services.Pos.Display.Managers;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Services.Documents.Creators.Conctracts;
using NasladdinPlace.Core.Services.HardToDetectLabels;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Models;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Printer.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;
using NasladdinPlace.Core.Services.Pos.ContentSynchronization;
using NasladdinPlace.Core.Services.Pos.LabeledGoodsCreator;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Models;
using NasladdinPlace.Core.Services.Shared.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILogger = NasladdinPlace.Logging.ILogger;

namespace NasladdinPlace.Api.Services.WebSocket.Controllers
{
    public class AccountingBalancesController : WsController
    {
        private readonly ILogger _logger;
        private readonly INasladdinWebSocketMessageSender _webSocketMessageSender;
        private readonly IPosContentSynchronizer _posContentSynchronizer;
        private readonly IPosLabeledGoodsFromLabelsCreator _posLabeledGoodsFromLabelsCreator;
        private readonly IOperationsManager _operationsManager;
        private readonly IPurchaseManager _purchaseManager;
        private readonly IHardToDetectLabelsManager _hardToDetectLabelsManager;
        private readonly IPosLabeledGoodsBlocker _posLabeledGoodsBlocker;
        private readonly IPosDisplayCommandsManager _posDisplayCommandsManager;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDocumentGoodsMovingCreator _documentGoodsMovingCreator;
        private readonly IUntiedLabeledGoodsInfoMessagePrinter _untiedLabeledGoodsPrinter;
        private readonly ITelegramChannelMessageSender _telegramMessageSender;

        public AccountingBalancesController(
            IServiceProvider serviceProvider,
            INasladdinWebSocketMessageSender webSocketMessageSender)
        {
            _webSocketMessageSender = webSocketMessageSender;
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _posContentSynchronizer = serviceProvider.GetRequiredService<IPosContentSynchronizer>();
            _posLabeledGoodsFromLabelsCreator = serviceProvider.GetRequiredService<IPosLabeledGoodsFromLabelsCreator>();
            _operationsManager = serviceProvider.GetRequiredService<IOperationsManager>();
            _purchaseManager = serviceProvider.GetRequiredService<IPurchaseManager>();
            _hardToDetectLabelsManager = serviceProvider.GetRequiredService<IHardToDetectLabelsManager>();
            _posLabeledGoodsBlocker = serviceProvider.GetRequiredService<IPosLabeledGoodsBlocker>();
            _posDisplayCommandsManager = serviceProvider.GetRequiredService<IPosDisplayCommandsManager>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _documentGoodsMovingCreator = serviceProvider.GetRequiredService<IDocumentGoodsMovingCreator>();
            _untiedLabeledGoodsPrinter = serviceProvider.GetRequiredService<IUntiedLabeledGoodsInfoMessagePrinter>();
            _telegramMessageSender = serviceProvider.GetRequiredService<ITelegramChannelMessageSender>();
        }

        public async Task Synchronize(PosAccountingBalancesDto posAccountingBalancesDto)
        {
            if (!posAccountingBalancesDto.PosId.HasValue)
                return;

            var posId = posAccountingBalancesDto.PosId.Value;

            await _webSocketMessageSender.ConfirmCommandDelivery(posId, posAccountingBalancesDto.CommandId);

            var labels = posAccountingBalancesDto.Labels;

            try
            {
                var posContent = new PosContent(posId, labels);

                _logger.LogFormattedInfo(
                    $"{nameof(Synchronize)}: POS id: {posId}. Labels number: {{0}}.", posAccountingBalancesDto);

                await ProcessPosContentAsync(posContent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Synchronize)}: Some error has occurred: {ex}.");
            }
        }

        public async Task NotifyHardToDetectFound(PosLostAndFoundAccountingBalancesDto posLostAndFoundAccountingBalancesDto)
        {
            if (!posLostAndFoundAccountingBalancesDto.PosId.HasValue)
                return;

            _logger.LogFormattedInfo($"{nameof(NotifyHardToDetectFound)} initiated with parameters: {{0}}",
                posLostAndFoundAccountingBalancesDto);

            var posId = posLostAndFoundAccountingBalancesDto.PosId.Value;

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);

                posRealTimeInfo.HardToDetectLabels = posLostAndFoundAccountingBalancesDto.All;

                await unitOfWork.CompleteAsync();

                var lostPosContent = new PosContent(posId, posLostAndFoundAccountingBalancesDto.LostLabels);
                var foundPosContent = new PosContent(posId, posLostAndFoundAccountingBalancesDto.FoundLabels);
                await _hardToDetectLabelsManager.MarkAsLostAsync(unitOfWork, lostPosContent);
                await _hardToDetectLabelsManager.MarkAsFoundAsync(unitOfWork, foundPosContent);

                var posContentToBlock = new PosContent(posId, posLostAndFoundAccountingBalancesDto.All);
                await BlockLabelsAsync(unitOfWork, posContentToBlock);
            }
        }

        private async Task<ValueResult<PosOperation>> ClosePosOperationAsync(IUnitOfWork unitOfWork, int posId)
        {
            var closingOperationResult = await _operationsManager.CloseLatestPosOperationAsync(unitOfWork, posId);

            return closingOperationResult;
        }

        private async Task TryCompletePurchaseAsync(IUnitOfWork unitOfWork, PosOperation operation, SyncResult syncResult)
        {
            var posId = operation.PosId;

            _logger.LogInfo($"Start method {nameof(AccountingBalancesController)}.{nameof(TryCompletePurchaseAsync)}. " +
                                $"POS id: {posId} operation id: {operation.Id}.");

            var user = operation.User;

            var purchaseCompletionResult =
                _purchaseManager.CompleteLastUnpaidAsync(unitOfWork, new PurchaseOperationParams(user.Id)).GetAwaiter().GetResult();

            var purchaseCompletionResultDto = Mapper.Map<PurchaseCompletionResultDto>(purchaseCompletionResult);

            _logger.LogFormattedInfo(
                $"{nameof(AccountingBalancesController)}.{nameof(TryCompletePurchaseAsync)} completed with result: {{0}}",
                purchaseCompletionResultDto);

            var putLabels = syncResult.PutLabels.ToList();

            var posPutContent = new PosContent(posId, putLabels);
            await _hardToDetectLabelsManager.MarkAsFoundAsync(unitOfWork, posPutContent);
            await BlockLabelsAsync(unitOfWork, posPutContent);

            _logger.LogInfo($"The method {nameof(AccountingBalancesController)}.{nameof(TryCompletePurchaseAsync)} has been successfully finished. " +
                                $"POS id: {posId} operation id: {operation.Id}.");
        }

        private async Task FindAndSendUntiedFromGoodLabeledGoodsAsync(IUnitOfWork unitOfWork, int posId)
        {
            var untiedLabeledGoods = await unitOfWork.LabeledGoods.GetEnabledUntiedFromGoodByPos(posId);
            var unitedLabeledGoodsDtos = untiedLabeledGoods.Select(Mapper.Map<LabeledGoodDto>);

            _logger.LogFormattedInfo("Untied labeled goods: {0}", unitedLabeledGoodsDtos);

            await _webSocketMessageSender.SendUntiedLabeledGoodsAsync(posId, unitedLabeledGoodsDtos);
        }

        private Task SendQrCodeAsync(int posId)
        {
            return _posDisplayCommandsManager.GenerateAndShowQrCodeAsync(posId);
        }

        private void LogPosSyncResult(int posId, SyncResult syncResult)
        {
            var logMessage = $"{nameof(Synchronize)} [Additional log]: " +
                             $"POS's content has been successfully synchronized. " +
                             $"POS id: {posId}. " +
                             $"Put labels: {{0}}. " +
                             $"Taken labels: {{1}}";

            _logger.LogFormattedInfo(logMessage, syncResult.PutLabels, syncResult.TakenLabels);
        }

        private async Task UpdatePosRealTimeInfoAsync(IUnitOfWork unitOfWork, PosContent posContent)
        {
            var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posContent.PosId);
            posRealTimeInfo.LabelsNumber = posContent.Labels.Count;
            await unitOfWork.CompleteAsync();
        }

        private async Task BlockLabelsAsync(IUnitOfWork unitOfWork, PosContent posContentToBlock)
        {
            if (!posContentToBlock.Any())
                return;

            await _posLabeledGoodsBlocker.BlockAsync(unitOfWork, posContentToBlock);
        }

        private async Task ProcessPosContentAsync(PosContent posContent)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                await ProcessPosContentAsync(unitOfWork, posContent);
            }
        }

        private async Task ProcessPosContentAsync(IUnitOfWork unitOfWork, PosContent posContent)
        {
            var posId = posContent.PosId;

            var latestActivePosOperation = await unitOfWork.PosOperations.GetLatestActiveOfPosAsync(posId);
            var labeledGoodsAtBegining = new List<GoodsMovingAggregatedItem>();

            if (latestActivePosOperation?.Mode == PosMode.GoodsPlacing)
            {
                labeledGoodsAtBegining = (await unitOfWork.LabeledGoods.GetInPosIncludingGoodAndCurrencyAsync(posId))
                .Select(lg => new GoodsMovingAggregatedItem(lg, BalanceType.AtBegining)).ToList();
            }

            await _posLabeledGoodsFromLabelsCreator.CreateAsync(unitOfWork, posContent);
            var syncResult = await _posContentSynchronizer.SyncAsync(unitOfWork, posContent);

            LogPosSyncResult(posId, syncResult);

            await UpdatePosRealTimeInfoAsync(unitOfWork, posContent);

            var closingOperationResult = await ClosePosOperationAsync(unitOfWork, posId);
            if (!closingOperationResult.Succeeded)
            {
                _logger.LogError(
                    $"Unable to close operation of pos {posId} because {closingOperationResult.Error}."
                );

                await SendQrCodeAsync(posId);

                return;
            }

            var operation = closingOperationResult.Value;

            if (operation?.Mode == PosMode.Purchase)
            {
                await TryCompletePurchaseAsync(unitOfWork, operation, syncResult);
            }
            else if (operation?.Mode == PosMode.GoodsIdentification)
            {
                await FindAndSendUntiedFromGoodLabeledGoodsAsync(unitOfWork, posId);
            }
            else if (operation?.Mode == PosMode.GoodsPlacing)
            {
                var documentGoodsMoving = await _documentGoodsMovingCreator.CreateAsync(labeledGoodsAtBegining, latestActivePosOperation, syncResult, unitOfWork);
                NotifyAboutUntiedLabeledGoods(latestActivePosOperation, documentGoodsMoving.GetItemWithUntiedLabeledGoods());
            }

            await SendQrCodeAsync(posId);

            MarkPosAsAvailableForPurchase(posId);
        }

        private void NotifyAboutUntiedLabeledGoods(PosOperation posOperation, DocumentGoodsMovingTableItem tableItem)
        {
            if (tableItem == null || tableItem.Income == 0)
                return;
            try
            {
                var untiedLabeledGoodsInfo = new UntiedLabeledGoodsInfo(posOperation, tableItem.Income);

                _telegramMessageSender.SendAsync(_untiedLabeledGoodsPrinter.PrintForGoodsMoving(untiedLabeledGoodsInfo)).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while trying notify about untied labeled goods. Error - {ex}");
            }
        }

        private void MarkPosAsAvailableForPurchase(int posId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var posRealmTimeInfo = unitOfWork.PosRealTimeInfos.GetById(posId);
                posRealmTimeInfo.IsPurchaseInProgress = false;
                unitOfWork.CompleteAsync();
            }
        }
    }
}