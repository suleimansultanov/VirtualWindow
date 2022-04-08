using NasladdinPlace.Core.Services.Authorization.Models;
using NasladdinPlace.DAL.Constants;

namespace NasladdinPlace.DAL.Utils
{
    public class RoleToAccessGroupMapper
    {
        private RoleToAccessGroupMapper()
        {
            // intentionally left empty
        }
        
        public static string Map(AccessGroup accessGroup)
        {
            switch (accessGroup)
            {
                case AccessGroup.Admin:
                    return Roles.Admin;
                default:
                    return Roles.Logistician;
            }
        }

        public static AccessGroup Map(string role)
        {
            switch (role)
            {
                 case Roles.Admin:
                     return AccessGroup.Admin;
                 default:
                     return AccessGroup.Logistician;
            }
        }
        
    }
}