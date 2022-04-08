using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum PosOperationStatus
    {
        [EnumDescription("Открыта")]
        Opened = 0,
        
        [EnumDescription("Ожидает завершения")]
        PendingCompletion = 5,

        [EnumDescription("Ожидает формирования чека")]
        PendingCheckCreation = 1,

        [EnumDescription("Завершена")]
        Completed = 2,

        [EnumDescription("Ожидает оплаты")]
        PendingPayment = 3,

        [EnumDescription("Оплачена")]
        Paid = 4
    }
}
