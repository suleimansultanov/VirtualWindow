using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.UI.Managers.Reference.Enums;
using NasladdinPlace.UI.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.UI.ViewModels.LabeledGoods
{
    public class LabeledGoodsGroupByGoodViewModel : BaseViewModel
    {
        [Display(Name = "Товар")]
        [Render(Control = RenderControl.TextReferenceValueText, TextReference = "GoodName", TextReferenceSource = TextReferenceSources.Goods, SortState = SortState.Disable)]
        public int GoodId { get; set; }

        [Render(DisplayType = DisplayType.Hide)]
        public string GoodName { get; set; }

        [Display(Name = "Цена")]
        [Render(Control = RenderControl.Decimal, FilterState = FilterState.Disable)]
        public decimal Price { get; set; }

        [Display(Name = "Количество")]
        [Render(Control = RenderControl.Integer, FilterState = FilterState.Disable, SortState = SortState.Disable)]
        public int LabeledGoodsCount { get; set; }

        [Display(Name = "Метки")]
        [Render(Control = RenderControl.Custom, FilterState = FilterState.Disable, PartialName = "~/Views/LabeledGoods/_labeledGoodsBreadcrumbs.cshtml")]
        public IEnumerable<LabeledGoodBasicInfoViewModel> LabeledGoods { get; set; }

        [Render(Ignore = true)]
        public override Type EntityType() => typeof(Entity);
    }
}