using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Infrastructure;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Services.CheckOnline.Helpers;
using NasladdinPlace.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.CheckOnline
{
    public class CheckOnlineManager : BaseManager, ICheckOnlineManager
    {
        private readonly ICheckOnlineBuilder _checkOnlineBuilder;
        private readonly IOnlineCashierAuth _onlineCashierAuth;
        private readonly IPosOperationTransactionTypeProvider _posOperationTransactionTypeProvider;
        private readonly int _taxCode;
        private readonly ILogger _logger;

        public CheckOnlineManager(IUnitOfWorkFactory unitOfWorkFactory,
                                  ICheckOnlineBuilder checkOnlineBuilder,
                                  IOnlineCashierAuth onlineCashierAuth,
                                  IPosOperationTransactionTypeProvider posOperationTransactionTypeProvider,
                                  ILogger logger,
                                  int taxCode) : base(unitOfWorkFactory)
        {
            if (checkOnlineBuilder == null)
                throw new ArgumentNullException(nameof(checkOnlineBuilder));
            if (onlineCashierAuth == null)
                throw new ArgumentNullException(nameof(onlineCashierAuth));
            if (posOperationTransactionTypeProvider == null)
                throw new ArgumentNullException(nameof(posOperationTransactionTypeProvider));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _checkOnlineBuilder = checkOnlineBuilder;
            _onlineCashierAuth = onlineCashierAuth;
            _posOperationTransactionTypeProvider = posOperationTransactionTypeProvider;
            _taxCode = taxCode;
            _logger = logger;
        }

        public async Task MakeFiscalizationAsync(IUnitOfWork unitOfWork, PosOperation posOperation, PosOperationTransaction posOperationTransaction)
        {
            if (!posOperation.CheckItems.Any())
                return;

            var storedFiscalizationInfo =
                await unitOfWork.FiscalizationInfos.GetIncludingFiscalizationCheckItemsByInitialPosOperationIdAsync(
                    posOperation.Id);

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
                    posOperation,
                    posOperationTransaction,
                    posOperationTransaction == null ? PosOperationTransactionType.RegularPurchase : posOperationTransaction.Type);
            }
            else
            {
                var fiscalizationInfo = new FiscalizationInfo(posOperation);
                unitOfWork.FiscalizationInfos.Add(fiscalizationInfo);
                await unitOfWork.CompleteAsync();

                await TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
                    unitOfWork,
                    fiscalizationInfo,
                    posOperation,
                    posOperationTransaction,
                    posOperationTransaction == null ? PosOperationTransactionType.RegularPurchase : posOperationTransaction.Type);
            }
        }

        public async Task MakeReFiscalizationAsync(List<int> fiscalizationInfoIds)
        {
            await ExecuteAsync(async unitOfWork =>
            {
                foreach (var fiscalizationInfoId in fiscalizationInfoIds)
                {
                    var fiscalizationInfo =
                        await unitOfWork.FiscalizationInfos.GetByIdIncludingPosOperationAndFiscalizationCheckItemsAsync(
                            fiscalizationInfoId);

                    if (fiscalizationInfo.State == FiscalizationState.Pending || fiscalizationInfo.State == FiscalizationState.PendingError)
                    {
                        var posOperationTransactionType = _posOperationTransactionTypeProvider.GetTransactionType(fiscalizationInfo.Type);
                        var posOperation = fiscalizationInfo.PosOperation;
                        var operationTransaction = GetPosOperationTransaction(posOperation, posOperationTransactionType);

                        await TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
                            unitOfWork: unitOfWork,
                            fiscalizationInfo: fiscalizationInfo,
                            posOperation: posOperation,
                            operationTransaction: operationTransaction,
                            posOperationTransactionType: posOperationTransactionType);

                    }
                }
            });
        }

        public async Task MakeFiscalizationCorrectionAsync(
            int posOperationId,
            PosOperationTransactionType posOperationTransactionType,
            decimal correctionAmount)
        {

            if (correctionAmount == 0)
                return;

            await ExecuteAsync(async unitOfWork =>
            {
                var posOperation = await unitOfWork.PosOperations.GetIncludingCheckItemsAsync(posOperationId);
                var operationTransaction = GetPosOperationTransaction(posOperation, posOperationTransactionType);

                var fiscalizationInfo = new FiscalizationInfo(posOperation, correctionAmount);
                unitOfWork.FiscalizationInfos.Add(fiscalizationInfo);

                await unitOfWork.CompleteAsync();

                await TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
                    unitOfWork, fiscalizationInfo, posOperation, operationTransaction, posOperationTransactionType);
            });
        }

        public async Task MakeIncomeRefundFiscalizationAsync(int posOperationId, IEnumerable<CheckItem> checkItemsToRefund, decimal bonusAmount)
        {
            await ExecuteAsync(async unitOfWork =>
            {
                var posOperation = await unitOfWork.PosOperations.GetIncludingCheckItemsAsync(posOperationId);
                if (!posOperation.CheckItems.Any() || posOperation.Status != PosOperationStatus.Paid)
                    return;

                var fiscalizationInfo = new FiscalizationInfo(posOperation, checkItemsToRefund, bonusAmount);
                unitOfWork.FiscalizationInfos.Add(fiscalizationInfo);

                await unitOfWork.CompleteAsync();

                var operationTransaction = GetPosOperationTransaction(posOperation, PosOperationTransactionType.Refund);

                await TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
                    unitOfWork,
                    fiscalizationInfo: fiscalizationInfo,
                    posOperation: posOperation,
                    operationTransaction: operationTransaction,
                    posOperationTransactionType: PosOperationTransactionType.Refund);

            });
        }

        private void PerformRequestToCheckOnline(PosOperation posOperation, FiscalizationInfo fiscalizationInfo, PosOperationTransaction operationTransaction = null)
        {
            var checkOnlineRequest = BuildCheckOnlineRequestModel(posOperation, fiscalizationInfo, operationTransaction);
            if (!_checkOnlineBuilder.ValidateRequest(_onlineCashierAuth, checkOnlineRequest, out var errors))
            {
                fiscalizationInfo.SetErrorRequestModelAndMarkAsPending(errors);
                _logger.LogError($"An error has been occurred while doing fiscalization validation. Erorr - {errors}");

                return;
            }

            var baseCheckOnlineResponse = _checkOnlineBuilder.BuildCheck(_onlineCashierAuth, checkOnlineRequest, fiscalizationInfo.Type);
            if (!baseCheckOnlineResponse.IsSuccess)
            {
                fiscalizationInfo.MarkAsPendingError(baseCheckOnlineResponse.Errors);
                _logger.LogError($"An error has been occurred while doing fiscalization. Erorr - {baseCheckOnlineResponse.Errors}");

                return;
            }

            var checkOnlineResponse = (CheckOnlineResponse)baseCheckOnlineResponse;

            fiscalizationInfo.MarkAsSuccessResponse(checkOnlineResponse);
        }

        private void PerformCorrectionRequestToCheckOnline(FiscalizationInfo fiscalizationInfo, PosOperationTransaction operationTransaction, bool useNewPaymentSystem)
        {
            var correctionCheckOnlineRequest = BuildCheckOnlineCorrectionRequestModel(fiscalizationInfo, operationTransaction, useNewPaymentSystem);
            if (!_checkOnlineBuilder.ValidateCorrectionRequest(_onlineCashierAuth, correctionCheckOnlineRequest, out var errors))
            {
                fiscalizationInfo.SetErrorRequestModelAndMarkAsPending(errors);
                _logger.LogError($"An error has been occurred while doing validation of fiscalization correction. Erorr - {errors}");
                return;
            }

            var baseCheckOnlineResponse = _checkOnlineBuilder.BuildCorrectionCheck(_onlineCashierAuth, correctionCheckOnlineRequest);
            if (!baseCheckOnlineResponse.IsSuccess)
            {
                fiscalizationInfo.MarkAsPendingError(baseCheckOnlineResponse.Errors);
                _logger.LogError($"An error has been occurred while doing fiscalization correction. Erorr - {baseCheckOnlineResponse.Errors}");
                return;
            }

            var correctionResponse = (CheckOnlineCorrectionResponse)baseCheckOnlineResponse;
            fiscalizationInfo.MarkAsCorrectionSuccessResponse(
                documentInfo: correctionResponse.ReceiptInfo,
                fiscalizationNumber: correctionResponse.FiscalData.Number,
                fiscalizationSerial: correctionResponse.FiscalData.Serial,
                fiscalizationSign: correctionResponse.FiscalData.Sign,
                documentDateTime: correctionResponse.DocumentDateTime);
        }

        private IOnlineCashierRequest BuildCheckOnlineRequestModel(PosOperation posOperation, FiscalizationInfo fiscalizationInfo, PosOperationTransaction operationTransaction)
        {
            return new CheckOnlineRequest
            {
                InvoiceId = fiscalizationInfo.RequestId,
                ClientPhoneOrEmail = posOperation.User.NormalizedUserName,
                Products = FiscalizationHelper.GetCheckOnlineProducts(posOperation.GetNewPaymentSystemFlag(), fiscalizationInfo, operationTransaction),
                TaxCode = _taxCode
            };
        }

        private IOnlineCashierCorrectionRequest BuildCheckOnlineCorrectionRequestModel(FiscalizationInfo fiscalizationInfo, PosOperationTransaction operationTransaction, bool useNewPaymentSystem)
        {
            return new CheckOnlineCorrectionRequest
            {
                CorrectionAmount = FiscalizationHelper.GetCorrectionAmount(useNewPaymentSystem, fiscalizationInfo, operationTransaction),
                InvoiceId = fiscalizationInfo.RequestId,
                CorrectionReason = new CheckOnlineCorrectionReason
                {
                    DocumentName = "Корректировочный акт",
                    DocumentNumber = fiscalizationInfo.Id.ToString(),
                    DocumentDateTime = fiscalizationInfo.DateTimeRequest
                },
                TaxCode = _taxCode
            };
        }

        private async Task TrySetFiscalizationInfoInProcessStatusAndMakeFiscalizationAsync(
            IUnitOfWork unitOfWork,
            FiscalizationInfo fiscalizationInfo,
            PosOperation posOperation = null,
            PosOperationTransaction operationTransaction = null,
            PosOperationTransactionType posOperationTransactionType = PosOperationTransactionType.RegularPurchase)
        {
            try
            {
                fiscalizationInfo.MarkAsInProcess();
                await unitOfWork.CompleteAsync();

                switch (fiscalizationInfo.Type)
                {
                    case FiscalizationType.Income:
                    case FiscalizationType.IncomeRefund:
                        PerformRequestToCheckOnline(posOperation, fiscalizationInfo, operationTransaction);
                        break;
                    case FiscalizationType.Correction:
                        var useNewPaymentSystemFlag = posOperation?.GetNewPaymentSystemFlag() ?? false;
                        PerformCorrectionRequestToCheckOnline(fiscalizationInfo, operationTransaction, useNewPaymentSystemFlag);
                        break;
                }

                await unitOfWork.CompleteAsync();

                await TrySaveFiscalizationInfoVersionTwoAndAdditionalTransactionRequisites(
                    unitOfWork,
                    fiscalizationInfo,
                    operationTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception is occured while trying set InProcess status to fiscalization info. Exception: {ex}");
            }
        }

        //TODO: return bool or Result when we will use new payment system
        private async Task TrySaveFiscalizationInfoVersionTwoAndAdditionalTransactionRequisites(
            IUnitOfWork unitOfWork,
            FiscalizationInfo fiscalizationInfo,
            PosOperationTransaction operationTransaction = null)
        {
            try
            {
                FiscalizationInfoVersionTwo fiscalizationInfoVersionTwo = null;

                if (operationTransaction != null)
                {
                    fiscalizationInfoVersionTwo = new FiscalizationInfoVersionTwo(operationTransaction.Id, fiscalizationInfo.RequestId);
                    operationTransaction.SetLastFiscalizationInfo(fiscalizationInfoVersionTwo);
                }

                if (fiscalizationInfoVersionTwo != null
                    && (fiscalizationInfo.State == FiscalizationState.Pending || fiscalizationInfo.State == FiscalizationState.PendingError))
                {
                    fiscalizationInfoVersionTwo.MarkAsError(fiscalizationInfo.ErrorInfo);
                }
                else if (fiscalizationInfoVersionTwo != null)
                {
                    operationTransaction.SetAdditionalFiscalizationRequisites(fiscalizationInfo.TotalFiscalizationAmount, fiscalizationInfo.DateTimeResponse);

                    fiscalizationInfoVersionTwo.MarkAsSuccessResponse(fiscalizationInfo);
                    operationTransaction.MarkAsPaidFiscalized();
                }

                await unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying save FiscalizationInfo. Exception: {ex}");
            }
        }

        private static PosOperationTransaction GetPosOperationTransaction(PosOperation posOperation,
            PosOperationTransactionType posOperationTransactionType)
        {
            var getTransactionResult = posOperation.GetTransaction(posOperationTransactionType);
            var operationTransaction = getTransactionResult.Succeeded ? getTransactionResult.Value : null;
            return operationTransaction;
        }
    }
}
