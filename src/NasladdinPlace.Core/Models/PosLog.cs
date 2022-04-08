using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Utils;

namespace NasladdinPlace.Core.Models
{
    public class PosLog : Entity
    {
        [Include]
        public Pos Pos { get; private set; }

        public int PosId { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        public string FileName { get; private set; }
        public byte[] FileContent { get; private set; }
        public  PosLogType LogType { get; private set; }

        protected PosLog()
        {
            DateTimeCreated = DateTime.UtcNow;
        }

        public PosLog(
            int posId,
            PosLogType logType,
            string fileName, 
            byte[] fileContent) : this()
        {
            PosId = posId;
            LogType = logType;
            FileName = fileName;
            FileContent = fileContent;
        }
    }
}
