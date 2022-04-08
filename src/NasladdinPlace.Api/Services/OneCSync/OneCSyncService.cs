using AutoMapper;
using NasladdinPlace.Api.Dtos.OneCSync;
using NasladdinPlace.Api.Dtos.OneCSync.Base;
using NasladdinPlace.Api.Dtos.OneCSync.GoodsMoving;
using NasladdinPlace.Api.Dtos.OneCSync.InventoryBalances;
using NasladdinPlace.Api.Dtos.OneCSync.Purchases;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Logging;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.OneCSync
{
    public class OneCSyncService : IOneCSyncService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;

        public OneCSyncService(IUnitOfWorkFactory unitOfWorkFactory, ILogger logger)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        public async Task<ValueResult<OneCSyncResult<PosOperationDataDto>>> GetPurchasesListByDateRangeAsync(DateTimeRange dateRange)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var posOperations = await unitOfWork.PosOperations.GetCompletedPosOperationsByPosModeAndDateRangeAsync(PosMode.Purchase, dateRange);

                    var goodInCheck = posOperations
                        .SelectMany(chi => chi.CheckItems)
                        .Select(Mapper.Map<GoodInCheckDto>)
                        .ToList();

                    var checkItemGroup = posOperations
                        .SelectMany(posOperation => posOperation.CheckItems)
                        .GroupBy(chi => chi.PosOperation)
                        .Select(groupItem =>
                        {
                            var dto = Mapper.Map<PosOperationDto>(groupItem);

                            dto.FiscalizationInfos = groupItem.Key.FiscalizationInfos
                                .Where(fi => fi.PosOperationId == dto.Id)
                                .Select(Mapper.Map<FiscalizationInfoOneCSyncDto>)
                                .ToList();

                            dto.BankTransactionInfos = groupItem.Key.BankTransactionInfos
                                .Where(bt => bt.PosOperationId == dto.Id)
                                .Select(Mapper.Map<BankTransactionInfoOneCSyncDto>)
                                .ToList();

                            dto.Goods = goodInCheck
                                .Where(chi =>
                                    chi.ShopId == groupItem.Key.PosId &&
                                    chi.CheckId == groupItem.Key.Id)
                                .ToList();
                            return dto;
                        })
                        .ToList();

                    var goods = posOperations
                        .SelectMany(x =>
                            x.CheckItems.Select(g => g.Good))
                        .Distinct()
                        .Select(Mapper.Map<GoodSyncDto>)
                        .OrderBy(x => x.Id)
                        .ToList();

                    var pointOfSales = posOperations
                        .Select(x => x.Pos)
                        .Distinct()
                        .Select(Mapper.Map<PosSyncDto>)
                        .OrderBy(x => x.Id)
                        .ToList();

                    var operationDataDto = new PosOperationDataDto
                    {
                        PointOfSales = pointOfSales,
                        Goods = goods,
                        Sales = checkItemGroup,
                    };

                    var requestSyncParams = new RequestSyncParam
                    {
                        EndDate = dateRange.End,
                        StartDate = dateRange.Start
                    };

                    var oneCSyncResult =
                        OneCSyncResult<PosOperationDataDto>.ForPurchases(requestSyncParams, operationDataDto);

                    return ValueResult<OneCSyncResult<PosOperationDataDto>>.Success(oneCSyncResult);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while getting purchases list. Error: {ex}";
                return ValueResult<OneCSyncResult<PosOperationDataDto>>.Failure(errorMessage);

            }
        }

        public ValueResult<OneCSyncResult<InventoryBalanceDataDto>> GetInventoryBalances()
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var tiedLabeledGoods = unitOfWork.LabeledGoods.GetAllTiedIncludingGoodAndPosAndCurrencyAndCategory();

                    var pointOfSales = tiedLabeledGoods
                        .Select(lg => lg.Pos)
                        .Distinct()
                        .Select(Mapper.Map<PosSyncDto>)
                        .OrderBy(p => p.Id)
                        .ToList();

                    var goods = tiedLabeledGoods
                        .Select(lg => lg.Good)
                        .Distinct()
                        .Select(Mapper.Map<GoodSyncDto>)
                        .OrderBy(g => g.Id)
                        .ToList();

                    var inventoryBalances = tiedLabeledGoods.GroupBy(
                            lg => new { lg.PosId, lg.GoodId },
                            lg => lg.Label,
                            (group, labelsInGroup) =>
                            {
                                var labels = labelsInGroup.ToList();
                                return new InventoryLabeledGoodDto
                                {
                                    PosId = group.PosId,
                                    GoodId = group.GoodId,
                                    InStock = labels.Count,
                                    Labels = labels
                                };
                            })
                        .ToList();

                    var inventoryBalancesDto = new InventoryBalanceDataDto
                    {
                        PointOfSales = pointOfSales,
                        Goods = goods,
                        InventoryBalances = inventoryBalances
                    };

                    var oneCSyncResult = OneCSyncResult<InventoryBalanceDataDto>.ForInventoryBalances(inventoryBalancesDto);

                    return ValueResult<OneCSyncResult<InventoryBalanceDataDto>>.Success(oneCSyncResult);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while getting inventory balances. Error: {ex}";
                return LogErrorAndReturnFailureResult<OneCSyncResult<InventoryBalanceDataDto>>(errorMessage);
            }
        }

        public async Task<ValueResult<OneCSyncResult<DocumentGoodsMovingDataDto>>> GetDocumentGoodsMovingAsync(DateTimeRange dateRange)
        {
            try
            {
                using (var unitOfwork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var documents = await unitOfwork.DocumentsGoodsMoving.GetByDateRangeIncludingTablePartPosAndGoods(dateRange);

                    var pointOfSales = documents
                        .Select(doc => doc.PointOfSale)
                        .Distinct()
                        .Select(Mapper.Map<PosSyncDto>)
                        .OrderBy(p => p.Id)
                        .ToList();

                    var goods = documents
                        .SelectMany(doc => doc.TablePart.Select(tp => tp.Good))
                        .Distinct()
                        .Select(Mapper.Map<GoodSyncDto>)
                        .OrderBy(g => g?.Id ?? 0)
                        .ToList();

                    var goodsMovingDtos = documents.GroupBy(doc => new { doc.Id, doc.CreatedDate, doc.PosId }, doc => doc.TablePart,
                        (group, tableParts) =>
                        {
                            var tablePart = tableParts.FirstOrDefault();
                            return new GoodsMovingDto
                            {
                                DocumentId = group.Id,
                                DocumentCreatedDate = group.CreatedDate,
                                PosId = group.PosId,
                                Goods = tablePart?.Select(tp =>
                                {
                                    var labelsAtBegignigList = tp.LabelsAtBegining == null ? new List<string>() : tp.LabelsAtBegining.Split(",").ToList();
                                    var labelsAtEndList = tp.LabelsAtEnd == null ? new List<string>() : tp.LabelsAtEnd.Split(",").ToList();

                                    var incomeLabels = labelsAtEndList.Except(labelsAtBegignigList).ToList();
                                    var outcomeLabels = labelsAtBegignigList.Except(labelsAtEndList).ToList();

                                    return new GoodsMovingTablePartDto
                                    {
                                        GoodId = tp.GoodId,
                                        Income = tp.Income,
                                        Outcome = tp.Outcome,
                                        IncomeLabels = incomeLabels,
                                        OutcomeLabels = outcomeLabels
                                    };
                                }).ToList()
                            };
                        }).ToList();

                    var requestSyncParams = new RequestSyncParam
                    {
                        EndDate = dateRange.End,
                        StartDate = dateRange.Start
                    };

                    var documentGoodsMovingDataDto = new DocumentGoodsMovingDataDto
                    {
                        PointOfSales = pointOfSales,
                        Goods = goods,
                        GoodsMoving = goodsMovingDtos

                    };

                    var oneCSyncResult = OneCSyncResult<DocumentGoodsMovingDataDto>.ForGoodsMoving(requestSyncParams, documentGoodsMovingDataDto);

                    return ValueResult<OneCSyncResult<DocumentGoodsMovingDataDto>>.Success(oneCSyncResult);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while getting goods moving documents. Error: {ex}";
                return LogErrorAndReturnFailureResult<OneCSyncResult<DocumentGoodsMovingDataDto>>(errorMessage);
            }
        }

        public async Task<ValueResult<OneCSyncResult<PosOperationVersionTwoDataDto>>> GetVersionTwoPurchasesListByDateRangeAsync(DateTimeRange dateRange)
        {
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var posOperations = await unitOfWork.PosOperations.GetNewBillingCompletedPosOperationsByPosModeAndDateRangeAsync(PosMode.Purchase, dateRange);

                    var goodInCheck = posOperations
                        .SelectMany(chi => chi.CheckItems)
                        .Select(Mapper.Map<GoodInCheckDto>)
                        .ToList();

                    var checkItemGroup = posOperations
                        .SelectMany(posOperation => posOperation.CheckItems)
                        .GroupBy(chi => chi.PosOperation)
                        .Select(groupItem =>
                        {
                            var dto = Mapper.Map<PosOperationVersionTwoDto>(groupItem);

                            dto.Goods = goodInCheck
                                .Where(chi =>
                                    chi.ShopId == groupItem.Key.PosId &&
                                    chi.CheckId == groupItem.Key.Id)
                                .ToList();
                            return dto;
                        })
                        .ToList();

                    var goods = posOperations
                        .SelectMany(x =>
                            x.CheckItems.Select(g => g.Good))
                        .Distinct()
                        .Select(Mapper.Map<GoodSyncDto>)
                        .OrderBy(x => x.Id)
                        .ToList();

                    var pointOfSales = posOperations
                        .Select(x => x.Pos)
                        .Distinct()
                        .Select(Mapper.Map<PosSyncDto>)
                        .OrderBy(x => x.Id)
                        .ToList();

                    var operationDataDto = new PosOperationVersionTwoDataDto
                    {
                        PointOfSales = pointOfSales,
                        Goods = goods,
                        Sales = checkItemGroup,
                    };

                    var requestSyncParams = new RequestSyncParam
                    {
                        EndDate = dateRange.End,
                        StartDate = dateRange.Start
                    };

                    var oneCSyncResult =
                        OneCSyncResult<PosOperationVersionTwoDataDto>.ForPurchases(requestSyncParams, operationDataDto);

                    return ValueResult<OneCSyncResult<PosOperationVersionTwoDataDto>>.Success(oneCSyncResult);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while getting purchases list. Error: {ex}";
                return ValueResult<OneCSyncResult<PosOperationVersionTwoDataDto>>.Failure(errorMessage);

            }
        }

        private ValueResult<T> LogErrorAndReturnFailureResult<T>(string errorMessage) where T : class
        {
            _logger.LogError(errorMessage);
            return ValueResult<T>.Failure(errorMessage);
        }
    }
}