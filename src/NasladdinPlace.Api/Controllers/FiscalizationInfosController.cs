using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Application.Services.FiscalizationInfos.Contracts;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [AllowAnonymous]
    public class FiscalizationInfosController : Controller
    {
        private readonly IFiscalizationInfosService _fiscalizationInfosService;

        public FiscalizationInfosController(IFiscalizationInfosService fiscalizationInfosService)
        {
            _fiscalizationInfosService = fiscalizationInfosService;
        }
        
        [HttpGet("{fiscalizationInfoId}/qrCode")]
        public async Task<IActionResult> GetQrCodeAsync(int fiscalizationInfoId)
        {
            var qrCodeStreamResult = await _fiscalizationInfosService.GetQrCodeByFicalizationInfoIdAsync(fiscalizationInfoId);
            return qrCodeStreamResult.Succeeded
                ? (IActionResult) new FileStreamResult(qrCodeStreamResult.Value.Value, qrCodeStreamResult.Value.MimeType)
                : BadRequest(qrCodeStreamResult.Error);
        }
    }
}