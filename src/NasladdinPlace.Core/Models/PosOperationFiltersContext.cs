using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models.Reference;

namespace NasladdinPlace.Core.Models
{
    public class PosOperationFiltersContext
    {
        public DateTime? OperationDateFrom { get; set; }
        public DateTime? OperationDateUntil { get; set; }
        public DateTime? AuditRequestDateTimeFrom { get; set; }
        public DateTime? AuditRequestDateTimeUntil { get; set; }
        public PosOperationStatus? OperationStatus { get; set; }
        public FilterTypes? OperationStatusFilterType { get; set; } 
        public decimal? TotalPrice { get; set; }
        public FilterTypes? TotalPriceFilterType { get; set; }
        public bool? HasUnverifiedCheckItems { get; set; }
        public bool? HasFiscalizationInfoErrors { get; set; }
        public PosMode? OperationMode { get; set; }
    }
}
