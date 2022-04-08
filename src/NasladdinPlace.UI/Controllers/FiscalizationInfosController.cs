using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.UI.ViewModels.Fiscalization;

namespace NasladdinPlace.UI.Controllers
{
    public class FiscalizationInfosController : Controller
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public FiscalizationInfosController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        [Route("fiscalizationInfos/{fiscalizationInfoId}")]
        public IActionResult Index(int fiscalizationInfoId)
        {
            FiscalizationInfo fiscalizationInfo;

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                fiscalizationInfo = unitOfWork.FiscalizationInfos.GetById(fiscalizationInfoId);
            }

            if (fiscalizationInfo == null)
            {
                return NotFound();
            }
            
            var fiscalizationInfoViewModel = Mapper.Map<FiscalizationInfoViewModel>(fiscalizationInfo);

            return View("Index", fiscalizationInfoViewModel);
        }
    }
}