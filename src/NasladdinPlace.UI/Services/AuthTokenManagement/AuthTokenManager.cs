using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Models;

namespace NasladdinPlace.UI.Services.AuthTokenManagement
{
    public class AuthTokenManager : IAuthTokenManager
    {
        private const string CookieKeyAuthToken = "AuthToken";
        
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenManager(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));
            
            _httpContextAccessor = httpContextAccessor;
        }
        
        public string GetToken()
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[CookieKeyAuthToken];
        }

        public Task<AuthToken> RetrieveAsync()
        {
            var tokenValue = _httpContextAccessor.HttpContext.Request.Cookies[CookieKeyAuthToken];

            return Task.FromResult(tokenValue == null 
                ? null 
                : new AuthToken(tokenValue, expirationInterval: TimeSpan.FromDays(1))
            );
        }

        public Task UpdateAsync(AuthToken authToken)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.Add(authToken.ExpirationInterval),
                HttpOnly = true
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieKeyAuthToken, authToken.Value, cookieOptions);

            return Task.CompletedTask;
        }

        public Task RemoveAuthTokenAsync()
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(CookieKeyAuthToken);

            return Task.CompletedTask;
        }
    }
}
