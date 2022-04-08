using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Fiscalization.Models;
using NasladdinPlace.Fiscalization.Services;
using NasladdinPlace.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;

namespace NasladdinPlace.Core.Services.CloudKassir
{
    public class CloudKassirManager : BaseManager, ICloudKassirManager
    {
        private readonly ICloudKassirService _cloudKassirService;
        private readonly ILogger _logger;
        private readonly string _inn;
        private readonly TaxationSystem _taxationSystem;

        public CloudKassirManager(
            IUnitOfWorkFactory unitOfWorkFactory,
            ICloudKassirService cloudKassirService,
            ILogger logger,
            string inn,
            TaxationSystem taxationSystem) : base(unitOfWorkFactory)
        {
            if (cloudKassirService == null)
                throw new ArgumentNullException(nameof(cloudKassirService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (inn == null)
                throw new ArgumentNullException(nameof(inn));
            if (!Enum.IsDefined(typeof(TaxationSystem), taxationSystem))
                throw new ArgumentException($"Incorrect value {taxationSystem} of the {nameof(TaxationSystem)} enum.");

            _cloudKassirService = cloudKassirService;
            _logger = logger;
            _inn = inn;
            _taxationSystem = taxationSystem;
        }

        public async Task MakeFiscalizationAsync(
            IUnitOfWork unitOfWork,
            PosOperationTransaction posOperationTransaction)
        {
            if (!posOperationTransaction.PosOperationTransactionCheckItems.Any())
                return;

            var storedFiscalizationInfo =
                await unitOfWork.FiscalizationInfosV2.GetIncludingPosOperationTransactionCheckItemsByInitialPosOperationTransactionIdAsync(
                    posOperationTransaction.Id);

            if (storedFiscalizationInfo != null &&
                storedFiscalizationInfo.State == FiscalizationState.InProcess &&
                storedFiscalizationInfo.Type == FiscalizationType.Income)
                return;

            if (storedFiscalizationInfo != null &&
                storedFiscalizationInfo.State == FiscalizationState.Pending &&
                storedFiscalizationInfo.Type == FiscalizationType.Income)
            {
                await TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
                    unitOfWork,
                    storedFiscalizationInfo,
                    posOperationTransaction);
            }
            else
            {
                var fiscalizationInfo = new FiscalizationInfoVersionTwo(posOperationTransaction.Id, FiscalizationType.Income);
                unitOfWork.FiscalizationInfosV2.Add(fiscalizationInfo);
                await unitOfWork.CompleteAsync();

                await TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
                    unitOfWork,
                    fiscalizationInfo,
                    posOperationTransaction);
            }
        }

        public async Task MakeIncomeRefundFiscalizationAsync(int posOperationId, IEnumerable<CheckItem> checkItemsToRefund, decimal bonusAmount)
        {
            await ExecuteAsync(async unitOfWork =>
            {
                var posOperation = await unitOfWork.PosOperations.GetIncludingCheckItemsAsync(posOperationId);
                if (!posOperation.CheckItems.Any() || posOperation.Status != PosOperationStatus.Paid)
                    return;

                var posOperationTransaction = GetPosOperationTransaction(posOperation, PosOperationTransactionType.Refund);

                if (posOperationTransaction == null)
                {
                    _logger.LogError($"Critical error has been occurred while making fiscalization refund. Can not find operation transaction for PosOperationId = {posOperationId}.");
                    return;
                }

                var fiscalizationInfo = new FiscalizationInfoVersionTwo(posOperationTransaction.Id, FiscalizationType.Income);
                unitOfWork.FiscalizationInfosV2.Add(fiscalizationInfo);

                await unitOfWork.CompleteAsync();

                await TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
                    unitOfWork,
                    fiscalizationInfo,
                    posOperationTransaction);
            });
        }

        public async Task MakeReFiscalizationAsync(List<int> fiscalizationInfoIds)
        {
            await ExecuteAsync(async unitOfWork =>
            {
                foreach (var fiscalizationInfoId in fiscalizationInfoIds)
                {
                    var fiscalizationInfo =
                        await unitOfWork.FiscalizationInfosV2.GetByIdIncludingPosOperationTransactionAndPosOperationTransactionCheckItemsAsync(
                            fiscalizationInfoId);

                    if (fiscalizationInfo.State == FiscalizationState.Error || fiscalizationInfo.State == FiscalizationState.PendingError)
                    {
                        await TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
                            unitOfWork,
                            fiscalizationInfo,
                            fiscalizationInfo.PosOperationTransaction);
                    }
                }
            });
        }

        private static PosOperationTransaction GetPosOperationTransaction(PosOperation posOperation,
            PosOperationTransactionType posOperationTransactionType)
        {
            var getTransactionResult = posOperation.GetTransaction(posOperationTransactionType);
            var operationTransaction = getTransactionResult.Succeeded ? getTransactionResult.Value : null;
            return operationTransaction;
        }

        private async Task TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
            IUnitOfWork unitOfWork,
            FiscalizationInfoVersionTwo fiscalizationInfo,
            PosOperationTransaction operationTransaction = null)
        {
            try
            {
                fiscalizationInfo.MarkAsInProcess();
                await unitOfWork.CompleteAsync();

                switch (fiscalizationInfo.Type)
                {
                    case FiscalizationType.Income:
                    case FiscalizationType.IncomeRefund:
                        await PerformRequestToCloudKassirAsync(operationTransaction, fiscalizationInfo);
                        break;
                    //TODO: what about correction
                }

                await unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception is occured while trying set InProcess status to fiscalization info. Exception: {ex}");
            }
        }

        private async Task PerformRequestToCloudKassirAsync(
            PosOperationTransaction operationTransaction,
            FiscalizationInfoVersionTwo fiscalizationInfo)
        {
            try
            {
                var fiscalItems = operationTransaction.GetCloudKassirRequestFiscalItems();
                var amounts = Amounts.CreateForElectronic(fiscalItems.Sum(fi => fi.Amount));
                var customerReceipt = new CustomerReceipt(fiscalItems, _taxationSystem, amounts);
                var fiscalizationType = GetRequestFicalizationType(fiscalizationInfo.Type);
                var fiscalizationRequest = new FiscalizationRequest(_inn, fiscalizationType, customerReceipt);

                var result = await _cloudKassirService.MakeFiscalizationAsync(fiscalizationRequest);

                if (!result.IsSuccess)
                {
                    fiscalizationInfo.MarkAsError(result.Error);
                    _logger.LogError($"An error has been occurred while doing fiscalization. Erorr - {result.Error}");

                    return;
                }

                fiscalizationInfo.MarkAsSuccessResponse(result.Result.Id);

                operationTransaction.SetLastFiscalizationInfo(fiscalizationInfo);
            }
            catch(Exception ex)
            {
                fiscalizationInfo.SetErrorRequestModelAndMarkAsPending(ex.ToString());
                _logger.LogError($"An error has been occurred while preparing data for fiscalization. Erorr - {ex}");
            }
            
        }

        private string GetRequestFicalizationType(FiscalizationType fiscalizationType)
        {
            switch (fiscalizationType)
            {
                case FiscalizationType.Income:
                    return "Income";
                case FiscalizationType.IncomeRefund:
                    return "IncomeReturn";
                default:
                    throw new ArgumentException($"Incorrect value of {nameof(fiscalizationType)}.");
            }
        }

    }
}
