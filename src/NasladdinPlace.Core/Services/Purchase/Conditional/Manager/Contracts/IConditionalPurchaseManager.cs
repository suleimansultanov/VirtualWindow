using NasladdinPlace.Core.Services.Purchase.Conditional.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Purchase.Conditional.Manager.Contracts
{
    public interface IConditionalPurchaseManager
    {
        event EventHandler<ConditionalPurchaseManagerResult> OperationCompleted;

        Task DeleteUnverifiedCheckItemsInConditionalPurchasesAsync();
        Task MarkPurchasedCheckItemsAsUnverifiedIfAppearedAfterPurchaseAsync();
    }
}
