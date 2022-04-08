using NasladdinPlace.Api.Services.MicroNutrients.Models;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.MicroNutrients.Models;

namespace NasladdinPlace.Api.Tests.Scenarios.MicroNutrients.DataGenerators
{
    public class NutrientsDataGeneratorBase
    {
        protected UserParams CreateUserGoalsWithParams(
            int age,
            Gender gender,
            int weight,
            int height,
            ActivityType activity,
            GoalType goal,
            PregnancyType pregnancy)
        {
            return new UserParams
            {
                Gender = gender.ToString(),
                Age = age,
                Activity = activity.ToString(),
                Goal = goal.ToString(),
                Height = height,
                Weight = weight,
                Pregnancy = pregnancy.ToString()
            };
        }

        protected Nutrients CreateExpectedNutrients(double expectedCalories,
            double expectedProteins,
            double expectedFats,
            double expectedCarbohydrates)
        {
            return new Nutrients(expectedCalories, expectedProteins, expectedFats, expectedCarbohydrates);
        }
    }
}
