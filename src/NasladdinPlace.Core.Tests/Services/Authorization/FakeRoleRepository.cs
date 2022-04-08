using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Tests.Services.Authorization
{
    public class FakeRoleRepository : IRoleRepository
    {
        private readonly ConcurrentDictionary<AccessGroup, Role> _roleByAccessGroupDictionary;

        public FakeRoleRepository(ConcurrentDictionary<AccessGroup, Role> roleByAccessGroupDictionary)
        {
            _roleByAccessGroupDictionary = roleByAccessGroupDictionary;
        }

        public Task<Role> GetByNameIncludingPermittedAppFeaturesAsync(string name)
        {
            return null;
        }

        public Task<Role> GetByIdIncludingPermittedAppFeaturesAsync(int id)
        {
            return null;
        }

        public Task<Role> GetByAccessGroupIncludingPermittedAppFeaturesAsync(AccessGroup accessGroup)
        {
            return Task.FromResult(_roleByAccessGroupDictionary[accessGroup]);
        }

        public Task<List<Role>> GetAllAsync()
        {
            return Task.FromResult(new List<Role>());
        }

        public IQueryable<Role> GetAll()
        {
            return new List<Role>().AsQueryable();
        }
    }
}