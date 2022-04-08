using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Users;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class UsersController : ReferenceBaseController
    {
        public UsersController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("/[controller]")]
        public IActionResult GetUsers(UsersFiltersContext filterContext)
        {
            var notLazyDateTimeFrom = filterContext.NotLazyUsersFrom?.ToDynamicFilterDateFormat();
            var notLazyDateTimeUntil = filterContext.NotLazyUsersUntil?.ToDynamicFilterDateFormat();
            var lazyDateTimeFrom = filterContext.LazyUsersFrom?.ToDynamicFilterDateFormat();
            var lazyDateTimeUntil = filterContext.LazyUsersUntil?.ToDynamicFilterDateFormat();

            var context = new List<FilterItemModel>();

            if (filterContext.Type.HasValue && filterContext.Type == UserLazinessIndex.Lazy)
            {
                context.Add(new FilterItemModel
                {
                    Value = null,
                    FilterType = FilterTypes.Equals,
                    ForceCastType = CastTypes.Int32,
                    PropertyName = nameof(UserLazinessIndex.Lazy),
                    FilterName = nameof(UserLazinessIndex.Lazy)
                });
            }

            if (filterContext.UserId.HasValue)
            {
                context.Add(new FilterItemModel
                {
                    Value = filterContext.UserId.Value.ToString(),
                    FilterType = FilterTypes.Equals,
                    ForceCastType = CastTypes.Int32,
                    PropertyName = nameof(UserViewModel.Id),
                    FilterName = nameof(UserViewModel.Id)
                });
            }

            return Reference<UserViewModel>(
                context: context,
                configuration: new ConfigReference
                {
                    Title = "Пользователи",
                    FilterType = RenderFilter.WithAdditionFilters,
                    CreateAdditionFilter = (filter, topFilter) =>
                    {
                        ConfigReference.CreatePropertyFilterFromToDateTime(filter, topFilter,
                            nameof(UserViewModel.RegistrationInitiationDate), notLazyDateTimeFrom, notLazyDateTimeUntil, "Дата начала регистрации c",
                            "Дата начала регистрации по");
                        ConfigReference.CreatePropertyFilterFromToDateTime(filter, topFilter,
                            nameof(UserViewModel.RegistrationCompletionDate), null, null, "Завершение регистрации c",
                            "Завершение регистрации по");
                        ConfigReference.CreatePropertyFilterFromToDateTime(filter, topFilter,
                            nameof(UserViewModel.BankingCardVerificationInitiationDate), null, null,
                            "Начало подтверждения карты c", "Начало подтверждения карты по");
                        ConfigReference.CreatePropertyFilterFromToDateTime(filter, topFilter,
                            nameof(UserViewModel.BankingCardVerificationCompletionDate), lazyDateTimeFrom, lazyDateTimeUntil,
                            "Завершение подтверждения карты c", "Завершение подтверждения карты по");
                        ConfigReference.CreatePropertyFilterFromToDateTime(filter, topFilter,
                            nameof(UserViewModel.LastPaidDate), null, null, "Последняя покупка c",
                            "Последняя покупка по");
                    },
                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        if (filterContext.NotLazyUsersFrom.HasValue && filterContext.NotLazyUsersUntil.HasValue)
                            filter.SetSort(nameof(UserViewModel.RegistrationInitiationDate), SortTypes.Desc);
                    },
                    Actions = User.IsInRole(nameof(Roles.Admin))
                        ? new List<Command>
                        {
                             new Command
                             {
                                 Type = CommandType.Custom,
                                 Name = "<em class='fa fa-plus'></em> Добавить",
                                 Binding = "click: $root.add",
                                 ClassStyle = "btn btn-primary add-user"
                             }
                        }
                        : null,
                    BeforeGridPartialName = "~/Views/Users/_beforeUserGridPartial.cshtml"
                });
        }
    }
}