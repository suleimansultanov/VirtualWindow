using NasladdinPlace.Core.Enums;
using System.Collections;
using System.Collections.Generic;

namespace NasladdinPlace.Api.Tests.Scenarios.MicroNutrients.DataGenerators
{
    public class NutrientsDataGenerator : NutrientsDataGeneratorBase, IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 64,
                    age: 25,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2251,
                    expectedProteins: 101,
                    expectedFats: 75,
                    expectedCarbohydrates: 293)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 64,
                    age: 25,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2019,
                    expectedProteins: 91,
                    expectedFats: 67,
                    expectedCarbohydrates: 262)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 64,
                    age: 25,
                    activity: ActivityType.VeryHigh,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.IncreaseWeight),
                CreateExpectedNutrients(expectedCalories: 4824,
                    expectedProteins: 241,
                    expectedFats: 161,
                    expectedCarbohydrates: 603)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 64,
                    age: 25,
                    activity: ActivityType.High,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.IncreaseWeight),
                CreateExpectedNutrients(expectedCalories: 3806,
                    expectedProteins: 190,
                    expectedFats: 127,
                    expectedCarbohydrates: 476)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 169,
                    weight: 49,
                    age: 27,
                    activity: ActivityType.High,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.IncreaseWeight),
                CreateExpectedNutrients(expectedCalories: 3300,
                    expectedProteins: 165,
                    expectedFats: 110,
                    expectedCarbohydrates: 413)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 100,
                    age: 25,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.DecreaseWeight),
                CreateExpectedNutrients(expectedCalories: 1831,
                    expectedProteins: 110,
                    expectedFats: 80,
                    expectedCarbohydrates: 167)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 100,
                    age: 25,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.DecreaseWeight),
                CreateExpectedNutrients(expectedCalories: 2008,
                    expectedProteins: 120,
                    expectedFats: 88,
                    expectedCarbohydrates: 183)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 64,
                    age: 25,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.FirstHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2019,
                    expectedProteins: 91,
                    expectedFats: 67,
                    expectedCarbohydrates: 262)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 64,
                    age: 25,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.SecondHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2369,
                    expectedProteins: 121,
                    expectedFats: 79,
                    expectedCarbohydrates: 292)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 64,
                    age: 35,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.SecondHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2299,
                    expectedProteins: 118,
                    expectedFats: 77,
                    expectedCarbohydrates: 283)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 64,
                    age: 45,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.SecondHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2229,
                    expectedProteins: 115,
                    expectedFats: 75,
                    expectedCarbohydrates: 274),
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 64,
                    age: 55,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.SecondHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2159,
                    expectedProteins: 111,
                    expectedFats: 72,
                    expectedCarbohydrates: 265)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.High,
                    pregnancy: PregnancyType.SecondHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 3544,
                    expectedProteins: 174,
                    expectedFats: 118,
                    expectedCarbohydrates: 445)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 64,
                    age: 35,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.BreastFeeding7To12,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2399,
                    expectedProteins: 118,
                    expectedFats: 80,
                    expectedCarbohydrates: 283)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.BreastFeeding7To12,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2483,
                    expectedProteins: 121,
                    expectedFats: 83,
                    expectedCarbohydrates: 294)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Moderate,
                    pregnancy: PregnancyType.BreastFeeding7To12,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2773,
                    expectedProteins: 135,
                    expectedFats: 92,
                    expectedCarbohydrates: 332)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Medium,
                    pregnancy: PregnancyType.BreastFeeding7To12,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 3209,
                    expectedProteins: 154,
                    expectedFats: 107,
                    expectedCarbohydrates: 389)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.High,
                    pregnancy: PregnancyType.BreastFeeding7To12,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 3644,
                    expectedProteins: 174,
                    expectedFats: 121,
                    expectedCarbohydrates: 445)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Zero,
                    pregnancy: PregnancyType.BreastFeeding7To12,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2338,
                    expectedProteins: 115,
                    expectedFats: 78,
                    expectedCarbohydrates: 275)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Moderate,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2589,
                    expectedProteins: 117,
                    expectedFats: 86,
                    expectedCarbohydrates: 337)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Medium,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 3074,
                    expectedProteins: 138,
                    expectedFats: 102,
                    expectedCarbohydrates: 400)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.High,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 3560,
                    expectedProteins: 160,
                    expectedFats: 119,
                    expectedCarbohydrates: 463)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.VeryHigh,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 4045,
                    expectedProteins: 182,
                    expectedFats: 135,
                    expectedCarbohydrates: 526)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.IncreaseWeight),
                CreateExpectedNutrients(expectedCalories: 2718,
                    expectedProteins: 136,
                    expectedFats: 91,
                    expectedCarbohydrates: 340)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.SecondHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2383,
                    expectedProteins: 121,
                    expectedFats: 80,
                    expectedCarbohydrates: 294)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Moderate,
                    pregnancy: PregnancyType.SecondHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2673,
                    expectedProteins: 135,
                    expectedFats: 89,
                    expectedCarbohydrates: 332)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Medium,
                    pregnancy: PregnancyType.SecondHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 3109,
                    expectedProteins: 154,
                    expectedFats: 104,
                    expectedCarbohydrates: 389)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 64,
                    age: 35,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.BreastFeeding1To6,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2449,
                    expectedProteins: 128,
                    expectedFats: 80,
                    expectedCarbohydrates: 293)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Moderate,
                    pregnancy: PregnancyType.BreastFeeding1To6,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2823,
                    expectedProteins: 145,
                    expectedFats: 92,
                    expectedCarbohydrates: 342)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.Medium,
                    pregnancy: PregnancyType.BreastFeeding1To6,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 3259,
                    expectedProteins: 164,
                    expectedFats: 107,
                    expectedCarbohydrates: 399)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 35,
                    activity: ActivityType.High,
                    pregnancy: PregnancyType.BreastFeeding1To6,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 3694,
                    expectedProteins: 184,
                    expectedFats: 121,
                    expectedCarbohydrates: 455)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Female,
                    height: 174,
                    weight: 70,
                    age: 40,
                    activity: ActivityType.Moderate,
                    pregnancy: PregnancyType.SecondHalf,
                    goal: GoalType.KeepWeight),
                CreateExpectedNutrients(expectedCalories: 2633,
                    expectedProteins: 133,
                    expectedFats: 88,
                    expectedCarbohydrates: 327)
            };
            yield return new object[]
            {
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 100,
                    age: 25,
                    activity: ActivityType.High,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.IncreaseWeight),
                CreateExpectedNutrients(expectedCalories: 5196,
                    expectedProteins: 260,
                    expectedFats: 173,
                    expectedCarbohydrates: 650)
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
