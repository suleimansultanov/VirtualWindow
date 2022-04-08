using Microsoft.AspNetCore.Authorization;
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

namespace NasladdinPlace.UI.Controllers
{
    [Obsolete("Will be removed in the future releases. Its replace PosScreenTemplate.")]
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class MediaContentToPlatformsController : ReferenceBaseController
    {
        public MediaContentToPlatformsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetMediaToPlatforms()
        {
            ViewBag.TextReference = true;

            return Reference<MediaContentToPosPlatformViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Медиа контент для витрин",
                    BeforeGridPartialName = "~/Views/MediaContentToPlatforms/_beforeGridPartial.cshtml",
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
                        filter.SetSort(nameof(MediaContentToPosPlatformViewModel.DateTimeCreated), SortTypes.Desc);
                    },
                    BreadCrumbs = new List<string>
                    {
                        GetBreadCrumb(Url.Action("All", "PointsOfSale"), "Домой"),
                        GetBreadCrumb(Url.Action("GetMedia", "MediaContents"), "Медиа контент")
                    },
                    IsRenderModalFilter = false
                });
        }
    }
}