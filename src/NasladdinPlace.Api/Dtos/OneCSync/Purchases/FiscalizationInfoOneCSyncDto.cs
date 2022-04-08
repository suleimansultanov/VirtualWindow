using NasladdinPlace.CheckOnline.Builders.CheckOnline.Enums;
using NasladdinPlace.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace NasladdinPlace.Api.Dtos.OneCSync.Purchases
{
    public class FiscalizationInfoOneCSyncDto
    {
        public int Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FiscalizationType Type { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FiscalizationState State { get; set; }
        public string ErrorInfo { get; set; }
        public string DocumentInfo { get; set; }
        public string FiscalizationNumber { get; set; }
        public string FiscalizationSerial { get; set; }
        public string FiscalizationSign { get; set; }
        public DateTime DocumentDateTime { get; set; }
        public decimal CorrectionAmount { get; set; }
        public decimal TotalFiscalizationAmount { get; set; }
    }
}