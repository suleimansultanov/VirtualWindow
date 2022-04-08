using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core;
using Newtonsoft.Json;
using System;

namespace NasladdinPlace.UI.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected string ControllerName => ControllerContext.RouteData.Values["controller"].ToString();
        protected virtual string ActionName => ControllerContext.RouteData.Values["action"].ToString();

        protected IUnitOfWork UnitOfWork;

        protected BaseController(IServiceProvider serviceProvider)
        {
            var unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            UnitOfWork = unitOfWorkFactory.MakeUnitOfWork();
        }

        /// <summary>
        /// Возвращает JSON на запросы.
        /// </summary>
        /// <param name="value">Обьект для сериализации в JSON.</param>
        /// <returns></returns>
        protected ContentResult JsonContent(object value)
        {
            var json = JsonConvert.SerializeObject(value);
            return Content(json, "application/json; charset=utf-8");
        }
    }
}
