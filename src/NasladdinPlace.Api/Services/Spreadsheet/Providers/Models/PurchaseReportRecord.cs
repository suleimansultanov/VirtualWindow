using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Api.Services.Spreadsheet.Providers.Models
{
    public class PurchaseReportRecord : BaseReportRecord, IReportRecord
    {
        [Display(Name = "Код чека")]
        public int Id { get; set; }

        [Display(Name="Дата и время покупки")]
        public string DateTime { get; set; }

        [Display(Name = "Дата покупки")]
        public string Date { get; set; }

        [Display(Name = "Время покупки")]
        public string Time { get; set; }

        [Display(Name = "Дата оплаты")]
        public string DatePaid { get; set; }

        [Display(Name = "Время оплаты")]
        public string TimePaid { get; set; }

        [Display(Name = "Код пользователь")]
        public int UserId { get; set; }

        [Display(Name = "Пользователь")]
        public string UserName { get; set; }

        [Display(Name = "Код витрины")]
        public int PosId { get; set; }

        [Display(Name = "Наименование витрины")]
        public string PosName { get; set; }

        [Display(Name = "Код категории")]
        public int GoodCategoryId { get; set; }

        [Display(Name = "Категория")]
        public string GoodCategoryName { get; set; }

        [Display(Name = "Код товара")]
        public int GoodId { get; set; }

        [Display(Name = "Товар")]
        public string GoodName { get; set; }

        [Display(Name = "Кол-во, шт")]
        public int GoodCount { get; set; }

        [Display(Name = "Цена, шт")]
        public decimal PricePerItem { get; set; }

        [Display(Name = "Скидка, шт")]
        public decimal Discount { get; set; }

        [Display(Name = "Сумма покупки")]
        public decimal Price { get; set; }

        [Display(Name = "Бонусы")]
        public decimal Bonuses { get; set; }

        [Display(Name = "Итого")]
        public decimal ActualPrice { get; set; }

        [Display(Name = "Условная покупка")]
        public string IsConditionalPurchase { get; set; }

    }
}
