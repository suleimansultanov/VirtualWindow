using System.Collections;
using System.Collections.Generic;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Api.Tests.Scenarios.PosLabeledGoodBlocker.DataGenerator
{
    public class LabeledGoodsDataGenerator : IEnumerable<object[]>
    {
        private string[] SingleLabeledGood { get; } =
        {
            "E2 00 00 16 18 0B 01 66 15 20 7E EA"
        };

        private string[] DoubleLabeledGoods { get; } =
        {
            "E2 00 00 16 18 0B 01 66 15 20 7E EA",
            "E2 80 11 60 60 00 02 05 2A 98 4B A1"
        };

        private string[] TripleLabeledGoods { get; } =
        {
            "E2 00 00 16 18 0B 01 66 15 20 7E EA",
            "E2 80 11 60 60 00 02 05 2A 98 4B A1",
            "E2 80 11 60 60 00 02 05 2A 98 AB 11"
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new PosContent(1, new List<string>()), 0
            };
            yield return new object[]
            {
                new PosContent(1, SingleLabeledGood), 1
            };
            yield return new object[]
            {
                new PosContent(1, DoubleLabeledGoods), 2
            };
            yield return new object[]
            {
                new PosContent(1, TripleLabeledGoods), 3
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
