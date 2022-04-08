using NasladdinPlace.Core.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NasladdinPlace.Api.Dtos.OneCSync.Purchases
{
    public class PosOperationTransactionDto
    {
        public decimal BonusAmount { get; set; }
        public decimal MoneyAmount { get; set; }
        public decimal FiscalizationAmount { get; set; }
        public DateTime? BankTransactionPaidDate { get; set; }
        public DateTime? FiscalizationDate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PosOperationTransactionType Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PosOperationTransactionStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public ICollection<BankTransactionInfoOneCSyncDto> BankTransactionInfos { get; set; }
        public ICollection<FiscalizationInfoOneCSyncDto> FiscalizationInfos { get; set; }
        public ICollection<PosOperationTransactionCheckItemDto> TransactionCheckItems { get; set; }

        public PosOperationTransactionDto()
        {
            BankTransactionInfos = new Collection<BankTransactionInfoOneCSyncDto>();
            FiscalizationInfos = new Collection<FiscalizationInfoOneCSyncDto>();
            TransactionCheckItems = new Collection<PosOperationTransactionCheckItemDto>();
        }
    }
}
