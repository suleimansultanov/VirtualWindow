using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum PosOperationTransactionStatus
    {
        [EnumDescription("Оплачен, фискализирован")]
        PaidFiscalized = 0,

        [EnumDescription("Неоплачен, не фискализирован")]
        Unpaid = 1,

        [EnumDescription("Оплачен, не фискализирован")]
        PaidUnfiscalized = 2,

        [EnumDescription("В процессе обработки")]
        InProcess = 3,

        [EnumDescription("Оплачен бонусами")]
        PaidByBonusPoints = 4
    }
}
