using NasladdinPlace.UI.Managers.Reference.Attributes;
using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.UI.Managers.Reference.Enums
{
    /// <summary>
    /// Источники данных для контрола TextReference
    /// </summary>
    public enum TextReferenceSources
    {
        [EnumDescription("Медиа контент")]
        ImagesMediaContent,

        [EnumDescription("Пользователи")]
        [TextReferenceFilter("simplePage")]
        Users,

        [EnumDescription("Витрины")]
        [TextReferenceFilter("simplePage")]
        PointOfSales,

        [EnumDescription("Города")]
        [TextReferenceFilter("simplePage")]
        Cities,

        [EnumDescription("Метки")]
        [TextReferenceFilter("simplePage")]
        LabeledGoods,

        [EnumDescription("Товары")]
        [TextReferenceFilter("simplePage")]
        Goods,

        [EnumDescription("Категории")]
        [TextReferenceFilter("simplePage")]
        GoodCategories,

        [EnumDescription("Производители")]
        [TextReferenceFilter("simplePage")]
        Makers,
    }
}
