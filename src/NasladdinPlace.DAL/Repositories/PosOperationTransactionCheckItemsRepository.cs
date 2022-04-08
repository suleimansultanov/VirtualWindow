using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    class PosOperationTransactionCheckItemsRepository : Repository<PosOperationTransactionCheckItem>, IPosOperationTransactionCheckItemsRepository

    {
        public PosOperationTransactionCheckItemsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
