using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Api.Dtos.SimpleCheck
{
    public class SimpleCheckDto
    {
        public int Id { get; set; }
        public ICollection<SimpleCheckItemDto> Items { get; set; }
        public DateTime DateCreated { get; set; }
        public SimpleCheckSummaryDto Summary { get; set; }
        public SimpleCheckOriginInfoDto OriginInfo { get; set; }
        public SimpleCheckFiscalizationInfoDto FiscalizationInfo { get; set; }
        public DateTime? DatePaid { get; set; }
        public CheckCorrectnessStatus CorrectnessStatus { get; set; }
        public SimpleCheckPaymentErrorInfoDto PaymentError { get; set; }
    }
}