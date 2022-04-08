using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Purchase.Conditional.Manager.Contracts;
using NasladdinPlace.Core.Services.Purchase.Conditional.Models;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Check.Refund.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Models;

namespace NasladdinPlace.Core.Services.Purchase.Conditional.Manager
{
    public class ConditionalPurchaseManager : BaseManager, IConditionalPurchaseManager
    {
        public event EventHandler<ConditionalPurchaseManagerResult> OperationCompleted;

        private readonly TimeSpan _conditionalPurchaseOnLabelAppearenceAfterTime;
        private readonly ICheckManager _checkManager;

        public ConditionalPurchaseManager(
            IUnitOfWorkFactory unitOfWorkFactory,
            ICheckManager checkManager,
            TimeSpan conditionalPurchaseOnLabelAppearenceAfterTime) : base (unitOfWorkFactory)
        {
            _conditionalPurchaseOnLabelAppearenceAfterTime = conditionalPurchaseOnLabelAppearenceAfterTime;
            _checkManager = checkManager;
        }

        public async Task DeleteUnverifiedCheckItemsInConditionalPurchasesAsync()
        {
            await ExecuteAsync(async unitOfWork =>
            {
                var conditionalCheckItems = await unitOfWork.CheckItems.GetNotModifiedByAdminUnverifiedAppearedAfterPurchaseAsync(
                    _conditionalPurchaseOnLabelAppearenceAfterTime);

                var checkItemsEditingInfos = conditionalCheckItems.GroupBy(cki => cki.PosOperationId)
                    .Select(group =>
                    {
                        var checkItemsIdsToDelete = group.Select(cki => cki.Id).ToImmutableList();
                        return CheckItemsEditingInfo.ForSystem(group.Key, checkItemsIdsToDelete);
                    });

                foreach (var checkItemsEditingInfo in checkItemsEditingInfos)
                    await _checkManager.RefundOrDeleteItemsAsync(checkItemsEditingInfo);

                NotifyConditionalPurchaseManagerComplete(
                    new ConditionalPurchaseManagerResult(conditionalCheckItems.Count, ConditionalPurchaseOperationType.DeleteUnverified));
            });
        }

        public async Task MarkPurchasedCheckItemsAsUnverifiedIfAppearedAfterPurchaseAsync()
        {
            await ExecuteAsync(async unitOfWork =>
            {
                var remainingСheckItems = await unitOfWork.CheckItems.GetNotModifiedByAdminPaidOrUnpaidAppearedAfterPurchaseAsync(
                    _conditionalPurchaseOnLabelAppearenceAfterTime);

                foreach (var remainingСheckItem in remainingСheckItems)
                {
                    if (remainingСheckItem.Status == CheckItemStatus.Paid)
                    {
                        remainingСheckItem.MarkAsPaidUnverified();
                    }
                    else
                    {
                        remainingСheckItem.MarkAsUnverified();
                    }
                }

                await unitOfWork.CompleteAsync();

                NotifyConditionalPurchaseManagerComplete(new ConditionalPurchaseManagerResult(remainingСheckItems.Count,
                    ConditionalPurchaseOperationType.MarkAsPaidUnverified));
            });
        }

        private void NotifyConditionalPurchaseManagerComplete(ConditionalPurchaseManagerResult conditionalPurchaseManagerResult)
        {
            try
            {
                OperationCompleted?.Invoke(this, conditionalPurchaseManagerResult);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
