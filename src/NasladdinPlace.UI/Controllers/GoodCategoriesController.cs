using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Core.Models.Reference;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Controllers.Base.Models;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.Managers.Reference.Models;
using NasladdinPlace.UI.ViewModels.GoodCategories;
using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Controllers
{
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class GoodCategoriesController : ReferenceBaseController
    {
        public GoodCategoriesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult GetGoodCategories()
        {
            return Reference<GoodCategoryViewModel>(
                configuration: new ConfigReference
                {
                    Title = "Категории товаров",
                    FilterType = RenderFilter.WithAdditionFilters,
                    BeforeGridPartialName = "~/Views/GoodCategories/_beforeGridPartial.cshtml",
                    CreateAdditionFilter = (filter, topFilter) =>
                    {
                        ConfigReference.CreateFilter(topFilter, filter,
                            new RenderAttribute
                            {
                                Control = RenderControl.Input,
                                DisplayName = "Наименование",
                                PropertyName = nameof(GoodCategoryViewModel.Name),
                                FilterName = nameof(GoodCategoryViewModel.Name)
                            }, string.Empty, FilterTypes.Contains);
                    },
                    Actions = new List<Command>()
                    {
                        new Command
                        {
                            Type = CommandType.Custom,
                            Name = "<i class='fa fa-plus'></i> Добавить",
                            Binding = "click: $root.add"
                        }
                    }
                });
        }
    }
}