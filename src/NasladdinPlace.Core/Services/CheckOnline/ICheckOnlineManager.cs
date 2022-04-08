using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.CheckOnline
{
    public interface ICheckOnlineManager
    {
        Task MakeFiscalizationAsync(IUnitOfWork unitOfWork, PosOperation posOperation, PosOperationTransaction posOperationTransaction);

        Task MakeReFiscalizationAsync(List<int> fiscalizationInfoIds);

        Task MakeFiscalizationCorrectionAsync(int posOperationId, PosOperationTransactionType posOperationTransactionType, decimal correctionAmount);

        Task MakeIncomeRefundFiscalizationAsync(int posOperationId, IEnumerable<CheckItem> checkItemsToRefund, decimal bonusAmount);
    }
}
