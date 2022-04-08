using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class PosMediaContent
    {
        public MediaContent MediaContent { get; private set; }
        public Pos Pos { get; private set; }

        public int PosId { get; private set; }
        public int MediaContentId { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        public PosScreenType PosScreenType { get; private set; }

        protected PosMediaContent()
        {
            DateTimeCreated = DateTime.UtcNow;
        }

        public PosMediaContent(int posId, int mediaContentId, PosScreenType posScreenType) : this()
        {
            PosId = posId;
            MediaContentId = mediaContentId;
            PosScreenType = posScreenType;
        }
    }
}
