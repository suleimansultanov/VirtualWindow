using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers.Models
{
    public class ApproachRecord : BaseReportRecord, IReportRecord
    {
        [Display(Name = "Код устройства (UID)")]
        public string Id { get; set; }

        [Display(Name = "Url устройства (URL)")]
        public string Url { get; set; }

        [Display(Name = "SSID")]
        public string Ssid { get; set; }

        [Display(Name = "PSWD")]
        public string Pswd { get; set; }

        [Display(Name = "TIMEOUT")]
        public string Timeout { get; set; }

        [Display(Name = "TOKEN")]
        public string Token { get; set; }

        [Display(Name = "Дата приема данных")]
        public string ReceivedDate { get; set; }

        [Display(Name = "Промежуток времени")]
        public string TimeInterval { get; set; }

        [Display(Name = "Дистанции")]
        public string Distances { get; set; }
    }
}