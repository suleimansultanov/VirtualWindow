using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder.DataGenerators
{
    public class UnhandledConditionalPurchasesContentBuilderCheckItemsDataGenerator : IEnumerable<object[]>
    {
        public const int ActiveOperationPosId = 1;
        public const int PaidActiveSecondOperationId = 2;

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.PaidUnverified, 1, PaidActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.PaidUnverified, 2, PaidActiveSecondOperationId)
                }, 1
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.PaidUnverified, 1, PaidActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Paid, 2, PaidActiveSecondOperationId)
                }, 1
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Deleted, 1, PaidActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Refunded, 2, PaidActiveSecondOperationId)
                }, 0
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static CheckItem CreateCheckItem(int posId, CheckItemStatus status, int labeledGoodId, int posOperationId)
        {
            return CheckItem.NewBuilder(
                    posId,
                    posOperationId,
                    1,
                    labeledGoodId,
                    1)
                .SetPrice(5M)
                .SetStatus(status)
                .Build();
        }
    }
}