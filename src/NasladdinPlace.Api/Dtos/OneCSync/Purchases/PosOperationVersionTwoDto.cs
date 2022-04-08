using NasladdinPlace.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Api.Dtos.OneCSync.Purchases
{
    public class PosOperationVersionTwoDto
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public DateTime? DateCompleted { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime? DateAuditCompleted { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PosOperationStatus OperationStatus { get; set; }
        public ICollection<GoodInCheckDto> Goods { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalCostWithoutDiscount { get; set; }
        public ICollection<PosOperationTransactionDto> PosOperationTransactions { get; set; }

        public PosOperationVersionTwoDto()
        {
            Goods = new Collection<GoodInCheckDto>();
            PosOperationTransactions = new Collection<PosOperationTransactionDto>();
        }
    }
}
