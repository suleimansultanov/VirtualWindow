using Microsoft.AspNetCore.Identity;

namespace NasladdinPlace.Core.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        public ApplicationUser User { get; private set; }
        public Role Role { get; private set; }
    }
}
