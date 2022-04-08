using CloudPaymentsClient.Rest.Dtos.Fiscalization;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.Core.Enums;
using System;

namespace NasladdinPlace.Core.Models.Fiscalization
{
    public class FiscalizationInfoVersionTwo : Entity
    {
        public string RequestId { get; private set; }
        public FiscalizationState State { get; private set; }
        public DateTime RequestDateTime { get; private set; }
        public DateTime? ResponseDateTime { get; private set; }
        public string ErrorInfo { get; private set; }
        public string DocumentInfo { get; private set; }
        public string FiscalizationNumber { get; private set; }
        public string FiscalizationSerial { get; private set; }
        public string FiscalizationSign { get; private set; }
        public string QrCodeValue { get; private set; }
        public DateTime? DocumentDateTime { get; private set; }
        public decimal? CorrectionAmount { get; private set; }
        public decimal? TotalFiscalizationAmount { get; private set; }
        public FiscalizationType Type { get; private set; }
        public int PosOperationTransactionId { get; private set; }
        public PosOperationTransaction PosOperationTransaction { get; private set; }
        public string OfdCheckUrl { get; private set; }

        protected FiscalizationInfoVersionTwo()
        {
            // intentionally left empty
        }

        public FiscalizationInfoVersionTwo(int posOperationTransactionId, FiscalizationType type)
        {
            State = FiscalizationState.Pending;
            PosOperationTransactionId = posOperationTransactionId;
            RequestDateTime = DateTime.UtcNow;
            Type = type;
        }

        public FiscalizationInfoVersionTwo(int posOperationTransactionId, Guid fiscalizationGuidFromOldSystem)
        {
            RequestId = fiscalizationGuidFromOldSystem.ToString();
            State = FiscalizationState.Pending;
            PosOperationTransactionId = posOperationTransactionId;
            RequestDateTime = DateTime.UtcNow;
            Type = FiscalizationType.Income;
        }

        public FiscalizationInfoVersionTwo(string requestId, int posOperationTransactionId)
        {
            RequestId = requestId;
            State = FiscalizationState.Pending;
            PosOperationTransactionId = posOperationTransactionId;
            RequestDateTime = DateTime.UtcNow;
            Type = FiscalizationType.Income;
        }

        public void SetErrorRequestModelAndMarkAsPending(string errors)
        {
            ErrorInfo = errors;
            State = FiscalizationState.Pending;
        }

        public void MarkAsInProcess()
        {
            State = FiscalizationState.InProcess;
        }

        public void MarkAsError(string errorInfo)
        {
            State = FiscalizationState.Error;
            ResponseDateTime = DateTime.UtcNow;
            ErrorInfo = errorInfo;
        }

        //TODO: add validation for fields
        public void MarkAsSuccessResponse(FiscalizationInfo fiscalizationInfo)
        {
            State = FiscalizationState.Success;
            ResponseDateTime = DateTime.UtcNow;
            DocumentInfo = fiscalizationInfo.DocumentInfo;
            QrCodeValue = fiscalizationInfo.QrCodeValue;
            FiscalizationNumber = fiscalizationInfo.FiscalizationNumber;
            FiscalizationSerial = fiscalizationInfo.FiscalizationSerial;
            FiscalizationSign = fiscalizationInfo.FiscalizationSign;
            DocumentDateTime = fiscalizationInfo.DocumentDateTime;
            TotalFiscalizationAmount = fiscalizationInfo.TotalFiscalizationAmount;
            CorrectionAmount = fiscalizationInfo.CorrectionAmount;
            Type = fiscalizationInfo.Type;
        }

        public void MarkAsSuccessResponse(string requestId)
        {
            RequestId = requestId;
        }

        public void MarkAsSuccessReceipt(ReceiptDto receipt)
        {
            State = FiscalizationState.Success;
            ResponseDateTime = DateTime.UtcNow;
            DocumentInfo = receipt.DocumentInfo;
            QrCodeValue = receipt.QrCodeUrl;
            FiscalizationNumber = receipt.DocumentNumber;
            FiscalizationSerial = receipt.FiscalNumber;
            FiscalizationSign = receipt.FiscalSign;
            DocumentDateTime = receipt.DateTime;
            TotalFiscalizationAmount = receipt.Amount;
            OfdCheckUrl = receipt.Url;
            //TODO: what about Correction??? and Type
            Type = FiscalizationType.Income;

            if (PosOperationTransaction != null)
                PosOperationTransaction.SetAdditionalFiscalizationRequisites(TotalFiscalizationAmount, ResponseDateTime);
        }

        //TODO: add validation for fields
        public void MarkAsCorrectionSuccessResponse(string documentInfo,
                                                    string fiscalizationNumber,
                                                    string fiscalizationSerial,
                                                    string fiscalizationSign,
                                                    DateTime? documentDateTime)
        {
            State = FiscalizationState.Success;
            ResponseDateTime = DateTime.UtcNow;
            DocumentInfo = documentInfo;
            FiscalizationNumber = fiscalizationNumber;
            FiscalizationSerial = fiscalizationSerial;
            FiscalizationSign = fiscalizationSign;
            DocumentDateTime = documentDateTime;
        }

        //TODO: Add code here for public List<IOnlineCashierProduct> GetCheckOnlineRequestProducts() when we will use PosOperationTransactionCheckItems
    }
}
