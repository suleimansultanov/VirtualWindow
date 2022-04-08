using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface ICountryRepository
    {
        Task<Country> GetAsync(int id);
        Task<List<Country>> GetAllAsync();
        void Add(Country country);
        void Remove(Country country);
    }
}
