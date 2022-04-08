using Microsoft.AspNetCore.Http;

namespace NasladdinPlace.UI.ViewModels.Goods
{
    public class BuyGoodViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double? Volume { get; set; }
        public double? NetWeight { get; set; }
        public double? ProteinsInGrams { get; set; }
        public double? FatsInGrams { get; set; }
        public double? CarbohydratesInGrams { get; set; }
        public double? CaloriesInKcal { get; set; }
        public string Composition { get; set; }
        public string ImagePath { get; set; }
        public IFormFile Image { get; set; }
        public string Maker { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
    }
}
