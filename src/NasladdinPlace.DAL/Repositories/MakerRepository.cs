using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class MakerRepository : Repository<Maker>, IMakerRepository
    {
        public MakerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<Maker> GetAsync(int id)
        {
            return GetAll().FirstOrDefaultAsync(m => m.Id == id);
        }

        public Task<List<Maker>> GetAllAsync()
        {
            return GetAll().ToListAsync();
        }

        public void Remove(Maker maker)
        {
            Context.Makers.Remove(maker);
        }

        public Task<Maker> GetByNameAsync(string name)
        {
            return GetAll().FirstOrDefaultAsync(m => m.Name == name);
        }
    }
}
