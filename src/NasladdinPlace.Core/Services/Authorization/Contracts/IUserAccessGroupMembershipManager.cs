using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Services.Authorization.Contracts
{
    public interface IUserAccessGroupMembershipManager
    {
        Task<bool> IsMemberAsync(int userId, AccessGroup accessGroup);
        Task GrantMembershipAsync(int userId, AccessGroup accessGroup);
        Task RestrictMembershipAsync(int userId, AccessGroup accessGroup);
        Task<IEnumerable<AccessGroup>> GetAccessGroupsInWhichUserIsAMemberAsync(int userId);
    }
}