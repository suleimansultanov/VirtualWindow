using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum ReportType
    {
        [EnumDescription("Выгрузка остатков товаров")]
        PointsOfSaleContent = 0,
        [EnumDescription("Ежедневный отчет о продажах")]
        DailyPurchaseStatistics = 1,
        [EnumDescription("Информация о перемещении товаров")]
        GoodsMovingInfo = 2
    }
}
