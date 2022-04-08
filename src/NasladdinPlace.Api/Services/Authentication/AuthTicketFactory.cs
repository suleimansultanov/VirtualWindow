using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NasladdinPlace.Api.Services.Authentication.Contracts;
using NasladdinPlace.Api.Services.Authentication.Models;
using NasladdinPlace.Core.Models;
using OpenIddict.Abstractions;

namespace NasladdinPlace.Api.Services.Authentication
{
    public class AuthTicketFactory : IAuthTicketFactory
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<IdentityOptions> _identityOptions;

        public AuthTicketFactory(
            SignInManager<ApplicationUser> signInManager,
            IOptions<IdentityOptions> identityOptions)
        {
            _signInManager = signInManager;
            _identityOptions = identityOptions;
        }
        
        public async Task<AuthenticationTicket> CreateAsync(TicketInitParams ticketInitParams)
        {
             var principal = await _signInManager.CreateUserPrincipalAsync(ticketInitParams.User);

            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(principal, 
                ticketInitParams.AuthenticationProperties,
                OpenIdConnectServerDefaults.AuthenticationScheme);

            if (ticketInitParams.GrantType != GrantType.RefreshToken)
            {
                // Set the list of scopes granted to the client application.
                // Note: the offline_access scope must be granted
                // to allow OpenIddict to return a refresh token.
                ticket.SetScopes(new[]
                {
                    OpenIdConnectConstants.Scopes.OpenId,
                    OpenIdConnectConstants.Scopes.Email,
                    OpenIdConnectConstants.Scopes.Profile,
                    OpenIdConnectConstants.Scopes.OfflineAccess,
                    OpenIddictConstants.Scopes.Roles
                }.Intersect(ticketInitParams.Scopes));
            }

            ticket.SetResources("resource_server");

            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
                {
                    continue;
                }

                var destinations = new List<string>
                {
                    OpenIdConnectConstants.Destinations.AccessToken
                };

                // Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
                // The other claims will only be added to the access_token, which is encrypted when using the default format.
                if (claim.Type == OpenIdConnectConstants.Claims.Name && ticket.HasScope(OpenIdConnectConstants.Scopes.Profile) ||
                    claim.Type == OpenIdConnectConstants.Claims.Email && ticket.HasScope(OpenIdConnectConstants.Scopes.Email) ||
                    claim.Type == OpenIdConnectConstants.Claims.Role && ticket.HasScope(OpenIddictConstants.Claims.Roles))
                {
                    destinations.Add(OpenIdConnectConstants.Destinations.IdentityToken);
                }

                claim.SetDestinations(destinations);
            }

            return ticket;
        }
    }
}