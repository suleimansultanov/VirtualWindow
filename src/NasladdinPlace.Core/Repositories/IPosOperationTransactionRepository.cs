using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosOperationTransactionRepository : IRepository<PosOperationTransaction>
    {
        Task<PosOperationTransaction> GetByIdIncludingBankAndFiscalisationInfosAsync(int id);
    }
}
