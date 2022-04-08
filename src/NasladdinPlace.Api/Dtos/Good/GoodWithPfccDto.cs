using NasladdinPlace.Api.Dtos.ProteinsFatsCarbohydratesCalories;

namespace NasladdinPlace.Api.Dtos.Good
{
    public class GoodWithPfccDto
    {
        public string Name { get; set; }
        public ProteinsFatsCarbohydratesCaloriesDto Nutrients { get; set; }
    }
}
