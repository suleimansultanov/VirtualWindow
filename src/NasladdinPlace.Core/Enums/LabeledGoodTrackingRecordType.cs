using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum LabeledGoodTrackingRecordType
    {
        [EnumDescription("Потеряна")]
        Lost = 0,
        
        [EnumDescription("Найдена")]
        Found = 1
    }
}