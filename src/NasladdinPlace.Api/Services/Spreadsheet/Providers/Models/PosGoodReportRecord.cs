using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers.Models
{
    public class PosGoodReportRecord : BaseReportRecord, IReportRecord
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
        public int GoodId { get; set; }

        [Display(Name = "Товар")]
        public string GoodName { get; set; }

        [Display(Name = "Кол-во, шт")]
        public int GoodCount { get; set; }

        [Display(Name = "Цена, руб")]
        public decimal PricePerItem { get; set; }

        [Display(Name = "Сумма, руб")]
        public decimal Price { get; set; }
    }
}
