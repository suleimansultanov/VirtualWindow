using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum PosMode
    {
        [EnumDescription("Покупка")]
        Purchase = 0,

        [EnumDescription("Идентификация")]
        GoodsIdentification = 1,

        [EnumDescription("Перемещение ТМЦ")]
        GoodsPlacing = 2
    }
}
