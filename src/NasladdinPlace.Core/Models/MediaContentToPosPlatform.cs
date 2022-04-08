using System;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Utils;

namespace NasladdinPlace.Core.Models
{
    public class MediaContentToPosPlatform : Entity
    {
        public int MediaContentId { get; private set; }

        public PosScreenType PosScreenType { get; private set; }

        public DateTime DateTimeCreated { get; private set; }

        [Include]
        public MediaContent MediaContent { get; private set; }

        protected MediaContentToPosPlatform() { }

        public MediaContentToPosPlatform(int mediaContentId, PosScreenType posScreenType)
        {
            MediaContentId = mediaContentId;
            PosScreenType = posScreenType;
            DateTimeCreated = DateTime.UtcNow;            
        }

        public void Update(int mediaContentId, PosScreenType posScreenType)
        {
            MediaContentId = mediaContentId;
            PosScreenType = posScreenType;
        }
    }
}