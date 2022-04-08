using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.UserNotifications;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class UserNotificationsController : ReferenceBaseController
    {
        public UserNotificationsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetNotifications(int? userId)
        {
            var context = new List<FilterItemModel>();

            if (userId.HasValue)
            {
                context.Add(
                    new FilterItemModel
                    {
                        Value = userId.ToString(),
                        FilterType = FilterTypes.Equals,
                        ForceCastType = CastTypes.Int32,
                        PropertyName = nameof(UserNotificationViewModel.UserId)
                    });
            }

            return Reference<UserNotificationViewModel>(
                context: context,
                configuration: new ConfigReference
                {
                    Title = "Коммуникации",
                    FilterType = RenderFilter.WithAdditionFilters,
                    CreateAdditionFilter = (filter, topFilter) =>
                    {
                        ConfigReference.CreateFilterFromToDateTime(filter, topFilter,
                            nameof(UserNotificationViewModel.DateTimeSent), null, null, "Дата сообщения c",
                            "Дата сообщения по");
                    },
                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        filter.SetSort(nameof(UserNotificationViewModel.DateTimeSent), SortTypes.Desc);
                    }
                });
        }
    }
}