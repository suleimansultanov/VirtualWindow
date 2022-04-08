using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Dtos.Currency;
using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Helpers.ACL;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Route(Routes.Api)]
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class CurrenciesController : BaseController
    {
        public CurrenciesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrenciesAsync()
        {
            var currencyDtos = (await
                UnitOfWork.Currencies.GetAllAsync()).Select(Mapper.Map<CurrencyDto>);

            return Ok(currencyDtos);
        }
    }
}