using System.Linq;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.CommonModels;
using NasladdinPlace.Core.Services.Check.Detailed.Mappers.Contracts;
using NasladdinPlace.Core.Services.Check.Detailed.Models;

namespace NasladdinPlace.Core.Services.Check.Detailed.Mappers
{
    public class DetailedCheckGoodInstanceCreator : IDetailedCheckGoodInstanceCreator
    {
        public DetailedCheckGoodInstance Create(
            CheckItem item,
            CheckFiscalizationInfo detailedCheckFiscalizationInfo,
            PosOperation posOperation,
            string fiscalizationQrCodeUrlTemplate,
            string fiscalCheckUrlTemplate)
        {
            var checkItemStatusInfo = CheckStatusInfo.Unmodified;
            if (item.IsModifiedByAdmin && item.TryGetAuditHistoryRecord(out var auditHistoryRecord))
            {
                var isAdded = auditHistoryRecord.NewStatus == CheckItemStatus.Paid ||
                              auditHistoryRecord.NewStatus == CheckItemStatus.Unpaid;

                checkItemStatusInfo = new CheckStatusInfo(
                    status: isAdded ? SimpleCheckStatus.Added : SimpleCheckStatus.Deleted,
                    dateModified: auditHistoryRecord.CreatedDate);
            }

            var fiscalizationInfo = GetFiscalizationInfo(
                detailedCheckFiscalizationInfo,
                posOperation,
                item,
                fiscalizationQrCodeUrlTemplate,
                fiscalCheckUrlTemplate
            );

            return new DetailedCheckGoodInstance(
                item,
                checkItemStatusInfo,
                fiscalizationInfo
            );
        }

        private CheckFiscalizationInfo GetFiscalizationInfo(
            CheckFiscalizationInfo detailedCheckFiscalizationInfo,
            PosOperation posOperation,
            CheckItem checkItem,
            string fiscalizationQrCodeUrlTemplate,
            string fiscalCheckUrlTemplate)
        {
            switch (checkItem.Status)
            {
                case CheckItemStatus.Paid:
                    return posOperation.FiscalizationInfos != null && posOperation.FiscalizationInfos
                               .Any(fi => fi.Type == FiscalizationType.Income
                                    && fi.State == FiscalizationState.Success
                                    && fi.FiscalizationCheckItems.Any(fci => fci.CheckItemId == checkItem.Id && fci.FiscalizationInfoId == fi.Id))
                        ? detailedCheckFiscalizationInfo
                        : CheckFiscalizationInfo.Empty;
                case CheckItemStatus.Refunded:
                    return posOperation.TryGetFiscalizationInfoByCheckItem(
                        fiscalizationQrCodeUrlTemplate, 
                        fiscalCheckUrlTemplate,
                        checkItem.Id, 
                        out var checkFiscalizationInfo
                    )
                        ? checkFiscalizationInfo
                        : CheckFiscalizationInfo.Empty;
                default:
                    return detailedCheckFiscalizationInfo;
            }
        }
    }
}