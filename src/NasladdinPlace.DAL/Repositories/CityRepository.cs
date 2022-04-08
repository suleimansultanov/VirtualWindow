using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class CityRepository : Repository<City>, ICityRepository
    {
        public CityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<City> GetAsync(int id)
        {
            return GetAll().SingleOrDefaultAsync(c => c.Id == id);
        }

        public Task<List<City>> GetAllAsync()
        {
            return GetAll().ToListAsync();
        }

        public IQueryable<City> GetAllIncludingCountry()
        {
            return GetAll().Include(c => c.Country);
        }
    }
}
