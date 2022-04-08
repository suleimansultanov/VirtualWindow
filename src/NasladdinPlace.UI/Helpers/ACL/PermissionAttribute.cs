using Microsoft.AspNetCore.Mvc;
using System;

namespace NasladdinPlace.UI.Helpers.ACL
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(string permission)
            : base(typeof(AuthorizeResourceFilter))
        {
            Arguments = new object[] { permission };
        }
    }
}
