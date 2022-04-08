using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Tests.Scenarios.Reports.DailyReportContentBuilder.DataGenerators
{
    public class ExpiredLabeledGoodsDataGenerator : IEnumerable<object[]>
    {
        public const int ActiveOperationPosId = 1;
        public const int DefaultGoodId = 1;
        public const int DefaultCurrencyId = 1;
        public const string FirstLabel = "E2 00 00 16 18 0B 01 66 15 20 7E 2A";
        public const string SecondLabel = "E2 80 11 60 60 00 02 05 2A 98 4B 41";
        public const decimal DefaultLabeledGoodPrice = 5M;

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoods(ActiveOperationPosId, DefaultGoodId, FirstLabel),
                    CreateLabeledGoods(ActiveOperationPosId, DefaultGoodId, SecondLabel)
                }, 0M, PosMode.Purchase
            };
            yield return new object[]
            {
                new Collection<LabeledGood>
                {
                    CreateLabeledGoods(ActiveOperationPosId, DefaultGoodId, FirstLabel),
                    CreateLabeledGoods(ActiveOperationPosId, DefaultGoodId, SecondLabel)
                }, 10M, PosMode.GoodsIdentification
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static LabeledGood CreateLabeledGoods(int posId, int goodId, string label)
        {
            return LabeledGood.NewOfPosBuilder(posId, label)
                .TieToGood(goodId, new LabeledGoodPrice(DefaultLabeledGoodPrice, DefaultCurrencyId),
                    new ExpirationPeriod(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(1)))
                .Build();
        }
    }
}