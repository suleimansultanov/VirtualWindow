using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder.DataGenerators
{
    public class UnpaidPurchasesContentBuilderCheckItemsDataGenerator : IEnumerable<object[]>
    {
        public const int ActiveOperationPosId = 1;
        public const int ActiveFirstOperationId = 1;
        public const int CompleteActiveSecondOperationId = 2;
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
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, CompleteActiveSecondOperationId)
                },
                1, 5M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, CompleteActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultSecondLabeledGoodId, CompleteActiveSecondOperationId)
                },
                1, 10M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, ActiveFirstOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultSecondLabeledGoodId, CompleteActiveSecondOperationId)
                },
                2, 10M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Deleted, DefaultFirstLabeledGoodId, ActiveFirstOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultSecondLabeledGoodId, CompleteActiveSecondOperationId)
                },
                1, 5M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, CompleteActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Refunded, DefaultSecondLabeledGoodId, CompleteActiveSecondOperationId)
                },
                1, 5M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, CompleteActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Deleted, DefaultSecondLabeledGoodId, CompleteActiveSecondOperationId)
                },
                1, 5M
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId, CompleteActiveSecondOperationId),
                    CreateCheckItem(ActiveOperationPosId, CheckItemStatus.PaidUnverified, DefaultSecondLabeledGoodId,
                        ActiveFirstOperationId)
                },
                0, 0M
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static CheckItem CreateCheckItem(int posId, CheckItemStatus status, int labeledGoodId,
            int posOperationId)
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