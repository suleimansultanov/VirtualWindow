using NasladdinPlace.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NasladdinPlace.Api.Dtos.OneCSync.Purchases
{
    public class GoodInCheckDto
    {
        public int Id { get; set; }
        public int CheckItemId { get; set; }
        [JsonIgnore]
        public int ShopId { get; set; }
        [JsonIgnore]
        public int CheckId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CheckItemStatus Status { get; set; }
        public decimal Sum { get; set; }
        public decimal SumWithoutDiscount { get; set; }
    }
}