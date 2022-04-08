using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.TestUtils.Seeding.Data
{
    public class GoodsDataSet : DataSet<Good>
    {
        protected override Good[] Data => new[]
        {
            new Good(
                id: 0, 
                name: "Огурцы", 
                description: "Описание огурцов", 
                goodParameters: new GoodParameters(1, 0)),
            new Good(
                id: 0, 
                name: "Шоколадка", 
                description: "Описание шоколадки", 
                goodParameters: new GoodParameters(1, 0))
        };
    }
}