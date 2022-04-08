using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NasladdinPlace.Api.Dtos.OneCSync.Purchases
{
    public class PosOperationDto
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
        public decimal BonusAmount { get; set; }
        public decimal TotalCostWithoutDiscount { get; set; }
        public ICollection<BankTransactionInfoOneCSyncDto> BankTransactionInfos { get; set; }
        public ICollection<FiscalizationInfoOneCSyncDto> FiscalizationInfos { get; set; }

        public PosOperationDto()
        {
            Goods = new Collection<GoodInCheckDto>();
            BankTransactionInfos = new Collection<BankTransactionInfoOneCSyncDto>();
            FiscalizationInfos = new Collection<FiscalizationInfoOneCSyncDto>();
        }
    }
}
