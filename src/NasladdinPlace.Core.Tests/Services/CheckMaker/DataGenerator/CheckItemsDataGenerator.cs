using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.Core.Tests.Services.CheckMaker.DataGenerator
{
    public class CheckItemsDataGenerator : IEnumerable<object[]>
    {
        private readonly Good _defaultGood = Good.Unknown;

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 0M)

                },
                1, 0M, 0M, 1, true
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Refunded, 10M)

                },
                1, 0M, 0M, 0, true
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Deleted, 10M)

                },
                0, 0M, 0M, 0, true
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 10M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 25M)

                },
                2, 35.0M, 35.0M, 2, false
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    //is empty
                },
                0, 0M, 0M, 0, true
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Refunded, 10M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Refunded, 25M)
                },
                2, 0M, 0M, 0, true
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Deleted, 10M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Deleted, 25M)
                },
                0, 0M, 0M, 0, true
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 10M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Deleted, 25M)
                },
                1, 10.0M, 10.0M, 1, false
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 10M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Refunded, 25M)
                },
                2, 10.0M, 10.0M, 1, false
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 10M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Refunded, 25M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Deleted, 15M)
                },
                2, 10.0M, 10.0M, 1, false
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Refunded, 10M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Deleted, 25M)
                },
                1, 0M, 0M, 0, true
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 10M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 25M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Refunded, 25M)
                },
                3, 35.0M, 35.0M, 2, false
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 10M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 25M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 25M)
                },
                2, 60.0M, 60.0M, 3, false
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 25M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 25M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 25M)
                },
                1, 75.0M, 75.0M, 3, false
            };
            yield return new object[]
            {
                new Collection<CheckItem>
                {
                    CreateCheckItem(_defaultGood, CheckItemStatus.PaidUnverified, 25M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 25M),
                    CreateCheckItem(_defaultGood, CheckItemStatus.Paid, 25M)
                },
                2, 75.0M, 75.0M, 3, false
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static CheckItem CreateCheckItem(Good defaultGood, CheckItemStatus status, decimal price)
        {
            var currency = Currency.Ruble;
            var posId = Pos.Default.Id;
            var posOperationId = PosOperation.NewOfUserAndPosBuilder(1, 0).Build().Id;
            var labeledGood = LabeledGood.OfPos(0, "Label");
            
            return CheckItem.NewBuilder(posId, posOperationId, defaultGood.Id, labeledGood.Id, currency.Id)
                .SetPrice(price)
                .SetStatus(status)
                .SetCurrency(currency)
                .SetLabeledGood(labeledGood)
                .Build();

        }
    }
}

