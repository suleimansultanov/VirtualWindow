using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IMakerRepository : IRepository<Maker>
    {
        Task<Maker> GetAsync(int id);
        Task<List<Maker>> GetAllAsync();
        Task<Maker> GetByNameAsync(string name);
        void Remove(Maker maker);
    }
}
