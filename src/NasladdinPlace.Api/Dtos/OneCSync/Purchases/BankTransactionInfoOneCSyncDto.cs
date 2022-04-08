using System;
using NasladdinPlace.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NasladdinPlace.Api.Dtos.OneCSync.Purchases
{
    public class BankTransactionInfoOneCSyncDto
    {
        public int Id { get; set; }
        public int BankTransactionId { get; set; }
        [JsonIgnore]
        public int PosOperationId { get; set; }
        public decimal Amount { get; set; } 
        public DateTime DateCreated { get; set; } 
        [JsonConverter(typeof(StringEnumConverter))]
        public BankTransactionInfoType Type { get; set; }
        public string Comment { get; set; }
    }
}