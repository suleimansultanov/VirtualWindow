using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum FiscalizationState
    {
        [EnumDescription("Ожидает фискализации")]
        Pending = 0,

        [EnumDescription("Фискализирован")]
        Success = 1,

        [EnumDescription("Фискализирован с ошибкой, ожидает фискализации")]
        PendingError = 2,

        [EnumDescription("В процессе обработки")]
        InProcess = 3,

        [EnumDescription("Ошибка фискализации")]
        Error = 4,
    }
}
