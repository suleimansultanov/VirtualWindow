using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Extensions
{
    public static class UserManagerExtensions
    {
        public static int GetUserIdAsInt(this UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            return int.Parse(userManager.GetUserId(principal));
        }
    }
}
