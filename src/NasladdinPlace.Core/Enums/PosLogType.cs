using NasladdinPlace.Utilities.EnumHelpers;

namespace NasladdinPlace.Core.Enums
{
    public enum PosLogType
    {
        /// <summary>
        /// Sent automatically every day for prev day
        /// </summary>
        [EnumDescription("За сутки")]
        Daily,

        /// <summary>
        /// Current active logs file
        /// </summary>
        [EnumDescription("Текущие активные")]
        Active,

        /// <summary>
        /// Logs from archive folder
        /// </summary>
        [EnumDescription("Папка архив")]
        ArchiveFolder,

        /// <summary>
        /// Logs form current folder
        /// </summary>
        [EnumDescription("Папка с текущими логами")]
        CurrentFolder
    }
}
