using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Infrastructure.IModels;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Check.CommonModels;
using NasladdinPlace.Core.Services.CheckOnline.Helpers;

namespace NasladdinPlace.Core.Models.Fiscalization
{
    [Obsolete("Will be removed in the future releases")]
    public class FiscalizationInfo : Entity
    {
        public Guid RequestId { get; private set; }

        public PosOperation PosOperation { get; private set; }

        public int PosOperationId { get; private set; }

        public int PosId { get; private set; }

        public FiscalizationState State { get; private set; }

        public DateTime DateTimeRequest { get; private set; }

        public DateTime? DateTimeResponse { get; private set; }

        public string ErrorInfo { get; private set; }

        public string DocumentInfo { get; private set; }

        public string FiscalizationNumber { get; private set; }

        public string FiscalizationSerial { get; private set; }

        public string FiscalizationSign { get; private set; }

        public DateTime? DocumentDateTime { get; private set; }

        public decimal? CorrectionAmount { get; private set; }

        public decimal? TotalFiscalizationAmount { get; private set; }

        public ICollection<FiscalizationCheckItem> FiscalizationCheckItems { get; set; }

        public FiscalizationType Type { get; set; }

        public string QrCodeValue { get; private set; }
        
        protected FiscalizationInfo()
        {  
            FiscalizationCheckItems = new Collection<FiscalizationCheckItem>();
        }

        public FiscalizationInfo(int posOperationId, int posId)
        {
            RequestId = Guid.NewGuid();
            State = FiscalizationState.Pending;
            PosOperationId = posOperationId;
            PosId = posId;
            DateTimeRequest = DateTime.UtcNow;            
        }

        public FiscalizationInfo(PosOperation posOperation) 
            : this(posOperation.Id, posOperation.PosId)
        {
            FiscalizationCheckItems = FiscalizationHelper.MakeFiscalizationCheckItems(posOperation);
            Type = FiscalizationType.Income;
        }

        public FiscalizationInfo(
            PosOperation posOperation,
            IEnumerable<CheckItem> checkItems,
            decimal bonusAmount) : this(posOperation.Id, posOperation.PosId)
        {
            FiscalizationCheckItems = FiscalizationHelper.MakeIncomeRefundFiscalizationCheckItems(checkItems, bonusAmount);
            Type = FiscalizationType.IncomeRefund;
        }

        public FiscalizationInfo(PosOperation posOperation, decimal correctionAmount) : this(posOperation.Id, posOperation.PosId)
        {
            Type = FiscalizationType.Correction;
            CorrectionAmount = correctionAmount;
        }
        
        public void SetErrorRequestModelAndMarkAsPending(string errors)
        {
            ErrorInfo = errors;
            State = FiscalizationState.Pending;
        }

        public void MarkAsPendingError(string errorInfo)
        {
            State = FiscalizationState.PendingError;
            DateTimeResponse = DateTime.UtcNow;
            ErrorInfo = errorInfo;
        }
        public void MarkAsInProcess()
        {
            State = FiscalizationState.InProcess;
        }

        public void MarkAsSuccessResponse(CheckOnlineResponse checkOnlineResponse)
        {
            State = FiscalizationState.Success;
            DateTimeResponse = DateTime.UtcNow;
            DocumentInfo = checkOnlineResponse.ReceiptInfo;
            FiscalizationNumber = checkOnlineResponse.FiscalData.Number;
            FiscalizationSerial = checkOnlineResponse.FiscalData.Serial;
            FiscalizationSign = checkOnlineResponse.FiscalData.Sign;
            DocumentDateTime = checkOnlineResponse.DocumentDateTime;
            TotalFiscalizationAmount = checkOnlineResponse.TotalFiscalizationAmount;
            QrCodeValue = checkOnlineResponse.QrCodeValue;
        }

        public void MarkAsCorrectionSuccessResponse(
            string documentInfo,
            string fiscalizationNumber,
            string fiscalizationSerial,
            string fiscalizationSign,
            DateTime? documentDateTime)
        {
            State = FiscalizationState.Success;
            DateTimeResponse = DateTime.UtcNow;
            DocumentInfo = documentInfo;
            FiscalizationNumber = fiscalizationNumber;
            FiscalizationSerial = fiscalizationSerial;
            FiscalizationSign = fiscalizationSign;
            DocumentDateTime = documentDateTime;
        }

        public List<IOnlineCashierProduct> GetCheckOnlineRequestProducts()
        {
            var fiscalizationCheckItems = FiscalizationCheckItems.ToImmutableList();

            return fiscalizationCheckItems.Select(cki => (IOnlineCashierProduct)new CheckOnlineRequestProduct
            {
                Name = cki.GetGoodName(),
                Amount = cki.Amount,
                Count = 1
            }).ToList();
        }

        public CheckFiscalizationInfo ToCheckFiscalizationInfo(string qrCodeLinkFormat, string fiscalCheckLinkFormat)
        {
            if (qrCodeLinkFormat == null)
                throw new ArgumentNullException(nameof(qrCodeLinkFormat));

            var qrCodeLink = string.Format(qrCodeLinkFormat, Id);
            var fiscalCheckLink = string.Format(fiscalCheckLinkFormat, Id);
            
            return new CheckFiscalizationInfo(
                FiscalizationNumber,
                FiscalizationSign,
                DateTimeRequest,
                qrCodeLink,
                QrCodeValue,
                fiscalCheckLink
            );
        }

        public bool IsSuccessfulAndFullOfTypeIncome => Type == FiscalizationType.Income && IsSuccessfulAndFull;

        public bool CheckWhetherSuccessfulAndFullOfTypeIncomeRefundRelatedToCheckItem(int checkItemId)
        {
            return Type == FiscalizationType.IncomeRefund && 
                   IsSuccessfulAndFull && 
                   FiscalizationCheckItems.Any(fci => fci.CheckItemId == checkItemId);
        }

        private bool IsSuccessfulAndFull => 
            State == FiscalizationState.Success 
            && !string.IsNullOrEmpty(FiscalizationNumber)
            && !string.IsNullOrEmpty(FiscalizationSign)
            && !string.IsNullOrEmpty(QrCodeValue)
            && DateTimeRequest != DateTime.MinValue;
    }
}
