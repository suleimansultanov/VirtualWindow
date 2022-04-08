using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.DAL.Contracts;

namespace NasladdinPlace.DAL.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IApplicationDbContext _context;

        public CountryRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Country> GetAsync(int id)
        {
            return _context.Countries.SingleOrDefaultAsync(c => c.Id == id);
        }

        public Task<List<Country>> GetAllAsync()
        {
            return _context.Countries.ToListAsync();
        }

        public void Add(Country country)
        {
            _context.Countries.Add(country);
        }

        public void Remove(Country country)
        {
            _context.Countries.Remove(country);
        }
    }
}
