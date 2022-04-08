using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NasladdinPlace.TestUtils.Utils
{
    public static class ControllerContextFactory
    {
        public static ControllerContext MakeForUserWithId(int userId)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                            },
                            "sameAuthTypeName"
                        )
                    )
                }
            };
        }
    }
}