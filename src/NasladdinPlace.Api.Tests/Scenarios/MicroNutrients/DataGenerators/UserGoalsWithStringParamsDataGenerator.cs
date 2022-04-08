using NasladdinPlace.Core.Enums;
using System.Collections;
using System.Collections.Generic;

namespace NasladdinPlace.Api.Tests.Scenarios.MicroNutrients.DataGenerators
{
    public class UserGoalsWithStringParamsDataGenerator : NutrientsDataGeneratorBase, IEnumerable<object[]>
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
                    goal: GoalType.KeepWeight)
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
                    goal: GoalType.KeepWeight)
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
                    goal: GoalType.IncreaseWeight)
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
                    goal: GoalType.IncreaseWeight)
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
                    goal: GoalType.IncreaseWeight)
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
                    goal: GoalType.DecreaseWeight)
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
                    goal: GoalType.DecreaseWeight)
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
