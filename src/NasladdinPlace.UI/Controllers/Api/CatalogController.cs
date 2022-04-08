using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Services.NasladdinApi;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Route(Routes.Api)]
    public class CatalogController : Controller
    {
        private readonly INasladdinApiClient _nasladdinApiClient;
        public CatalogController(IServiceProvider serviceProvider)
        {
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
        }

        [HttpGet("pointsOfSale")]
        public async Task<IActionResult> GetPointOfSales(int page)
        {
            var pointsOfSale = await _nasladdinApiClient.GetCatalogPointsOfSaleAsync(page);

            return Json(pointsOfSale.Result);
        }

        [HttpGet("posContent")]
        public async Task<IActionResult> GetPosContent(int posId, int page)
        {
            var posContent = await _nasladdinApiClient.GetPosContentAsync(posId, page);

            return Json(posContent.Result);
        }

        public IActionResult GetCategoryContent()
        {
            return Json(new { sucess = true });
        }
    }
}