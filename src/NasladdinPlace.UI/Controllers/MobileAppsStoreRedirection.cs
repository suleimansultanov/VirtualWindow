using Microsoft.AspNetCore.Mvc;

namespace NasladdinPlace.UI.Controllers
{
    public class MobileAppsStoreRedirection : Controller
    {
        [Route("stores/{qrCode}")]
        public IActionResult Index(string qrCode = null)
        {
            return View();
        }
    }
}