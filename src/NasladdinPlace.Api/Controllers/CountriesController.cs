using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Country;
using NasladdinPlace.Core;
using NasladdinPlace.DAL.Constants;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class CountriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CountriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> GetAll() =>
            Ok((await _unitOfWork.Countries.GetAllAsync()).Select(Mapper.Map<CountryDto>));
    }
}
