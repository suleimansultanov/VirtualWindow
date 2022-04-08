using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.GoodCategories
{
    public class GoodCategoryViewModel : BaseViewModel
    {
        [Render(DisplayType = DisplayType.Hide)]
        public int Id { get; set; }

        [Display(Name = "Наименование")]
        [Render(Control = RenderControl.Input, FilterState = FilterState.Ignore, SortState = SortState.Disable)]
        public string Name { get; set; }

        [Display(Name = "Изображение по умолчанию")]
        [Render(DisplayType = DisplayType.Hide)]
        public string ImagePath { get; set; } 

        [Display(Name = "Загрузка изображения")]
        [Render(DisplayType = DisplayType.Hide)]
        public IFormFile Image { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/Shared/Actions/_editActionControl.cshtml")]
        public string Action { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(GoodCategory);
    }
}