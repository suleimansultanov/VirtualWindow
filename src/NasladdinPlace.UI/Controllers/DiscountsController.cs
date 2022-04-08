using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Enums.Discounts;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.Discounts;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class DiscountsController : ReferenceBaseController
    {
        public DiscountsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetDiscounts()
        {
            return Reference<DiscountViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Динамические цены. Скидки",
                    BeforeGridPartialName = "~/Views/Discounts/_beforeGridPartial.cshtml",

                    SetDefaultValueFilter = (filter, infos) =>
                    {
                        filter.SetSort(nameof(DiscountViewModel.DateTimeCreated), SortTypes.Desc);
                    },
                    Actions = new List<Command>
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<em class='fa fa-plus'></em> Добавить",
                            Binding = "click: $root.add"
                        }
                    },
                    AdditionReferenceSource = new List<Type> { typeof(DiscountRuleArea), typeof(DiscountRuleType) }
                });
        }
    }
}