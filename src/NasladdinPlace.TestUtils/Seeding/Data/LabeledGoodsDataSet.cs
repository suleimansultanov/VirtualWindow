using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class LabeledGoodsDataSet : DataSet<LabeledGood>
    {
        public static LabeledGoodsDataSet FromPosId(int posId)
        {
            return new LabeledGoodsDataSet
            {
                PosId = posId
            };
        }

        public static LabeledGoodsDataSet FromPosIdWithLabels(int posId, params string[] labels)
        {
            return new LabeledGoodsDataSet
            {
                PosId = posId,
                Labels = labels
            };
        }
        
        public int PosId { get; private set; }

        public string[] Labels { get; set; } =
        {
            "E2 00 00 16 18 0B 01 66 15 20 7E EA", 
            "E2 80 11 60 60 00 02 05 2A 98 4B A1", 
            "E2 80 11 60 60 00 02 05 2A 98 AB 11"
        };
        
        protected override LabeledGood[] Data => new[]
        {
            LabeledGood.NewOfPosBuilder(PosId, Labels[0])
                .TieToGood(
                    1, 
                    new LabeledGoodPrice(5M, 1), 
                    new ExpirationPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1))
                )
                .Build(),
            LabeledGood.NewOfPosBuilder(PosId, Labels[1])
                .TieToGood(
                    1, 
                    new LabeledGoodPrice(5M, 1), 
                    new ExpirationPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1))
                )
                .Build(),
            LabeledGood.NewOfPosBuilder(PosId, Labels[2])
                .TieToGood(
                    1, 
                    new LabeledGoodPrice(15M, 1), 
                    new ExpirationPeriod(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1))
                )
                .Build()
        };
    }
}