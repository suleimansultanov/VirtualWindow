using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Services.Pos.Display;
using NasladdinPlace.DAL.Constants;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class PosDisplayController : Controller
    {
        private readonly IPosDisplayRemoteController _posDisplayRemoteController;
        private readonly IUnitOfWork _unitOfWork;
        public PosDisplayController(IPosDisplayRemoteController posDisplayRemoteController, 
            IUnitOfWork unitOfWork)
        {
            _posDisplayRemoteController = posDisplayRemoteController;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("/api/pointsOfSale/{posId}/display/reloading")]
        public async Task<IActionResult> RefreshDisplayPageInPos(int posId)
        {
            await _posDisplayRemoteController.RefreshDisplayPageAsync(posId);

            return Ok();
        }

        [HttpPost("/api/pointsOfSale/all/display/reloading")]
        public async Task<IActionResult> RefreshDisplaysPage()
        {
            var pointsOfSale = await _unitOfWork.PointsOfSale.GetAllAsync();

            Parallel.ForEach(pointsOfSale,
                async pos => { await _posDisplayRemoteController.RefreshDisplayPageAsync(pos.Id); });

            return Ok();
        }
    }
}