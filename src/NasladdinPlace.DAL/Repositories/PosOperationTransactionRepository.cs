using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosOperationTransactionRepository: Repository<PosOperationTransaction>, IPosOperationTransactionRepository
    {

        public PosOperationTransactionRepository(ApplicationDbContext context): base(context)
        {
        }

        public Task<PosOperationTransaction> GetByIdIncludingBankAndFiscalisationInfosAsync(int id)
        {
            return GetAll()
                .Include(pot => pot.BankTransactionInfos)
                .Include(pot => pot.FiscalizationInfos)
                .Include(pot => pot.LastBankTransactionInfo)
                .Include(pot => pot.LastFiscalizationInfo)
                .FirstOrDefaultAsync(pot => pot.Id == id);
        }
    }
}
