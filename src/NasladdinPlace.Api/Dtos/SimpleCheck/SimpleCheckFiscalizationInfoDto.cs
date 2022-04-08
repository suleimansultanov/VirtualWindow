using System;

namespace NasladdinPlace.Api.Dtos.SimpleCheck
{
    public class SimpleCheckFiscalizationInfoDto
    {
        public string FiscalNumber { get; set; }
        public string Sign { get; set; }
        public DateTime? DateOfPurchase { get; set; }
        public string QrCodeLink { get; set; }
        public string QrCodeValue { get; set; }
        public string FiscalCheckLink { get; set; }
    }
}
