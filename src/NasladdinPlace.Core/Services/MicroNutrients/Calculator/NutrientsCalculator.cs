using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.MicroNutrients.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.MicroNutrients.Calculator
{
    public class NutrientsCalculator : INutrientsCalculator
    {
        #region Constants

        private const double PercentageOfCaloriesInTheAbsenceOfActivityAndDecreasingWeight = 0.21;
        private const double PercentageOfCaloriesOnDecreasingWeight = 0.24;

        private const double ZeroActivityCoefficient = 1.3;
        private const double LowActivityCoefficient = 1.4;
        private const double ModerateActivityCoefficient = 1.6;
        private const double MediumActivityCoefficient = 1.9;
        private const double HighActivityCoefficient = 2.2;
        private const double VeryHighActivityCoefficient = 2.5;

        private const int AdditionalCaloriesOnSecondHalfOfPregnant = 350;
        private const int AdditionalCaloriesOnSecondHalfOfBreastFeeding1To6 = 500;
        private const int AdditionalCaloriesOnSecondHalfOfBreastFeeding7To12 = 450;
        private const int AdditionalProteinsOnSecondHalfOfPregnantAndBreastFeedingSecondHalf = 30;
        private const int AdditionalProteinsOnBreastFeedingFirstHalf = 40;
        private const int AdditionalFatsOnBreastFeeding = 15;
        private const int AdditionalFatsOnSecondHalfOfPregnant = 12;
        private const int AdditionalCarbohydratesOnSecondHalfOfPregnantAndBreastFeedingSecondHalf = 30;
        private const int AdditionalCarbohydratesOnBreastFeedingFirstHalf = 40;

        private const int FatsDevider = 9;
        private const int ProteinsAndCarbohydratesDevider = 4;

        private const double MultiplierOfProteinsOnDecreasingWeight = 0.24;
        private const double MultiplierOfFatsOnDecreasingWeight = 0.395;
        private const double MultiplierOfCarbohydratesOnDecreasingWeight = 0.365;

        private const double MultiplierOfProteinsOnEncreasingWeight = 0.2;
        private const double MultiplierOfFatsOnEncreasingWeight = 0.3;
        private const double MultiplierOfCarbohydratesOnEncreasingWeight = 0.5;

        private const double MultiplierOfProteinsOnKeepingWeight = 0.18;
        private const double MultiplierOfFatsOnKeepingWeight = 0.3;
        private const double MultiplierOfCarbohydratesOnKeepingWeight = 0.52;

        #endregion

        private int _additionalCalories;
        private int _additionalProteins;
        private int _additionalFats;
        private int _additionalCarbohydrates;

        public ValueResult<Nutrients> GetNormalNutrientsByUser(ApplicationUser user)
        {
            var nutrients = new Nutrients();

            if (!user.CanCalculateNutrientsByUserParams)
                return ValueResult<Nutrients>.Failure("It is impossible to calculate, " +
                                                      "because one of the parameters from: Activity, Gender, Weight, Height, Pregnancy, Age or Goal is null. Please check your entries again.");

            var basicMetabolismInCcal = BasicMetabolismCalculator.CalculateByMifflinSanGeorFormula(user);

            SetAdditionalParameters(user.Pregnancy);

            var generalCalories = GetCaloriesWithActivityRate(user, basicMetabolismInCcal);

            nutrients.SetCalories(generalCalories);

            SetNutrients(user, nutrients);

            generalCalories = generalCalories + _additionalCalories;

            if (user.IsGoalDecreasingWeightWithoutActivity)
            {
                var percentageOfCaloriesInTheAbsenceOfActivity = Math.Round(
                        generalCalories *
                        PercentageOfCaloriesInTheAbsenceOfActivityAndDecreasingWeight,
                        MidpointRounding.AwayFromZero);
                nutrients.SetCalories(generalCalories - percentageOfCaloriesInTheAbsenceOfActivity);
            }
            else if (user.IsGoalDecreasingWeightWithActivity)
            {
                var percentageOfCalories = Math.Round(generalCalories * PercentageOfCaloriesOnDecreasingWeight,
                    MidpointRounding.AwayFromZero);
                nutrients.SetCalories(generalCalories - percentageOfCalories);
            }
            else
                nutrients.SetCalories(generalCalories);

            return ValueResult<Nutrients>.Success(nutrients);
        }

        private double GetActivityRate(ActivityType activity)
        {
            switch (activity)
            {
                case ActivityType.Zero: return ZeroActivityCoefficient;
                case ActivityType.Low: return LowActivityCoefficient;
                case ActivityType.Moderate: return ModerateActivityCoefficient;
                case ActivityType.Medium: return MediumActivityCoefficient;
                case ActivityType.High: return HighActivityCoefficient;
                case ActivityType.VeryHigh: return VeryHighActivityCoefficient;
                default:
                    throw new ArgumentOutOfRangeException(nameof(activity), activity,
                        $"Activity value cannot be less than {(int) ActivityType.Zero} and greater than {(int) ActivityType.VeryHigh}.");
            }
        }

        private double GetCaloriesWithActivityRate(ApplicationUser user, double ccalWithoutActivity)
        {
            var activity = GetActivityRate(user.Activity.Value);
            var roundedCalories = Math.Round(ccalWithoutActivity * activity, MidpointRounding.AwayFromZero);
            return roundedCalories;
        }

        private void SetAdditionalParameters(PregnancyType? pregnancy)
        {
            switch (pregnancy)
            {
                case PregnancyType.SecondHalf:
                    _additionalProteins = AdditionalProteinsOnSecondHalfOfPregnantAndBreastFeedingSecondHalf;
                    _additionalFats = AdditionalFatsOnSecondHalfOfPregnant;
                    _additionalCarbohydrates = AdditionalCarbohydratesOnSecondHalfOfPregnantAndBreastFeedingSecondHalf;
                    _additionalCalories = AdditionalCaloriesOnSecondHalfOfPregnant;
                    break;
                case PregnancyType.BreastFeeding1To6:
                    _additionalProteins = AdditionalProteinsOnBreastFeedingFirstHalf;
                    _additionalFats = AdditionalFatsOnBreastFeeding;
                    _additionalCarbohydrates = AdditionalCarbohydratesOnBreastFeedingFirstHalf;
                    _additionalCalories = AdditionalCaloriesOnSecondHalfOfBreastFeeding1To6;
                    break;
                case PregnancyType.BreastFeeding7To12:
                    _additionalProteins = AdditionalProteinsOnSecondHalfOfPregnantAndBreastFeedingSecondHalf;
                    _additionalFats = AdditionalFatsOnBreastFeeding;
                    _additionalCarbohydrates = AdditionalCarbohydratesOnSecondHalfOfPregnantAndBreastFeedingSecondHalf;
                    _additionalCalories = AdditionalCaloriesOnSecondHalfOfBreastFeeding7To12;
                    break;
                default:
                    _additionalProteins = 0;
                    _additionalFats = 0;
                    _additionalCarbohydrates = 0;
                    _additionalCalories = 0;
                    break;
            }
        }

        private void SetNutrients(ApplicationUser user, Nutrients nutrients)
        {
            switch (user.Goal)
            {
                case GoalType.IncreaseWeight:
                    NutrientsForEncreasingWeight(nutrients);
                    return;
                case GoalType.KeepWeight:
                    NutrientsForKeepingWeight(nutrients);
                    return;
                case GoalType.DecreaseWeight:
                    NutrientsForDecreasingWeight(nutrients, user.Activity.Value);
                    return;
                default:
                    throw new ArgumentException(nameof(user.Goal), $"Goal value cannot be less than {(int)GoalType.IncreaseWeight} and greater than {(int)GoalType.KeepWeight}.");
            }
        }

        private void NutrientsForEncreasingWeight(Nutrients nutrients)
        {
            var correctedCalloriesForProteins = Math.Round(MultiplierOfProteinsOnEncreasingWeight * nutrients.Calories,
                MidpointRounding.AwayFromZero);
            var proteins = Math.Round(correctedCalloriesForProteins / ProteinsAndCarbohydratesDevider,
                MidpointRounding.AwayFromZero);

            var correctedCalloriesForFats = Math.Round(MultiplierOfFatsOnEncreasingWeight * nutrients.Calories,
                MidpointRounding.AwayFromZero);
            var fats = Math.Round(correctedCalloriesForFats / FatsDevider,
                MidpointRounding.AwayFromZero);

            var correctedCalloriesForCarbohydrates = Math.Round(MultiplierOfCarbohydratesOnEncreasingWeight * nutrients.Calories,
                MidpointRounding.AwayFromZero);
            var carbohydrates = Math.Round(correctedCalloriesForCarbohydrates / ProteinsAndCarbohydratesDevider,
                MidpointRounding.AwayFromZero);

            nutrients.SetProteins(proteins);
            nutrients.SetFats(fats);
            nutrients.SetCarbohydrates(carbohydrates);
        }

        private void NutrientsForKeepingWeight(Nutrients nutrients)
        {
            var proteins = Math.Round(
                MultiplierOfProteinsOnKeepingWeight * nutrients.Calories / ProteinsAndCarbohydratesDevider,
                MidpointRounding.AwayFromZero);

            var fats = Math.Round(MultiplierOfFatsOnKeepingWeight * nutrients.Calories / FatsDevider,
                MidpointRounding.AwayFromZero);

            var carbohydrates = Math.Round(
                MultiplierOfCarbohydratesOnKeepingWeight * nutrients.Calories / ProteinsAndCarbohydratesDevider,
                MidpointRounding.AwayFromZero);

            nutrients.SetProteins(proteins, _additionalProteins);
            nutrients.SetFats(fats, _additionalFats);
            nutrients.SetCarbohydrates(carbohydrates, _additionalCarbohydrates);
        }

        private void NutrientsForDecreasingWeight(Nutrients nutrients, ActivityType activity)
        {
            var proteins = activity == ActivityType.Zero
                ? Math.Round(
                    Math.Round(
                        MultiplierOfProteinsOnDecreasingWeight * (nutrients.Calories -
                                Math.Round(nutrients.Calories * PercentageOfCaloriesInTheAbsenceOfActivityAndDecreasingWeight, MidpointRounding.AwayFromZero)),
                        MidpointRounding.AwayFromZero) / ProteinsAndCarbohydratesDevider)
                : Math.Round(
                    Math.Round(
                        MultiplierOfProteinsOnDecreasingWeight * (nutrients.Calories -
                                Math.Round(nutrients.Calories * PercentageOfCaloriesOnDecreasingWeight, MidpointRounding.AwayFromZero)),
                        MidpointRounding.AwayFromZero) / ProteinsAndCarbohydratesDevider);
           
            var fats = activity == ActivityType.Zero
                ? Math.Round(
                    Math.Round(
                        MultiplierOfFatsOnDecreasingWeight * (nutrients.Calories -
                                 Math.Round(nutrients.Calories * PercentageOfCaloriesInTheAbsenceOfActivityAndDecreasingWeight, MidpointRounding.AwayFromZero)),
                        MidpointRounding.AwayFromZero) / FatsDevider)
                : Math.Round(
                    Math.Round(
                        MultiplierOfFatsOnDecreasingWeight * (nutrients.Calories -
                                 Math.Round(nutrients.Calories * PercentageOfCaloriesOnDecreasingWeight, MidpointRounding.AwayFromZero)),
                        MidpointRounding.AwayFromZero) / FatsDevider);
            
            var carbohydrates = activity == ActivityType.Zero
                ? Math.Round(
                    Math.Round(
                        MultiplierOfCarbohydratesOnDecreasingWeight * (nutrients.Calories -
                                 Math.Round(nutrients.Calories * PercentageOfCaloriesInTheAbsenceOfActivityAndDecreasingWeight, MidpointRounding.AwayFromZero)),
                        MidpointRounding.AwayFromZero) / ProteinsAndCarbohydratesDevider)
                : Math.Round(
                    Math.Round(
                        MultiplierOfCarbohydratesOnDecreasingWeight * (nutrients.Calories -
                                 Math.Round(nutrients.Calories * PercentageOfCaloriesOnDecreasingWeight, MidpointRounding.AwayFromZero)),
                        MidpointRounding.AwayFromZero) / ProteinsAndCarbohydratesDevider);
           
            nutrients.SetProteins(proteins);
            nutrients.SetFats(fats);
            nutrients.SetCarbohydrates(carbohydrates);
        }
    }
}
