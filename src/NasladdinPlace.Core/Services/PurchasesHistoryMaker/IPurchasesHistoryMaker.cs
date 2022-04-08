using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PurchasesHistoryMaker
{
    public interface IPurchasesHistoryMaker
    {
        Task<Models.PurchaseHistory> MakeNonEmptyChecksForUserAsync(int userId);
        Task<Models.PurchaseHistory> MakeChecksWithItemsForUserAsync(int userId);
    }
}