using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Managers.Reference;
using System;
using NasladdinPlace.UI.Managers.Reference.Models;

namespace NasladdinPlace.UI.Controllers
{
    public class CommonReferenceController : BaseController
    {
        private readonly TextReferenceManager _textReferenceManager;

        public CommonReferenceController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _textReferenceManager = new TextReferenceManager(UnitOfWork);
        }

        [HttpPost]
        public IActionResult GetTextReferenceSource([FromBody] TextReferenceParamsModel paramsModel)
        {
            var data = _textReferenceManager.GetData(paramsModel.Source, paramsModel.Filter);
            return JsonContent(new {Message = "Данные успешно получены!", data});
        }

        public IActionResult GetFilter(string filter, string contextData)
        {
            return PartialView("Controls/Filters/" + filter, contextData);
        }
    }
}