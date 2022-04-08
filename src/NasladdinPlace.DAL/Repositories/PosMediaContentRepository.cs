using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosMediaContentRepository : IPosMediaContentRepository
    {
        private readonly ApplicationDbContext _context;

        public PosMediaContentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PosMediaContent>> GetByPosAsync(int posId)
        {
            return await _context.PosMediaContents.Where(p => p.PosId == posId).Include(p => p.MediaContent).ToListAsync();
        }

        public async Task<PosMediaContent> GetByPosIdAndScreenImageTypeAsync(int posId, PosScreenType screenType)
        {
            return await _context.PosMediaContents
                .Where(p => p.PosId == posId && p.PosScreenType == screenType)
                .Include(p => p.MediaContent)
                .OrderByDescending(p => p.DateTimeCreated)
                .FirstOrDefaultAsync();
        }

        public async Task<PosMediaContent> GetByPosIdMediaContentIdAsync(int posId, int mediaContentId)
        {
            return await _context.PosMediaContents.SingleOrDefaultAsync(p => p.PosId == posId && p.MediaContentId == mediaContentId);
        }

        public IQueryable<PosMediaContent> GetAll()
        {
            return _context.PosMediaContents;
        }

        public void Add(PosMediaContent posMediaContent)
        {
            _context.PosMediaContents.Add(posMediaContent);
        }

        public void Remove(PosMediaContent posMediaContent)
        {
            _context.PosMediaContents.Remove(posMediaContent);
        }
    }
}
