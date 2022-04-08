using System;

namespace NasladdinPlace.Core.Models.Goods
{
    public class ProteinsFatsCarbohydratesCalories : Entity
    {
        private double _proteinsInGrams;
        private double _fatsInGrams;
        private double _carbohydratesInGrams;
        private double _caloriesInKcal;

        public double ProteinsInGrams
        {
            get => _proteinsInGrams;
            set
            {
                EnsurePropertyGreaterThanZero(nameof(ProteinsInGrams), value);
                
                _proteinsInGrams = value;
            }
        }

        public double FatsInGrams
        {
            get => _fatsInGrams;
            set
            {
                EnsurePropertyGreaterThanZero(nameof(FatsInGrams), value);
                
                _fatsInGrams = value;
            }
        }

        public double CarbohydratesInGrams
        {
            get => _carbohydratesInGrams;
            set
            {
                EnsurePropertyGreaterThanZero(nameof(CarbohydratesInGrams), value);

                _carbohydratesInGrams = value;
            }
        }

        public double CaloriesInKcal
        {
            get => _caloriesInKcal;
            set
            {
                EnsurePropertyGreaterThanZero(nameof(CaloriesInKcal), value);
                
                _caloriesInKcal = value;
            }
        }

        protected ProteinsFatsCarbohydratesCalories()
        {
            // required for EF
        }

        public ProteinsFatsCarbohydratesCalories(
            double proteinsInGrams,
            double fatsInGrams,
            double carbohydratesInGrams,
            double caloriesInKcal)
        {
            ProteinsInGrams = proteinsInGrams;
            FatsInGrams = fatsInGrams;
            CarbohydratesInGrams = carbohydratesInGrams;
            CaloriesInKcal = caloriesInKcal;
        }

        public void Update(ProteinsFatsCarbohydratesCalories proteinsFatsCarbohydratesCalories)
        {
            if (proteinsFatsCarbohydratesCalories == null)
                throw new ArgumentNullException(nameof(proteinsFatsCarbohydratesCalories));

            ProteinsInGrams = proteinsFatsCarbohydratesCalories.ProteinsInGrams;
            FatsInGrams = proteinsFatsCarbohydratesCalories.FatsInGrams;
            CarbohydratesInGrams = proteinsFatsCarbohydratesCalories.CarbohydratesInGrams;
            CaloriesInKcal = proteinsFatsCarbohydratesCalories.CaloriesInKcal;
        }

        private void EnsurePropertyGreaterThanZero(string propertyName, double value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    $"{propertyName} must be equal or greater than zero."
                );
        }
    }
}