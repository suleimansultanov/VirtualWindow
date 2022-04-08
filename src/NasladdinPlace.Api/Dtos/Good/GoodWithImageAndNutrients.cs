using NasladdinPlace.Api.Dtos.ProteinsFatsCarbohydratesCalories;

namespace NasladdinPlace.Api.Dtos.Good
{
    public class GoodWithImageAndNutrients
    {
        public int Id { get; set; }
        public string Maker { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public string Composition { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public int PublishingStatus { get; set; }
        public string Currency { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public ProteinsFatsCarbohydratesCaloriesDto Nutrients { get; set; }
    }
}
