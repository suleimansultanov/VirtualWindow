using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum LabeledGoodStatus
    {
        [EnumDescription("Товар ранее размещался на витринах")]
        GoodHasAlreadyInPointOfSale,
        [EnumDescription("Товар участвовал в продажах")]
        GoodHasAlreadyBeenSold
    }
}
