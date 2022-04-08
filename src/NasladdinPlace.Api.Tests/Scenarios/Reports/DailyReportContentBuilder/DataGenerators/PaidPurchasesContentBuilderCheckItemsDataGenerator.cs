using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder.DataGenerators
{
    public class PaidPurchasesContentBuilderCheckItemsDataGenerator : IEnumerable<object[]>
    {
        public const int ActiveOperationPosId = 1;
        public const int ActiveFirstOperationId = 1;
        public const int PaidActiveSecondOperationId = 2;
        public const int DefaultFirstLabeledGoodId = 1;
        public const int DefaultSecondLabeledGoodId = 2;
        public const int DefaultGoodId = 1;
        public const int DefaultCurrencyId = 1;

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, ActiveFirstOperationId)
                },
                0, 0M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, ActiveFirstOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultSecondLabeledGoodId, ActiveFirstOperationId)
                },
                0, 0M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, PaidActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Refunded, DefaultSecondLabeledGoodId, PaidActiveSecondOperationId)
                },
                0, 0M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, PaidActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Deleted, DefaultSecondLabeledGoodId, PaidActiveSecondOperationId)
                },
                0, 0M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId, PaidActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.PaidUnverified, DefaultSecondLabeledGoodId, PaidActiveSecondOperationId)
                },
                1, 5M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId, PaidActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Paid, DefaultSecondLabeledGoodId, PaidActiveSecondOperationId)
                },
                1, 10M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, ActiveFirstOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Paid, DefaultSecondLabeledGoodId, ActiveFirstOperationId)
                },
                0, 0M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId, ActiveFirstOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Paid, DefaultSecondLabeledGoodId, PaidActiveSecondOperationId)
                },
                1, 5M
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static CheckItem CreateCheckItem(int posId, CheckItemStatus status, int labeledGoodId, int posOperationId)
        {
            return CheckItem.NewBuilder(
                    posId,
                    posOperationId,
                    DefaultGoodId,
                    labeledGoodId,
                    DefaultCurrencyId)
                .SetPrice(5M)
                .SetStatus(status)
                .Build();
        }
    }
}