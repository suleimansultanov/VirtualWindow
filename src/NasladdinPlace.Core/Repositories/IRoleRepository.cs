using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IRoleRepository 
    {
        Task<Role> GetByNameIncludingPermittedAppFeaturesAsync(string name);
        Task<Role> GetByIdIncludingPermittedAppFeaturesAsync(int id);
        Task<Role> GetByAccessGroupIncludingPermittedAppFeaturesAsync(AccessGroup accessGroup);
        Task<List<Role>> GetAllAsync();
        IQueryable<Role> GetAll();
    }
}