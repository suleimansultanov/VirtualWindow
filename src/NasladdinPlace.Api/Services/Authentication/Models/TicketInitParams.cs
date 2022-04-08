using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Services.Authentication.Models
{
    public class TicketInitParams
    {
        public ApplicationUser User { get; }
        public AuthenticationProperties AuthenticationProperties { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public GrantType GrantType { get; set; }

        public TicketInitParams(ApplicationUser user)
        {
            User = user;
            Scopes = Enumerable.Empty<string>();
            GrantType = GrantType.Password;
        }
    }
}