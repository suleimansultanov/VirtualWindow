using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Core.Services.LabeledGoods.Untied.Manager.Contracts;
using NasladdinPlace.DAL.Constants;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.Admin)]
    public class UntiedLabeledGoodsController : Controller
    {
        private readonly IUntiedLabeledGoodsManager _untiedLabeledGoodsManager;

        public UntiedLabeledGoodsController(IUntiedLabeledGoodsManager untiedLabeledGoodsManager)
        {
            _untiedLabeledGoodsManager = untiedLabeledGoodsManager;
        }

        [HttpPost]
        public IActionResult RunUntiedLabeledGoodsAgent()
        {
            Task.Run(() => _untiedLabeledGoodsManager.FindUntiedLabeledGoodsAsync());

            return Ok();
        }
    }
}