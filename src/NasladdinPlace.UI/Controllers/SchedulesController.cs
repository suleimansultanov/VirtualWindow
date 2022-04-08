using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.ViewModels.Schedules;
using System;

namespace NasladdinPlace.UI.Controllers
{

    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class SchedulesController : ReferenceBaseController
    {
        public SchedulesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetSchedules()
        {
            return Reference<ScheduleViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Статус планировщика задач в API"
                });
        }
    }
}