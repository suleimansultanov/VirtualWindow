using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.CloudKassir
{
    public interface ICloudKassirManager
    {
        Task MakeFiscalizationAsync(IUnitOfWork unitOfWork, PosOperationTransaction posOperationTransaction);
        Task MakeIncomeRefundFiscalizationAsync(int posOperationId, IEnumerable<CheckItem> checkItemsToRefund, decimal bonusAmount);
        Task MakeReFiscalizationAsync(List<int> fiscalizationInfoIds);
    }
}
