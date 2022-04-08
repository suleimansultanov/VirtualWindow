using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.UI.ViewModels.Results3Ds;

namespace NasladdinPlace.UI.Controllers
{
    public class Results3DsController : Controller
    {
        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Failure(string error = null)
        {
            return View(new FailureResult3DsViewModel
            {
                Error = error
            });
        }
    }
}