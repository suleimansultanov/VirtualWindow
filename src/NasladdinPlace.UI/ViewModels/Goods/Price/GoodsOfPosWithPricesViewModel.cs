using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NasladdinPlace.UI.Dtos.Good;
using NasladdinPlace.UI.Dtos.Pos;

namespace NasladdinPlace.UI.ViewModels.Goods.Price
{
    public class GoodsOfPosWithPricesViewModel
    {
        public SelectList PointsOfSaleSelectList { get; }
        
        public PosDto Pos { get; }
        public IEnumerable<GoodDto> Goods { get; }

        public int ShopId { get; set; }

        public GoodsOfPosWithPricesViewModel()
        {
            // intentionally left empty
        }

        public GoodsOfPosWithPricesViewModel(
            PosDto pos, 
            IEnumerable<GoodDto> goods, 
            SelectList pointsOfSaleSelectList)
        {
            Pos = pos ?? new PosDto();
            Goods = goods;
            PointsOfSaleSelectList = pointsOfSaleSelectList;
            ShopId = Pos.Id;
        }
    }
}