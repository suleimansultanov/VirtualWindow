using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Services.OverdueGoods.Models.Enums
{
    public enum OverdueType
    {
        [EnumDescription("Срок годности истек")]
        Overdue = 0,

        [EnumDescription("Срок годности менее 24 часов")]
        OverdueInDay = 1,

        [EnumDescription("Срок годности менее 48 часов, но более 24 часов")]
        OverdueBetweenTommorowAndNextDay = 2,

        [EnumDescription("Срок годности более 48 часов")]
        Fresh = 3,
    }
}
