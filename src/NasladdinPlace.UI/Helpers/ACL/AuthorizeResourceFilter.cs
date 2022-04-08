using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Authorization.Contracts;
using NasladdinPlace.DAL.Constants;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Helpers.ACL
{
    public class AuthorizeResourceFilter : IAsyncActionFilter
    {
        private readonly IAccessGroupAppFeaturesAccessManager _accessGroupAppFeaturesAccessManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly string _requirement;

        public AuthorizeResourceFilter(
            IAccessGroupAppFeaturesAccessManager accessGroupAppFeaturesAccessManager,
            string requirement,
            RoleManager<Role> roleManager)
        {
            if (accessGroupAppFeaturesAccessManager == null)
                throw new ArgumentNullException(nameof(accessGroupAppFeaturesAccessManager));
            if (string.IsNullOrEmpty(requirement))
                throw new ArgumentNullException(nameof(requirement));
            if (roleManager == null)
                throw new ArgumentNullException(nameof(roleManager));

            _accessGroupAppFeaturesAccessManager = accessGroupAppFeaturesAccessManager;
            _requirement = requirement;
            _roleManager = roleManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var roles = context.HttpContext.User.Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            }).Where(r => r.Type == ClaimTypes.Role);

            foreach (var roleName in roles)
            {
                if (roleName.Value == nameof(Roles.Admin))
                {
                    await next();
                    return;
                }

                var role = await _roleManager.FindByNameAsync(roleName.Value);

                if (role == null)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                var hasAccess = await _accessGroupAppFeaturesAccessManager
                    .IsAccessGrantedAsync(role.Id, _requirement);

                if (hasAccess) continue;

                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
