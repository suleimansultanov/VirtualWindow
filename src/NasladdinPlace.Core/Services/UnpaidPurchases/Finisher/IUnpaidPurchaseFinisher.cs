using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.UnpaidPurchases.Finisher
{
    public interface IUnpaidPurchaseFinisher
    {
        Task FinishUnpaidPurchasesAsync(TimeSpan considerUnpaidAfter);
    }
}