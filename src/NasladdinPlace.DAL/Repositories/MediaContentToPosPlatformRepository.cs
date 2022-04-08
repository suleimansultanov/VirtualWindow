using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.DAL.Contracts;

namespace NasladdinPlace.DAL.Repositories
{
    public class MediaContentToPosPlatformRepository : IMediaContentToPosPlatformRepository
    {
        private readonly IApplicationDbContext _context;

        public MediaContentToPosPlatformRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MediaContentToPosPlatform> GetLastMediaContentToPosPlatformByScreenType(PosScreenType screenType)
        {
            return await  _context.MediaContentToPosPlatforms
                                  .Include(p => p.MediaContent)
                                  .Where(p => p.PosScreenType == screenType)
                                  .OrderByDescending(p => p.DateTimeCreated)
                                  .FirstOrDefaultAsync();
        }
    }
}
