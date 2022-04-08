namespace NasladdinPlace.Core.Services.MicroNutrients.Models
{
    public class Nutrients
    {
        public double Calories { get; private set; }
        public double Proteins { get; private set; }
        public double Fats { get; private set; }
        public double Carbohydrates { get; private set; }

        public Nutrients()
        {

        }

        public Nutrients(double calories,
            double proteins,
            double fats,
            double carbohydrates)
        {
            Calories = calories;
            Proteins = proteins;
            Fats = fats;
            Carbohydrates = carbohydrates;
        }

        public void SetCalories(double calories)
        {
            Calories = calories;
        }

        public void SetProteins(double proteins, int additionalProteins = 0)
        {
            Proteins = proteins + additionalProteins;
        }

        public void SetFats(double fats, int additionalFats = 0)
        {
            Fats = fats + additionalFats;
        }

        public void SetCarbohydrates(double carbohydrates, int additionalCarbohydrates = 0)
        {
            Carbohydrates = carbohydrates + additionalCarbohydrates;
        }

        public void AdditionCalories(int additionalCalories)
        {
            Calories += additionalCalories;
        }
    }
}
