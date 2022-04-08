using System;
using System.IO;

namespace NasladdinPlace.Application.Services.FiscalizationInfos.Models
{
    public class QrCodeStream
    {
        public MemoryStream Value { get; }
        public string MimeType { get; }

        public QrCodeStream(MemoryStream value, string mimeType)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (mimeType == null)
                throw new ArgumentNullException(nameof(mimeType));
            
            Value = value;
            MimeType = mimeType;
        }
    }
}