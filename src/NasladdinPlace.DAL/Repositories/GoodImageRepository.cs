using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.DAL.Contracts;

namespace NasladdinPlace.DAL.Repositories
{
    public class GoodImageRepository : IGoodImageRepository
    {
        private readonly IApplicationDbContext _context;

        public GoodImageRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<GoodImage>> GetByGoodAsync(int goodId)
        {
            return _context.GoodImages
                .Where(gi => gi.GoodId == goodId)
                .ToListAsync();
        }

        public Task<GoodImage> GetAsync(int goodImageId)
        {
            return _context.GoodImages.SingleOrDefaultAsync(gi => gi.Id == goodImageId);
        }

        public void Add(GoodImage goodImage)
        {
            _context.GoodImages.Add(goodImage);
        }

        public void Remove(GoodImage goodImage)
        {
            _context.GoodImages.Remove(goodImage);
        }

        public void Update(GoodImage goodImage)
        {
            _context.GoodImages.Update(goodImage);
        }

        public Task<GoodImage> GetByGoodIdAsync(int goodId)
        {
            return _context.GoodImages
                .FirstOrDefaultAsync(gi => gi.GoodId == goodId);
        }
    }
}
