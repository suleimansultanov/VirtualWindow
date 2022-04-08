using System.Security.Claims;

namespace NasladdinPlace.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsNullOrUnauthorized(this ClaimsPrincipal principal)
        {
            return principal == null || !principal.Identity.IsAuthenticated;
        }
    }
}
