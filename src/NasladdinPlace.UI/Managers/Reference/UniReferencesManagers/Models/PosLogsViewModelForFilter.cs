using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.UI.Managers.Reference.UniReferencesManagers.Models
{
    public class PosLogsViewModelForFilter
    {
        public int Id { get; set; }
        public int PosId { get; set; }
        public string AbbreviatedName { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public string FileName { get; set; }
        public PosLogType LogType { get; set; }
    }
}
