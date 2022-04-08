using Microsoft.AspNetCore.Mvc.Rendering;
using NasladdinPlace.Utilities.ValidationAttributes.Basic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Localization = NasladdinPlace.UI.Resources.ViewModels.Goods;

namespace NasladdinPlace.UI.ViewModels.Goods
{
    public class GoodsFormViewModel
    {
        public SelectList MakerSelectList { get; set; }
        public SelectList GoodCategorySelectList { get; set; }
        public SelectList PublishingStatusSelectList { get; set; }

        public int Id { get; set; }

        [LocalizedRequired]
        [Display(Name = "Maker", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public int? MakerId { get; set; }

        [LocalizedRequired]
        [Display(Name = "Category", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public int? GoodCategoryId { get; set; }

        [LocalizedRequired]
        [LocalizedStringLength(255)]
        [Display(Name = "Name", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public string Name { get; set; }

        [LocalizedRequired]
        [LocalizedStringLength(2000)]
        [Display(Name = "Description", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public string Description { get; set; }

        [LocalizedRange(0, int.MaxValue)]
        [Display(Name = "Volume", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public double? Volume { get; set; }

        [LocalizedRange(0, int.MaxValue)]
        [Display(Name = "NetWeight", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public double? NetWeight { get; set; }

        [LocalizedRequired]
        [Display(Name = "ProteinsInGrams", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public double? ProteinsInGrams { get; set; }

        [LocalizedRequired]
        [Display(Name = "FatsInGrams", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public double? FatsInGrams { get; set; }

        [LocalizedRequired]
        [Display(Name = "CarbohydratesInGrams", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public double? CarbohydratesInGrams { get; set; }

        [LocalizedRequired]
        [Display(Name = "CaloriesInKcal", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public double? CaloriesInKcal { get; set; }

        [Display(Name = "Composition", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public string Composition { get; set; }

        [Display(Name = "Category", ResourceType = typeof(Localization.GoodsFormViewModel))]
        public string ImagePath { get; set; }

        public IFormFile Image { get; set; }

        public string DefaultImagePath { get; set; }

        public int? PublishingStatus { get; set; }

        public string Header => Id == 0
            ? "Add Good"
            : "Edit Good";
    }
}
