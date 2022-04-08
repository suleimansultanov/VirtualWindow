using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface ICityRepository : IRepository<City>
    {
        Task<City> GetAsync(int id);
        Task<List<City>> GetAllAsync();
        IQueryable<City> GetAllIncludingCountry();
    }
}
