using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.ViewModels.Promotions;
using System;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class PromotionSettingsController : ReferenceBaseController
    {
        public PromotionSettingsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetPromotionSettings()
        {
            return Reference<PromotionSettingViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Промо акции",
                    BeforeGridPartialName = "~/Views/PromotionSettings/_beforeGridPartial.cshtml"
                });
        }
    }
}