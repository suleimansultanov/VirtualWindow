using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers.Models
{
    public class GoodMovingReportRecord : BaseReportRecord, IReportRecord
    {
        [Display(Name = "Код витрины")]
        public int PosId { get; set; }

        [Display(Name = "Наименование витрины")]
        public string PosName { get; set; }

        [Display(Name = "Код категории")]
        public int GoodCategoryId { get; set; }

        [Display(Name = "Категория")]
        public string GoodCategory { get; set; }

        [Display(Name = "Код товара")]
        public int? GoodId { get; set; }

        [Display(Name = "Товар")]
        public string GoodName { get; set; }

        [Display(Name = "Было")]
        public int BalanceAtBegining { get; set; }

        [Display(Name = "Приход")]
        public int Income { get; set; }

        [Display(Name = "Расход")]
        public int Outcome { get; set; }

        [Display(Name = "Стало")]
        public int BalanceAtEnd { get; set; }

        [Display(Name = "Id документа")]
        public int DocumentId { get; set; }

        [Display(Name = "Номер строки")]
        public int LineNum { get; set; }

        [Display(Name = "Номер документа 1С")]
        public string DocumentNumber { get; set; }

        [Display(Name = "Дата создания")]
        public string CreatedDate { get; set; }

        [Display(Name = "Метки ДО")]
        public string LabelsAtBegining { get; set; }

        [Display(Name = "Метки ПОСЛЕ")]
        public string LabelsAtEnd { get; set; }
    }
}