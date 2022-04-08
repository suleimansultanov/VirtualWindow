using System;
using System.ComponentModel.DataAnnotations;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;

namespace NasladdinPlace.UI.ViewModels.Goods
{
    public class GoodViewModel : BaseViewModel
    {
        [Display(Name = "Код товара")]
        [Render(Control = RenderControl.Integer, FilterState = FilterState.Enable)]
        public int Id { get; set; }

        [Display(Name = "Название товара")]
        [Render(Control = RenderControl.Input, FilterState = FilterState.Enable)]
        public string Name { get; set; }

        [Display(Name = "Статус публикации")]
        [Render(Control = RenderControl.Combo, ComboSource = typeof(GoodPublishingStatus), FilterName = nameof(PublishingStatus))]
        public int PublishingStatus { get; set; }

        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "GoodCategory", TextReferenceSource = TextReferenceSources.GoodCategories)]
        [Display(Name = "Категория")]
        public int GoodCategoryId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string GoodCategory { get; set; }

        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "Maker", TextReferenceSource = TextReferenceSources.Makers)]
        [Display(Name = "Производитель")]
        public int MakerId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string Maker { get; set; }

        [Display(Name = "Действие")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/Shared/Actions/_editRemoveActionControl.cshtml")]
        public string Action { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(Entity);
    }
}