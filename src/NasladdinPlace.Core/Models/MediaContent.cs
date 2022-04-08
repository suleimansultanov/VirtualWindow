using System;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class MediaContent : Entity
    {
        public MediaContentType ContentType { get; private set; }

        public string FileName { get; private set; }

        public DateTime UploadDateTime { get; private set; }

        public byte[] FileContent { get; private set; }

        protected MediaContent()
        {
            FileContent = new byte[0];
        }

        public MediaContent(MediaContentType contentType,
                            string fileName,
                            byte[] fileContent)
        {
            ContentType = contentType;
            FileName = fileName;
            FileContent = fileContent;
            UploadDateTime = DateTime.UtcNow;
        }
    }
}
