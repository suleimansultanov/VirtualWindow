using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Authorization.Models;
using NasladdinPlace.DAL.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class RoleRepository: IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Role> GetByNameIncludingPermittedAppFeaturesAsync(string name)
        {
            return GetAll()
                .Include(r => r.InternalPermittedAppFeatures)
                .Include(r=>r.InternalPermittedFeatures)
                .SingleOrDefaultAsync(r => r.NormalizedName == name.ToUpper());
        }

        public Task<Role> GetByIdIncludingPermittedAppFeaturesAsync(int id)
        {
            return GetAll()
                .Include(r => r.InternalPermittedAppFeatures)
                .Include(r => r.InternalPermittedFeatures)
                .Include(r => r.InternalAssignedPoses)
                .SingleOrDefaultAsync(r => r.Id == id);
        }

        public Task<Role> GetByAccessGroupIncludingPermittedAppFeaturesAsync(AccessGroup accessGroup)
        {
            var roleName = RoleToAccessGroupMapper.Map(accessGroup);
            return GetByNameIncludingPermittedAppFeaturesAsync(roleName);
        }

        public Task<List<Role>> GetAllAsync()
        {
            return GetAll()
                .ToListAsync();
        }

        public IQueryable<Role> GetAll()
        {
            return _context.Roles;
        }
    }
}