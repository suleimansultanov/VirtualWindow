using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.TestUtils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Tests.Scenarios.MicroNutrients.DataGenerators
{
    public class NutrientsHistoryDataGenerator : NutrientsDataGeneratorBase, IEnumerable<object[]>
    {
        private const byte CountOfDaysIsFive = 5;
        private const byte CountOfDaysIsTen = 10;

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItemAndSetDatePaid(posOperationId: 1, daysToSubstract: 0),
                    CreateCheckItemAndSetDatePaid(posOperationId: 2, daysToSubstract: 1),
                    CreateCheckItemAndSetDatePaid(posOperationId: 3, daysToSubstract: 2),
                    CreateCheckItemAndSetDatePaid(posOperationId: 4, daysToSubstract: 3),
                    CreateCheckItemAndSetDatePaid(posOperationId: 5, daysToSubstract: 4),
                },
                CountOfDaysIsFive,
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
                new Collection<CheckItem>
                {
                    CreateCheckItemAndSetDatePaid(posOperationId: 1, daysToSubstract: 0),
                    CreateCheckItemAndSetDatePaid(posOperationId: 2, daysToSubstract: 1),
                    CreateCheckItemAndSetDatePaid(posOperationId: 3, daysToSubstract: 2),
                    CreateCheckItemAndSetDatePaid(posOperationId: 4, daysToSubstract: 3),
                    CreateCheckItemAndSetDatePaid(posOperationId: 5, daysToSubstract: 4),
                    CreateCheckItemAndSetDatePaid(posOperationId: 6, daysToSubstract: 5),
                    CreateCheckItemAndSetDatePaid(posOperationId: 7, daysToSubstract: 6),
                    CreateCheckItemAndSetDatePaid(posOperationId: 8, daysToSubstract: 7),
                    CreateCheckItemAndSetDatePaid(posOperationId: 9, daysToSubstract: 8),
                    CreateCheckItemAndSetDatePaid(posOperationId: 10, daysToSubstract: 9),
                },
                CountOfDaysIsTen,
                CreateUserGoalsWithParams(
                    gender: Gender.Male,
                    height: 174,
                    weight: 64,
                    age: 25,
                    activity: ActivityType.Low,
                    pregnancy: PregnancyType.No,
                    goal: GoalType.KeepWeight)
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static CheckItem CreateCheckItemAndSetDatePaid(int posOperationId, byte daysToSubstract)
        {
            var checkItem = CheckItem.NewBuilder(
                    posId: 1,
                    posOperationId: posOperationId,
                    goodId: 1,
                    labeledGoodId: 1,
                    currencyId: 1)
                .SetPrice(5M)
                .SetStatus(CheckItemStatus.Paid)
                .Build();

            checkItem.SetProperty(nameof(CheckItem.DatePaid), DateTime.UtcNow.AddDays(-daysToSubstract));

            return checkItem;
        }
    }
}
