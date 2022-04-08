using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum GoodPublishingStatus
    {
        [EnumDescription("Не опубликовано")]
        NotPublished = 0,

        [EnumDescription("Подготовка к публикации")]
        PrepareForPublish = 1,

        [EnumDescription("Опубликовано")]
        Published = 2
    }
}
