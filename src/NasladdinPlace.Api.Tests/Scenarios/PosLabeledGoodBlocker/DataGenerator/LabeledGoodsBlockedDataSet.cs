using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.TestUtils.Seeding.Data;

namespace NasladdinPlace.Api.Tests.Scenarios.PosLabeledGoodBlocker.DataGenerator
{
    public class LabeledGoodsBlockedDataSet : DataSet<LabeledGood>
    {
        public static LabeledGoodsBlockedDataSet FromPosIdWithLabels(int posId, string label)
        {
            return new LabeledGoodsBlockedDataSet
            {
                PosId = posId,
                Label = label
            };
        }

        public int PosId { get; set; }

        public string Label { get; set; }

        protected override LabeledGood[] Data => new[]
        {
            LabeledGood.NewOfPosBuilder(PosId, Label)
                .TieToGood(
                    1,
                    new LabeledGoodPrice(5M, 1),
                    new ExpirationPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1))
                )
                .Build()
        };
    }
}
