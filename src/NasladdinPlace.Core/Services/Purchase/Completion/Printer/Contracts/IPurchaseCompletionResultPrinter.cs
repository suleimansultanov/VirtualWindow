using NasladdinPlace.Core.Services.Purchase.Completion.Models;

namespace NasladdinPlace.Core.Services.Purchase.Completion.Printer.Contracts
{
    public interface IPurchaseCompletionResultPrinter
    {
        string Print(PurchaseCompletionResult purchaseCompletionResult, string printedCheck);
    }
}
