using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Helpers.ACL;

namespace NasladdinPlace.UI.Controllers
{
    public class CatalogController : ReferenceBaseController
    {

        public CatalogController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [Permission(nameof(GoodCrudPermission))]
        public IActionResult GetCatalogs()
        {
            return View();
        }
    }
}
