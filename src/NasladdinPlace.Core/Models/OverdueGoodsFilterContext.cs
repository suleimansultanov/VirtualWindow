using NasladdinPlace.Core.Services.OverdueGoods.Models.Enums;

namespace NasladdinPlace.Core.Models
{
    public class OverdueGoodsFilterContext
    {
        public int? PosId { get; set; }
        public OverdueType? Type { get; set; }
    }
}