using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum PermissionCategory
    {
        [EnumDescription("Администрирование")]
        Administration,
        [EnumDescription("Продажи")]
        Sales,
        [EnumDescription("Витрины")]
        PointOfSales,
        [EnumDescription("Общее")]
        Common,
        [EnumDescription("Маркетинг")]
        Marketing
    }
}
