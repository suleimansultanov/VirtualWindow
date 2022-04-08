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
    public class MediaContentsController : ReferenceBaseController
    {
        public MediaContentsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetMedia()
        {
            return Reference<MediaContentViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Медиа контент",
                    BeforeGridPartialName = "~/Views/MediaContents/_beforeGridPartial.cshtml",
                    Actions = GetActions(),
                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        filter.SetSort(nameof(MediaContentViewModel.UploadDateTime), SortTypes.Desc);
                    }
                });
        }

        private List<Command> GetActions()
        {
            return new List<Command>
            {
                new Command
                {
                    Type = CommandType.Custom,
                    Name = "<em class='fa fa-plus'></em> Добавить",
                    Binding = "click: $root.add"
                },
                new Command
                {
                    Type = CommandType.OpenUrl,
                    Name = "Медиа для витрин",
                    Url = Url.Action("GetMediaToPlatforms", "MediaContentToPlatforms"),
                    ClassStyle = "btn-success"
                }
            };
        }
    }
}