using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.UI.Controllers;

namespace NasladdinPlace.UI.Services.Authorization
{
    public class AuthorizationPermissionApiHandler : IAuthorizationHandler
    {
        private readonly IAuthTokenManager _authTokenManager;

        public AuthorizationPermissionApiHandler(IAuthTokenManager authTokenManager)
        {
            _authTokenManager = authTokenManager;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (await _authTokenManager.RetrieveAsync() != null)
            {
                context.Succeed(context.Requirements.FirstOrDefault());
            }
            else
            {
                if (!(context.Resource is AuthorizationFilterContext externalContext))
                    return;

                foreach (var cookieKey in externalContext.HttpContext.Request.Cookies.Keys)
                {
                    externalContext.HttpContext.Response.Cookies.Delete(cookieKey);
                }

                externalContext.Result = new RedirectToActionResult(
                    actionName: nameof(AccountController.Login),
                    controllerName: nameof(AccountController).Replace("Controller", string.Empty),
                    routeValues: null
                );
            }
        }
    }
}
