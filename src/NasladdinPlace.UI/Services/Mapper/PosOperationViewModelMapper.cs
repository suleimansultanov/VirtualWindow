using AutoMapper;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.ViewModels.Checks;
using NasladdinPlace.Utilities.DateTimeConverter;
using System;
using System.Linq;

namespace NasladdinPlace.UI.Services.Mapper
{
    public class PosOperationViewModelMapper : ITypeConverter<PosOperation, PosOperationViewModel>
    {
        public PosOperationViewModel Convert(PosOperation source, PosOperationViewModel destination,
            ResolutionContext context)
        {
            var actualCheckItems = source.CheckItems
                                         .Where(cgi => cgi.Status != CheckItemStatus.Deleted && cgi.Status != CheckItemStatus.Refunded)
                                         .ToList();

            var actualTotalPrice = actualCheckItems.Sum(cki => cki.Price);
            var actualTotalDiscount = actualCheckItems.Sum(cki => cki.RoundedDiscountAmount);

            return new PosOperationViewModel
            {
                UserName = source.User.UserName,
                UserId = source.UserId,
                PosOperationId = source.Id,
                Status = (int) source.Status,
                PosOperationMode = (int) source.Mode,
                HasUnverifiedCheckItems = source.CheckItems.Any(ci => ci.Status == CheckItemStatus.Unverified || ci.Status == CheckItemStatus.PaidUnverified),
                HasFiscalizationInfoErrors = source.PosOperationTransactions.Any(pot => pot.Status == PosOperationTransactionStatus.PaidUnfiscalized),
                PosId = source.PosId,
                PosName = source.Pos.Name,
                FiscalizationState = (int?) (source.FiscalizationInfos != null && source.FiscalizationInfos.Any()
                    ? source.FiscalizationInfos.OrderByDescending(f => f.DateTimeRequest).First().State
                    : (FiscalizationState?) null),
                AuditRequestDateTime = source.AuditRequestDateTime.HasValue
                    ? UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(source.AuditRequestDateTime.Value)
                    : (DateTime?) null,
                AuditCompletionDateTime = source.AuditCompletionDateTime.HasValue
                    ? UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(source.AuditCompletionDateTime.Value)
                    : (DateTime?) null,
                OperationDateTime =
                    UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(
                        source.DatePaid ?? source.DateCompleted ?? source.DateStarted),
                HasDocumentsGoodsMoving = source.DocumentsGoodsMoving.Any(),
                IsDocumentGoodsMovingHasUntiedItems = source.DocumentsGoodsMoving.Any(doc => doc.State == DocumentGoodsMovingState.HasUntiedGood),
                TotalPriceWithoutDiscountAndBonus = Math.Max(actualTotalPrice - actualTotalDiscount - source.BonusAmount, 0),
                CryptogramSources = source.CryptogramSources
            };
        }
    }
}