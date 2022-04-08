using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace NasladdinPlace.UI.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult RedirectToHome(this Controller controller)
        {
            var displayUrl = controller.Request.GetDisplayUrl();
            if (displayUrl.ToLower().Contains("logout"))
                displayUrl = "/";
            return IsUserNotSignedIn(controller) 
                ? controller.RedirectToAction("Login", "Account", new { returnUrl = displayUrl }) 
                : controller.RedirectToAction("All", "PointsOfSale");
        }

        public static bool IsUserNotSignedIn(this Controller controller)
        {
            var context = controller.HttpContext;
            var user = context.User;

            return user?.Claims.FirstOrDefault()?.Value == null;
        }
    }
}
