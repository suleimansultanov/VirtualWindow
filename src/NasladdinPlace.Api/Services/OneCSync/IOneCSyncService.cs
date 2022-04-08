using NasladdinPlace.Api.Dtos.OneCSync;
using NasladdinPlace.Api.Dtos.OneCSync.InventoryBalances;
using NasladdinPlace.Api.Dtos.OneCSync.Purchases;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;
using NasladdinPlace.Api.Dtos.OneCSync.GoodsMoving;

namespace NasladdinPlace.Api.Services.OneCSync
{
    public interface IOneCSyncService
    {
        Task<ValueResult<OneCSyncResult<PosOperationDataDto>>> GetPurchasesListByDateRangeAsync(DateTimeRange dateRange);
        ValueResult<OneCSyncResult<InventoryBalanceDataDto>> GetInventoryBalances();
        Task<ValueResult<OneCSyncResult<DocumentGoodsMovingDataDto>>> GetDocumentGoodsMovingAsync(DateTimeRange dateRange);
        Task<ValueResult<OneCSyncResult<PosOperationVersionTwoDataDto>>> GetVersionTwoPurchasesListByDateRangeAsync(
            DateTimeRange dateRange);
    }
}