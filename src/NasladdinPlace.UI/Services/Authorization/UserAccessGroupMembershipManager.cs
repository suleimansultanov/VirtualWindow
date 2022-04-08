using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.Core.Services.Authorization.Models;
using NasladdinPlace.DAL.Utils;

namespace NasladdinPlace.UI.Services.Authorization
{
    public class UserAccessGroupMembershipManager : IUserAccessGroupMembershipManager
    {
        private readonly IServiceProvider _serviceProvider;

        public UserAccessGroupMembershipManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task<bool> IsMemberAsync(int userId, AccessGroup accessGroup)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var role = RoleToAccessGroupMapper.Map(accessGroup);
                var roleUsers = await userManager.GetUsersInRoleAsync(role);
                return roleUsers.Select(u => u.Id).Contains(userId);
            }
        }

        public async Task GrantMembershipAsync(int userId, AccessGroup accessGroup)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                if (await IsMemberAsync(userId, accessGroup)) return;
                
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var role = RoleToAccessGroupMapper.Map(accessGroup);

                var user = await userManager.FindByIdAsync(userId.ToString());
                await userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task RestrictMembershipAsync(int userId, AccessGroup accessGroup)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                if (!await IsMemberAsync(userId, accessGroup)) return;
                
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var role = RoleToAccessGroupMapper.Map(accessGroup);

                var user = await userManager.FindByIdAsync(userId.ToString());
                await userManager.RemoveFromRoleAsync(user, role);
            }
        }

        public async Task<IEnumerable<AccessGroup>> GetAccessGroupsInWhichUserIsAMemberAsync(int userId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.FindByIdAsync(userId.ToString());
                var roles = await userManager.GetRolesAsync(user);
                return roles.Select(RoleToAccessGroupMapper.Map).ToImmutableList();
            }
        }
    }
}