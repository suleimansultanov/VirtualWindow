using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        Task<List<Currency>> GetAllAsync();
    }
}