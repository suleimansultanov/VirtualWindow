using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Check;
using NasladdinPlace.Core;
using NasladdinPlace.DAL.Constants;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class CurrenciesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CurrenciesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(
            _unitOfWork.Currencies.GetAll()
                .AsEnumerable()
                .Select(Mapper.Map<CurrencyDto>)
        );
    }
}