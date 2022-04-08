using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Media;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers
{
    [Obsolete("Will be removed in the future releases. Its replace PosScreenTemplate.")]
    [Permission(nameof(ReadOnlyAccess))]
    public class PosMediaContentsController : ReferenceBaseController
    {
        public PosMediaContentsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IActionResult> GetPosMedia(int posId)
        {
            var pos = await UnitOfWork.PointsOfSale.GetByIdAsync(posId);
            ViewBag.TextReference = true;

            return Reference<PosMediaContentViewModel>(
                context: new List<FilterItemModel>
                {
                    new FilterItemModel
                    {
                        Value = posId.ToString(),
                        FilterType = FilterTypes.Equals,
                        ForceCastType = CastTypes.Int32,
                        PropertyName = nameof(PosMediaContentViewModel.PosId)
                    }
                },
                configuration: new ConfigReference
                {
                    Title = $"Медиа контент витрины: {pos.Name}",
                    BeforeGridPartialName = "~/Views/PosMediaContents/_beforeGridPartial.cshtml",
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<em class='fa fa-plus'></em> Добавить",
                            Binding = "click: $root.add"
                        }
                    },
                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        filter.SetSort(nameof(PosMediaContentViewModel.DateTimeCreated), SortTypes.Desc);
                    },
                    BreadCrumbs = new List<string>
                    {
                        GetBreadCrumb(Url.Action("All", "PointsOfSale"), "Домой"),
                        GetBreadCrumb(Url.Action("EditPos", "PointsOfSale", new {id = posId}), "Карточка витрины")
                    },
                    IsRenderModalFilter = false
                });

        }
    }
}